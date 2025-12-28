using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BlazeEngine;

public static class YAML
{
    public static ISerializer serializer;
    public static IDeserializer deserializer;

    public static void Init()
    {
        var namingConvention = new PascalCaseNamingConvention();
        serializer = new SerializerBuilder().WithNamingConvention(namingConvention).Build();
        deserializer = new DeserializerBuilder().WithNamingConvention(namingConvention).Build();
    }

    public static string Serialize<T>(in T obj)
    {
        return serializer.Serialize(obj);
    }
    public static void SerializeToFile<T>(string path, in T obj)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, Serialize(obj));
    }
    
    public static T DeserializeFromFile<T>(string path)
    {
        return Deserialize<T>(File.ReadAllText(path));
    }
    public static T Deserialize<T>(string source)
    {
        return deserializer.Deserialize<T>(source);        
    }
}