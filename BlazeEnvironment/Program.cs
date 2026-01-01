using System.Diagnostics;
using System.Text;
using BlazeEngine;
using System.Windows.Forms;
using BlazeEngine.ResourcesManagement;
using BlazeEngine.Utils;
using SDL;
using VelcroPhysics.Dynamics.Joints;
using Debug = BlazeEngine.Debug;

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

public class ProjectOpenSystem
{
    public ProjectInfoSystem projectInfoSystem;
    public ProjectMetaSystem projectMetaSystem;

    public string projectRootDirPath;

    public ProjectOpenSystem(ProjectInfoSystem projectInfoSystem, ProjectMetaSystem projectMetaSystem)
    {
        this.projectInfoSystem = projectInfoSystem;
        this.projectMetaSystem = projectMetaSystem;
    }
    
    public void Open()
    {
        projectInfoSystem.Load(projectRootDirPath);
        projectMetaSystem.Load();
    }
}

public class ProjectOpenDialog
{
    public ProjectOpenSystem projectOpenSystem;

    public ProjectOpenDialog(ProjectOpenSystem projectOpenSystem)
    {
        this.projectOpenSystem = projectOpenSystem;
    }
    
    public void Show()
    {
        using (FolderBrowserDialog fbd = new())
        {
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
            {
                projectOpenSystem.projectRootDirPath = fbd.SelectedPath;
                projectOpenSystem.Open();
            }
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

        ProjectInfoData projectInfoData = new();
        ProjectInfoSystem projectInfoSystem = new(projectInfoData);

        ProjectMetaData projectMetaData = new();
        ProjectMetaSystem projectMetaSystem = new(projectMetaData, projectInfoSystem);
        
        EnvironmentPrefsSystem environmentPrefsSystem = new();
        environmentPrefsSystem.Init();
        EnvironmentPrefsWindow environmentPrefsWindow = new(environmentPrefsSystem);
        
        ProjectCreateSystem projectCreateSystem = new(environmentPrefsSystem, projectInfoSystem, projectMetaSystem);
        ProjectCreateDialog projectCreateDialog = new(projectCreateSystem);

        ProjectOpenSystem projectOpenSystem = new(projectInfoSystem, projectMetaSystem);
        ProjectOpenDialog projectOpenDialog = new(projectOpenSystem);

        ProjectBuildSystem projectBuildSystem = new(projectMetaSystem, projectInfoSystem);
        ProjectBuildWindow projectBuildWindow = new(projectBuildSystem);
        
        ImGui.Init();
        while (ImGui.IsRunning())
        {
            ImGui.PreDraw();

            ImGui.BeginCustomMainDockspace();
            ImGui.End();
            
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
                
                ImGui.EndMainMenuBar();
            }

            ImGui.PostDraw();
        }
    }
}
