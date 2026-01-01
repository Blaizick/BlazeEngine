using BlazeEngine;
using SDL;
using System.Text;

namespace BlazeEnvironment;

public class EnvironmentPrefsData
{
    public const string DefaultFileName = "EnvironmentPrefs.yaml";

    public string engineCsProjPath = string.Empty;
    public string engineBootstrapCsProjPath = string.Empty;
}

public class EnvironmentPrefsSystem
{
    public string directory;
    public string path;

    public EnvironmentPrefsData data;
    
    public void Init()
    {
        directory = SDL3.SDL_GetPrefPath("Blaizi", "BlazeEngine");
        if (directory != null)
        {
            path = Path.Combine(directory, EnvironmentPrefsData.DefaultFileName);
        }
        Load();
        if (data == null)
        {
            data = new();
        }
    }
    
    public void Save()
    {
        if (path != null)
        {
            Directory.CreateDirectory(directory);
            YAML.SerializeToFile(path, data);
        }
    }
    public void Load()
    {
        data = null!;
        if (path != null && File.Exists(path))
        {
            data = YAML.DeserializeFromFile<EnvironmentPrefsData>(path);
        }
    }
}

public class EnvironmentPrefsWindow
{
    public bool active = false;

    public EnvironmentPrefsSystem environmentPrefsSystem;

    public EnvironmentPrefsWindow(EnvironmentPrefsSystem environmentPrefsSystem)
    {
        this.environmentPrefsSystem = environmentPrefsSystem;
    }
    
    public void Display()
    {
        if (!active)
        {
            return;
        }
        
        ImGui.Begin("Settings", ref active);

        ImGui.PushId("EngineCsProjPath");
        StringBuilder sb = new(environmentPrefsSystem.data.engineCsProjPath);
        if (ImGui.InputText("Engine CsProj Path", sb))
        {
            environmentPrefsSystem.data.engineCsProjPath = sb.ToString();
            environmentPrefsSystem.Save();
        }
        ImGui.SameLine();
        if (ImGui.Button("..."))
        {
            using (OpenFileDialog fbd = new OpenFileDialog())
            {
                fbd.Title = "Open Engine CsProj Path";
                fbd.Filter = "CsProj Files(*.csproj)|*.csproj";
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.FileName))
                {
                    environmentPrefsSystem.data.engineCsProjPath = fbd.FileName;
                    environmentPrefsSystem.Save();
                }
            }            
        }
        ImGui.PopId();

        ImGui.PushId("EngineBootstrapCsProjPath");
        sb = new(environmentPrefsSystem.data.engineBootstrapCsProjPath);
        if (ImGui.InputText("Engine Bootstrap CsProj Path", sb))
        {
            environmentPrefsSystem.data.engineBootstrapCsProjPath = sb.ToString();            
            environmentPrefsSystem.Save();
        }
        ImGui.SameLine();
        if (ImGui.Button("..."))
        {
            using (OpenFileDialog fbd = new OpenFileDialog())
            {
                fbd.Title = "Open Engine Bootstrap CsProj Path";
                fbd.Filter = "CsProj Files(*.csproj)|*.csproj";
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.FileName))
                {
                    environmentPrefsSystem.data.engineBootstrapCsProjPath = fbd.FileName;
                    environmentPrefsSystem.Save();
                }
            }            
        }
        ImGui.PopId();
        
        ImGui.End();
    }
}
