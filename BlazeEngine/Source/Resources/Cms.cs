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
}

public class CmsComponent
{
    
}