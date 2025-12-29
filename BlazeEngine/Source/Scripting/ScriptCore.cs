using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BlazeEngine;

public class ScriptCore
{
    public IGameCore gameCore;

    public void Init()
    {
        var runtimeData = YAML.DeserializeFromFile<RuntimeData>(RuntimeData.RuntimeDataPath);
        if (!File.Exists(runtimeData.assemblyPath))
        {
            Debug.LogError($"Game assembly not found: {runtimeData.assemblyPath}");
            return;
        }
        var asm = Assembly.LoadFrom(runtimeData.assemblyPath);
        var types = asm.GetTypes()
            .Where(t => typeof(IGameCore).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        if (types.Count() <= 0)
        {
            Debug.LogError("No IGameCore found!");
            return;
        }
        gameCore = (IGameCore)Activator.CreateInstance(types.First());
    }

    public void CallInit()
    {
        if (gameCore != null)
            gameCore.Init();
    }
    public void CallUpdate()
    {
        if (gameCore != null)
            gameCore.Update();
    }
    public void CallDraw()
    {
        if (gameCore != null)
            gameCore.Draw();
    }
    public void CallQuit()
    {
        if (gameCore != null)
            gameCore.Quit();
    }
    public void CallPreFixedUpdate()
    {
        if(gameCore != null && gameCore is IFixedUpdateGameCore core)
            core.PreFixedUpdate();
    }
    public void CallFixedUpdate()
    {
        if(gameCore != null && gameCore is IFixedUpdateGameCore core)
            core.FixedUpdate();
    }
    public void CallPostFixedUpdate()
    {
        if(gameCore != null && gameCore is IFixedUpdateGameCore core)
            core.PostFixedUpdate();
    }
}

public class RuntimeData
{
    public const string RuntimeDataPath = "./RuntimeData.yaml";
    
    public string assemblyPath;
}

public interface IGameCore
{
    public void Init();
    public void Update();
    public void Draw();
    public void Quit();
}
public interface IFixedUpdateGameCore
{
    public void PreFixedUpdate();
    public void FixedUpdate();
    public void PostFixedUpdate();
}