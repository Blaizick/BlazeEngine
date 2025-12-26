using System.Reflection;

namespace BlazeEngine;

public class ScriptCore
{
    public IGameCore gameCore;
    
    public void Init()
    {
        var type = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IGameCore).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).First();
        gameCore = (IGameCore)Activator.CreateInstance(type)!;
    }

    public void CallInit()
    {
        gameCore.Init();
    }
    public void CallUpdate()
    {
        gameCore.Update();
    }
    public void CallDraw()
    {
        gameCore.Draw();
    }
    public void CallQuit()
    {
        gameCore.Quit();
    }
    public void CallPreFixedUpdate()
    {
        if(gameCore is IFixedUpdateGameCore core)
            core.PreFixedUpdate();
    }
    public void CallFixedUpdate()
    {
        if(gameCore is IFixedUpdateGameCore core)
            core.FixedUpdate();
    }
    public void CallPostFixedUpdate()
    {
        if(gameCore is IFixedUpdateGameCore core)
            core.PostFixedUpdate();
    }
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