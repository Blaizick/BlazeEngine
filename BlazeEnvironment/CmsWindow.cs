using System.Reflection;
using System.Text;
using BlazeEngine;
using BlazeEngine.ResourcesManagement;

namespace BlazeEnvironment;

//TODO Gen

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

public class CmsEntitySystem
{
    public CmsEntity activeCmsEntity;
    public string cmsEntityPath;
    
    public AssemblySystem assemblySystem;
    
    public List<Type> cmsComponentTypes = new();

    public CmsEntitySystem(AssemblySystem assemblySystem)
    {
        this.assemblySystem = assemblySystem;
        
        assemblySystem.onAssemblyLoaded += assembly =>
        {
            cmsComponentTypes = assembly.
                GetTypes().
                Where(t => typeof(CmsComponent).IsAssignableFrom(t)).
                ToList();
        };
    }

    public void CreateComponent(Type type)
    {
        activeCmsEntity.AddComponent((CmsComponent)Activator.CreateInstance(type)!);
    }
    
    public void LoadCmsEntity(string path)
    {
        if (assemblySystem.assembly == null || !File.Exists(path))
        {
            return;
        }
        cmsEntityPath = path;
        activeCmsEntity = YAML.DeserializeFromFile<SerializedCmsEntity>(cmsEntityPath).AsCmsEntity(cmsComponentTypes);
    }

    public void SaveCmsEntity()
    {
        YAML.SerializeToFile(cmsEntityPath, activeCmsEntity.AsSerializedCmsEntity());
    }
}

public class CmsEntityWindow
{
    public bool active = true;

    public CmsEntitySystem cmsEntitySystem;
    
    public Dictionary<Type, ISerializedFieldDrawer> fieldDrawers;
    
    public CmsEntityWindow(CmsEntitySystem cmsEntitySystem)
    {
        this.cmsEntitySystem = cmsEntitySystem;

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

        var activeCmsEntity = cmsEntitySystem.activeCmsEntity;
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
                    var cmsComponentTypes = cmsEntitySystem.cmsComponentTypes;
                    for (int i = 0; i < cmsComponentTypes.Count; i++)
                    {
                        ImGui.PushId(i);
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        if (ImGui.Button(cmsComponentTypes[i].Name))
                        {
                            cmsEntitySystem.CreateComponent(cmsComponentTypes[i]);
                        }
                        ImGui.PopId();
                    }
                        
                    ImGui.EndTable();
                }
                    
                ImGui.EndPopup();
            }

            if (ImGui.Button("Save"))
            {
                cmsEntitySystem.SaveCmsEntity();
            }
        }

        ImGui.End();
    }
}

public class CmsSystem
{
    public ProjectInfoSystem projectInfoSystem;
    
    public string curDir = string.Empty;
    
    
    public enum RenamingState
    {
        None,
        Directory,
        File
    };

    public RenamingState renamingState;
    public string renamingItemName;
    public string renamingItemPath;
    
    public bool RenamingFile => renamingState == RenamingState.File;
    public bool RenamingDirectory => renamingState == RenamingState.Directory;
    
    public CmsSystem(ProjectInfoSystem projectInfoSystem)
    {
        this.projectInfoSystem = projectInfoSystem;
        projectInfoSystem.onLoad += () =>
        {
            curDir = Path.Combine(projectInfoSystem.projectInfo.projectRoot, projectInfoSystem.projectInfo.projectName);
        };
    }

    public void StartRenaming(string item, bool dir)
    {
        renamingItemPath = item;
        renamingItemName = Path.GetFileName(item);
        renamingState = dir ? RenamingState.Directory : RenamingState.File;
    }

    public void StopRenaming()
    {
        renamingState = RenamingState.None;
    }

    public void Rename()
    {
        string newPath = Path.Combine(Path.GetDirectoryName(renamingItemPath), renamingItemName);
        if (renamingState == RenamingState.Directory)
        {
            if (!Directory.Exists(newPath))
            {
                Directory.Move(renamingItemPath, newPath);
            }
        }
        else
        {
            if (!File.Exists(newPath))
            {
                File.Move(renamingItemPath, newPath);
            }
        }
        renamingState = RenamingState.None;
    }

    public void CreateDir()
    {
        int i = 0;
        string path;
        do 
        {
            path = Path.Combine(curDir, $"Directory {i}");
        } while (Directory.Exists(path) && i++ < 1000);
        
        Directory.CreateDirectory(path);
    }
    public void CreateCmsEntity()
    {
        var file = GetUniqueFile("Cms Entity", ".ent");
        YAML.SerializeToFile(file.path, new SerializedCmsEntity(file.name));
    }
    public (string name, string path) GetUniqueFile(string fileName, string extension)
    {
        int i = 0;
        string path, name;
        do 
        { 
            name = $"{fileName} {i}";
            path = Path.Combine(curDir, name) + extension;
        } while (File.Exists(path) && i++ < 1000);
        return (name, path);
    }
}

public class CmsWindow
{
    public CmsSystem cmsSystem;
    public CmsEntitySystem cmsEntitySystem;
    public AssemblySystem assemblySystem;

    public bool active = true;
    
    
    public CmsWindow(CmsSystem cmsSystem, CmsEntitySystem cmsEntitySystem, AssemblySystem assemblySystem)
    {
        this.cmsSystem = cmsSystem;
        this.cmsEntitySystem = cmsEntitySystem;
        this.assemblySystem = assemblySystem;
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
                cmsSystem.CreateCmsEntity();
            }
            if (ImGui.MenuItem("New Directory"))
            {
                cmsSystem.CreateDir();
            }
            ImGui.EndPopup();
        }

        if (!string.IsNullOrEmpty(cmsSystem.curDir))
        {
            if (ImGui.Button("<-"))
            {
                cmsSystem.curDir = Path.GetDirectoryName(cmsSystem.curDir);
            }
            ImGui.Separator();
            ImGui.Columns(8, borders: false);

            string deleteDir = null;
            foreach (var dir in Directory.GetDirectories(cmsSystem.curDir))
            {
                if (cmsSystem.RenamingDirectory && cmsSystem.renamingItemPath == dir)
                {
                    StringBuilder sb = new(cmsSystem.renamingItemName, 256);
                    ImGui.SetKeyboardFocusHere();
                    if (ImGui.InputText("##DirectoryRenaming", sb))
                    {
                        cmsSystem.renamingItemName = sb.ToString();
                    }

                    if (ImGui.IsKeyPressed(ImGuiKey.ImGuiKey_Enter) || ImGui.IsMouseClicked(ImGuiMouseButton.ImGuiMouseButton_Left))
                    {
                        cmsSystem.Rename();
                    }
                    else if (ImGui.IsKeyPressed(ImGuiKey.ImGuiKey_Escape))
                    {
                        cmsSystem.StopRenaming();
                    }
                }
                else
                {
                    if (ImGui.Button(Path.GetFileName(dir)))
                    {
                        cmsSystem.curDir = dir;
                    }    
                }
                if (ImGui.BeginPopupContextItem($"FileСontextMenu{dir}"))
                {
                    if (ImGui.MenuItem("Rename"))
                    {
                        cmsSystem.StartRenaming(dir, true);
                    }
                    if (ImGui.MenuItem("Delete"))
                    {
                        deleteDir = dir;
                    }
                    ImGui.EndPopup();
                }
                ImGui.NextColumn();
            }
            if (deleteDir != null)
            {
                Directory.Delete(deleteDir, true);
            }

            string deleteFile = null;
            foreach (var file in Directory.GetFiles(cmsSystem.curDir))
            {
                if (cmsSystem.RenamingFile && cmsSystem.renamingItemPath == file)
                {
                    StringBuilder sb = new(cmsSystem.renamingItemName, 256);
                    ImGui.SetKeyboardFocusHere();
                    if (ImGui.InputText("##FileRenaming", sb))
                    {
                        cmsSystem.renamingItemName = sb.ToString();
                    }

                    if (ImGui.IsKeyPressed(ImGuiKey.ImGuiKey_Enter))
                    {
                        cmsSystem.Rename();
                    }
                    else if (ImGui.IsKeyPressed(ImGuiKey.ImGuiKey_Escape) || ImGui.IsMouseClicked(ImGuiMouseButton.ImGuiMouseButton_Left))
                    {
                        cmsSystem.StopRenaming();
                    }
                }
                else
                {
                    if (ImGui.Button(Path.GetFileName(file)))
                    {
                        if (Path.GetExtension(file) == ".ent")
                        {
                            cmsEntitySystem.LoadCmsEntity(file);
                        }
                    }
                }
                if (ImGui.BeginPopupContextItem($"FileСontextMenu{file}"))
                {
                    if (ImGui.MenuItem("Rename"))
                    {
                        cmsSystem.StartRenaming(file, false);
                    }
                    if (ImGui.MenuItem("Delete"))
                    {
                        deleteFile = file;
                    }
                    ImGui.EndPopup();
                }
                ImGui.NextColumn();
            }
            if (deleteFile != null)
            {
                File.Delete(deleteFile);
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