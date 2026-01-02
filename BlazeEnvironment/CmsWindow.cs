using System.CodeDom;
using System.Reflection;
using System.Text;
using BlazeEngine;
using BlazeEngine.ResourcesManagement;

namespace BlazeEnvironment;

public class SerializedCmsComponent
{
    public string typeName = string.Empty;
    public Dictionary<string, object> fields = new();

    public SerializedCmsComponent() {}
    public SerializedCmsComponent(Type type)
    {
        typeName = type.FullName;
        foreach (var field in type.GetFields())
        {
            fields[field.Name] = Activator.CreateInstance(field.FieldType)!;
        }
    }
    public SerializedCmsComponent(CmsComponent cmsComponent)
    {
        var type = cmsComponent.GetType();
        typeName = type.FullName;
        foreach (var field in type.GetFields())
        {
            fields[field.Name] = Convert.ChangeType(field.GetValue(cmsComponent), field.FieldType)!;
        }
    }

    public CmsComponent AsCmsComponent(List<Type> cmsComponentTypes)
    {
        Type type = null;
        foreach (var t in cmsComponentTypes)
        {
            if (t.FullName == typeName)
            {
                type = t;
                break;
            }
        }
        if (type == null)
        {
            Debug.LogError($"No type with name {typeName} found!");
            return null!;
        }
        var comp = (CmsComponent)Activator.CreateInstance(type)!;
        foreach (var field in comp.GetType().GetFields())
        {
            if (fields.TryGetValue(field.Name, out var obj))
            {
                if (field.FieldType == obj.GetType()) 
                    field.SetValue(comp, obj);
                else 
                    field.SetValue(comp, Convert.ChangeType(obj, field.FieldType));
            }
        }
        return comp;
    }
}

public class SerializedCmsEntity
{
    public string id = string.Empty;
    public List<SerializedCmsComponent> components = new();

    public SerializedCmsEntity() {}
    public SerializedCmsEntity(string id)
    {
        this.id = id;
    }
    public SerializedCmsEntity(CmsEntity cmsEntity)
    {
        id = cmsEntity.id;
        foreach (var comp in cmsEntity.Components)
        {
            components.Add(new SerializedCmsComponent(comp));
        }
    }
    
    public CmsEntity AsCmsEntity(List<Type> cmsComponentTypes)
    {
        CmsEntity ent = Activator.CreateInstance<CmsEntity>();
        ent.id = id;
        foreach (var comp in components)
        {
            var cmsComp = comp.AsCmsComponent(cmsComponentTypes);
            if (cmsComp != null)
            {
                ent.AddComponent(cmsComp);
            }
        }
        return ent;
    }
}

public interface ISerializedFieldDrawer
{
    public void Display(FieldInfo fieldInfo, object target);
}

public class SerializedStringDrawer : ISerializedFieldDrawer
{
    public void Display(FieldInfo fieldInfo, object target)
    {
        var sb = new StringBuilder((string)fieldInfo.GetValue(target), 256);
        ImGui.InputText(fieldInfo.Name, sb);
        fieldInfo.SetValue(target, sb.ToString());
    }
}

public class SerializedIntDrawer : ISerializedFieldDrawer
{
    public void Display(FieldInfo fieldInfo, object target)
    {
        var val = (int)fieldInfo.GetValue(target);
        ImGui.InputInt(fieldInfo.Name, ref val);
        fieldInfo.SetValue(target, val);
    }
}

public class SerializedFloatDrawer : ISerializedFieldDrawer
{
    public void Display(FieldInfo fieldInfo, object target)
    {
        var val = (float)fieldInfo.GetValue(target);
        ImGui.InputFloat(fieldInfo.Name, ref val);
        fieldInfo.SetValue(target, val);
    }
}

public class AssemblySystem
{
    public Assembly assembly;

    public event Action<Assembly> onAssemblyLoaded;

    public ProjectInfoSystem projectInfoSystem;
    public ProjectBuildSystem projectBuildSystem;

    public AssemblySystem(ProjectInfoSystem projectInfoSystem, ProjectBuildSystem projectBuildSystem)
    {
        this.projectInfoSystem = projectInfoSystem;
        this.projectBuildSystem = projectBuildSystem;
    }
    
    public void Load()
    {
        string path = Path.Combine(projectBuildSystem.GetBuildDirectory(), projectInfoSystem.projectInfo.projectName + ".dll");
        if (File.Exists(path))
        {
            assembly = Assembly.LoadFrom(path);
            onAssemblyLoaded?.Invoke(assembly);
        }
    }

    public void Unload()
    {
        assembly = null;
    }
}

public class CmsEntityWindow
{
    public bool active = true;

    public CmsEntity activeCmsEntity;
    public string cmsEntityPath;
    
    public AssemblySystem assemblySystem;
    public List<Type> cmsComponentTypes = new();

    public Dictionary<Type, ISerializedFieldDrawer> fieldDrawers;
    
    public CmsEntityWindow(AssemblySystem assemblySystem)
    {
        this.assemblySystem = assemblySystem;
        assemblySystem.onAssemblyLoaded += assembly =>
        {
            cmsComponentTypes = assembly.
                GetTypes().
                Where(t => typeof(CmsComponent).IsAssignableFrom(t)).
                ToList();
        };
        fieldDrawers = new()
        {
            {typeof(string), new SerializedStringDrawer()},
            {typeof(int), new SerializedIntDrawer()},
            {typeof(float), new SerializedFloatDrawer()},
        };
    }

    public void Display()
    {
        if (!active)
        {
            return;
        }

        ImGui.Begin("Cms Entity Viewer", ref active);

        if (activeCmsEntity != null)
        {
            StringBuilder sb = new(activeCmsEntity.id, 256);
            if (ImGui.InputText("Id", sb))
            {
                activeCmsEntity.id = sb.ToString();
            }
            
            if (ImGui.BeginTable("Cms Components", 1))
            {
                int removeId = -1;
                for (int i = 0; i < activeCmsEntity.Components.Count; i++)
                {
                    ImGui.PushId(i);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    
                    ImGui.Text(activeCmsEntity.Components[i].GetType().Name);
                    ImGui.SameLine();
                    if (ImGui.Button("-"))
                    {
                        removeId = i;
                    }
                    foreach (var field in activeCmsEntity.Components[i].GetType().GetFields())
                    {
                        if (fieldDrawers.TryGetValue(field.FieldType, out var drawer))
                        {
                            drawer.Display(field, activeCmsEntity.Components[i]);
                        }
                    }
                    
                    ImGui.PopId();
                }
                if (removeId != -1)
                {
                    activeCmsEntity.Components.RemoveAt(removeId);
                }

                ImGui.EndTable();
            }

            if (ImGui.Button("+"))
            {
                ImGui.OpenPopup("Add Cms Component");
            }
            if (ImGui.BeginPopup("Add Cms Component"))
            {
                if (ImGui.BeginTable("Cms Component Types", 1))
                {
                    for (int i = 0; i < cmsComponentTypes.Count; i++)
                    {
                        ImGui.PushId(i);
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.Button(cmsComponentTypes[i].Name))
                        {
                            activeCmsEntity.AddComponent((CmsComponent)Activator.CreateInstance(cmsComponentTypes[i])!);
                        }
                        ImGui.PopId();
                    }
                        
                    ImGui.EndTable();
                }
                    
                ImGui.EndPopup();
            }

            if (ImGui.Button("Save"))
            {
                
                SaveCmsEntity();
            }
        }

        ImGui.End();
    }

    public void LoadCmsEntity(string path)
    {
        if (assemblySystem.assembly == null || !File.Exists(path))
        {
            return;
        }
        cmsEntityPath = path;
        Debug.Log(path);
        activeCmsEntity = YAML.DeserializeFromFile<SerializedCmsEntity>(cmsEntityPath).AsCmsEntity(cmsComponentTypes);
        Debug.Log(1);
    }

    public void SaveCmsEntity()
    {
        YAML.SerializeToFile(cmsEntityPath, new SerializedCmsEntity(activeCmsEntity));
    }
}

public class CmsWindow
{
    public CmsEntityWindow cmsEntityWindow;
    public AssemblySystem assemblySystem;
    public ProjectInfoSystem projectInfoSystem;

    public string curDir = string.Empty;
    
    public bool active = true;

    public CmsWindow(CmsEntityWindow cmsEntityWindow, AssemblySystem assemblySystem, ProjectInfoSystem projectInfoSystem)
    {
        this.cmsEntityWindow = cmsEntityWindow;
        this.assemblySystem = assemblySystem;
        this.projectInfoSystem = projectInfoSystem;
        projectInfoSystem.onLoad += () =>
        {
            curDir = Path.Combine(projectInfoSystem.projectInfo.projectRoot, projectInfoSystem.projectInfo.projectName);
        };
    }
    
    public void Display()
    {
        if (!active)
        {
            return;
        }
        
        ImGui.Begin("Cms Viewer", ref active);
        
        if (ImGui.BeginPopupContextWindow())
        {
            if (ImGui.MenuItem("New CmsEntity"))
            {
                int i = 0;
                string entName, path;
                do
                {
                    entName = $"New Cms Entity {i}";
                    path = Path.Combine(curDir, entName) + ".ent";
                } while (File.Exists(path) && i++ < 1000);

                YAML.SerializeToFile(path, new SerializedCmsEntity(entName));
            }

            if (ImGui.MenuItem("Create Directory"))
            {
                int i = 0;
                string dirName, path;
                do
                {
                    dirName = "New Directory" + i;
                    path = Path.Combine(curDir, dirName);
                } while (Directory.Exists(path) && i++ < 1000);
                
                Directory.CreateDirectory(path);
            }
            ImGui.EndPopup();
        }

        if (!string.IsNullOrEmpty(curDir))
        {
            if (ImGui.Button("<-"))
            {
                curDir = Directory.GetParent(curDir).FullName;
            }
            ImGui.Separator();
            ImGui.Columns(8, borders: false);
            foreach (var i in Directory.GetDirectories(curDir))
            {
                if (ImGui.Button(Path.GetFileName(i)))
                {
                    curDir = i;
                }
                ImGui.NextColumn();
            }
            foreach (var i in Directory.GetFiles(curDir))
            {
                if (ImGui.Button(Path.GetFileNameWithoutExtension(i)))
                {
                    if (Path.GetExtension(i) == ".ent")
                    {
                        cmsEntityWindow.LoadCmsEntity(i);
                    }
                }
                ImGui.NextColumn();
            }
            ImGui.Columns(1);
            ImGui.Separator();
        }

        if (ImGui.Button("Load Assembly"))
        {
            assemblySystem.Load();
        }
        ImGui.SameLine();
        if (ImGui.Button("Unload Assembly"))
        {
            assemblySystem.Unload();
        }
            
        ImGui.End();
    }
}