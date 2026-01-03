using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace BlazeEngine.ResourcesManagement;

public class CmsEntity
{
    public string id = string.Empty;
    private List<CmsComponent> m_Components = new();
    
    public List<CmsComponent> Components => m_Components;
    
    public CmsEntity() {}
    public CmsEntity(string id)
    {
        this.id = id;
    }

    public void AddComponent(CmsComponent component)
    {
        m_Components.Add(component);
    }

    public T GetComponent<T>() where T : CmsComponent
    {
        return (T)m_Components.Find(comp => comp is T)!;
    }

    public bool TryGet<T>(out T component) where T : CmsComponent
    {
        var comp = m_Components.Find(comp => comp is T);
        if (comp == null)
        {
            component = null;
            return false;
        }
        component = (T)comp;
        return true;
    }
    
    public SerializedCmsEntity AsSerializedCmsEntity()
    {
        SerializedCmsEntity ent = new();
        ent.id = id;
        foreach (var comp in Components)
        {
            ent.components.Add(comp.AsSerializedCmsComponent());
        }
        return ent;
    }
}

public class CmsComponent
{
    public SerializedCmsComponent AsSerializedCmsComponent()
    {
        var type = GetType();
        SerializedCmsComponent cmsComp = new();
        cmsComp.typeName = type.FullName;
        foreach (var field in type.GetFields())
        {
            cmsComp.fields[field.Name] = Convert.ChangeType(field.GetValue(this), field.FieldType)!;
        }
        return cmsComp;
    }
}

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