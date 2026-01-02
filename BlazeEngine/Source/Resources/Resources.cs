using StbImageSharp;

namespace BlazeEngine.ResourcesManagement;

public interface IResourceLoader<T>
{
    public T LoadFromFile(string path);
}

public class ResourcesCore
{
    public const string DefaultRootDirectory = "Resources";
    
    public Dictionary<Type, object> resourceLoadersDic;

    public string resourcesRootDirectory = Path.Combine(".", DefaultRootDirectory);
    
    public ResourcesCore()
    {
        resourceLoadersDic = new Dictionary<Type, object>()
        {
            {typeof(Image), new ImageLoader()},
        };
    }

    public T Load<T>(string _path)
    {
        string path = Path.Combine(resourcesRootDirectory, _path);
        return resourceLoadersDic.TryGetValue(typeof(T), out var loader) ? ((IResourceLoader<T>)loader).LoadFromFile(path) : default!;
    }
}

public static class Resources
{
    public static ResourcesCore resourcesCore;

    public static void Construct(ResourcesCore resourcesCore)
    {
        Resources.resourcesCore = resourcesCore;
    }
    public static T? Load<T>(string path)
    {
        return resourcesCore.Load<T>(path);
    }
}