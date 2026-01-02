using System.Diagnostics;
using BlazeEngine;
using Debug = BlazeEngine.Debug;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeEnvironment;

public static class SystemUtils
{
    public static void System(string fileName, string arguments)
    {
        ProcessStartInfo psi = new()
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using (var process = Process.Start(psi))
        {
            process?.WaitForExit();
        }
    }
}


public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Debug.Construct(new ConsoleLogger());
        YAML.Init();

        var services = new ServiceCollection();

        services.AddSingleton<ProjectInfoData>();
        services.AddSingleton<ProjectMetaData>();
        
        services.AddSingleton<ProjectInfoSystem>();
        services.AddSingleton<ProjectMetaSystem>();
        
        services.AddSingleton<EnvironmentPrefsSystem>(p =>
        {
            var sys = new EnvironmentPrefsSystem();
            sys.Init();
            return sys;
        });
        services.AddSingleton<EnvironmentPrefsWindow>();

        services.AddSingleton<ProjectCreateSystem>();
        services.AddSingleton<ProjectCreateDialog>();
        
        services.AddSingleton<ProjectOpenSystem>();
        services.AddSingleton<ProjectOpenDialog>();
        
        services.AddSingleton<ProjectBuildSystem>();
        services.AddSingleton<ProjectBuildWindow>();
        
        services.AddSingleton<AssemblySystem>();
        
        services.AddSingleton<CmsEntityWindow>();
        services.AddSingleton<CmsWindow>();
        
        services.AddSingleton<Main>();

        var provider = services.BuildServiceProvider();
        
        Main main = provider.GetRequiredService<Main>();
        
        ImGui.Init();
        while (ImGui.IsRunning())
        {
            ImGui.PreDraw();
            main.Display();
            ImGui.PostDraw();
        }
    }
}

public class Main
{
    public CmsWindow cmsWindow;
    public CmsEntityWindow cmsEntityWindow;
    
    public ProjectCreateDialog projectCreateDialog;
    public ProjectOpenDialog projectOpenDialog;
    
    public ProjectBuildWindow projectBuildWindow;

    public EnvironmentPrefsWindow environmentPrefsWindow;

    public Main(CmsWindow cmsWindow,
        CmsEntityWindow cmsEntityWindow,
        ProjectCreateDialog projectCreateDialog,
        ProjectOpenDialog projectOpenDialog,
        ProjectBuildWindow projectBuildWindow,
        EnvironmentPrefsWindow environmentPrefsWindow)
    {
        this.cmsWindow = cmsWindow;
        this.cmsEntityWindow = cmsEntityWindow;
        
        this.projectCreateDialog = projectCreateDialog;
        this.projectOpenDialog = projectOpenDialog;
        
        this.projectBuildWindow = projectBuildWindow;
        
        this.environmentPrefsWindow = environmentPrefsWindow;
    }
    
    public void Display()
    {
        ImGui.BeginCustomMainDockspace();
        ImGui.End();
        
        cmsWindow.Display();
        cmsEntityWindow.Display();
            
        projectCreateDialog.Display();
        environmentPrefsWindow.Display();
        projectBuildWindow.Display();
            
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New Project"))
                {
                    projectCreateDialog.active = true;
                }
                if (ImGui.MenuItem("Open Project"))
                {
                    projectOpenDialog.Show();
                }
                if (ImGui.MenuItem("Settings"))
                {
                    environmentPrefsWindow.active = true;
                }
                if (ImGui.MenuItem("Build"))
                {
                    projectBuildWindow.active = true;
                }
                 
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.Checkbox("Cms Window", ref cmsWindow.active))
                {
                    
                }
                if (ImGui.Checkbox("Cms Entity Window", ref cmsEntityWindow.active))
                {
                    
                }
                
                ImGui.EndMenu();
            }
                
            ImGui.EndMainMenuBar();
        }
    }
}