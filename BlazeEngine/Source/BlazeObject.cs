namespace BlazeEngine;

public class BlazeObject
{
    protected List<object> m_Components = new();

    public T? GetComponent<T>()
    {
        return m_Components.OfType<T>().FirstOrDefault();
    }

    public void AddComponent<T>(in T component)
    {
        if (component != null)
            m_Components.Add(component);
    }
    public void AddComponent<T>()
    {
        var comp = Activator.CreateInstance<T>();
        if (comp == null)
        {
            Debug.LogError($"Failed to create instance of {typeof(T).Name}");
            return;
        }
        m_Components.Add(comp);
    }
}