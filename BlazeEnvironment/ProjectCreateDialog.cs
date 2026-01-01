using System.Text;
using BlazeEngine;

namespace BlazeEnvironment;

public class ProjectCreateSystem
{
    public string projectsRootDir;
    public string projectName;
    
    public EnvironmentPrefsSystem environmentPrefsSystem;
    
    public ProjectInfoSystem projectInfoSystem;
    public ProjectMetaSystem projectMetaSystem;

    public ProjectCreateSystem(EnvironmentPrefsSystem environmentPrefsSystem, ProjectInfoSystem projectInfoSystem, ProjectMetaSystem projectMetaSystem)
    {
        this.projectInfoSystem = projectInfoSystem;
        this.projectMetaSystem = projectMetaSystem;
        this.environmentPrefsSystem = environmentPrefsSystem;
    }

    public bool Create()
    {
        string projectRootDir = Path.Combine(projectsRootDir, projectName);
        if (Directory.Exists(projectRootDir))
            return false;
        projectInfoSystem.projectInfo = new()
        {
            projectName = projectName,
            projectRoot = projectRootDir,
        };
        projectMetaSystem.projectMeta = new();

        var info = projectInfoSystem.projectInfo;
        
        Directory.CreateDirectory(projectRootDir);
        var projectDir = Path.Combine(projectRootDir, info.projectName);
        Directory.CreateDirectory(projectDir);
        
        YAML.SerializeToFile(Path.Combine(projectRootDir, ProjectInfoData.DefaultFileName), info);
        
        SystemUtils.System("dotnet", $"new solution --output \"{info.projectRoot}\" --name \"{info.projectName}\"");
        SystemUtils.System("dotnet", $"new classlib --output \"{projectDir}\" --name \"{info.projectName}\"");

        string csprojPath = Path.Combine(projectDir, info.projectName + ".csproj");
        
        SystemUtils.System("dotnet", $"solution \"{info.projectRoot}\" add \"{csprojPath}\"");
        SystemUtils.System("dotnet", $"add \"{csprojPath}\" reference \"{environmentPrefsSystem.data.engineCsProjPath}\"");
        SystemUtils.System("dotnet", $"add \"{csprojPath}\" reference \"{environmentPrefsSystem.data.engineBootstrapCsProjPath}\"");
        
        projectInfoSystem.Save();
        projectMetaSystem.Save();
        
        return true;
    }
}

public class ProjectCreateDialog
{
    public bool active = false;

    public ProjectCreateSystem projectCreateSystem;

    public ProjectCreateDialog(ProjectCreateSystem projectCreateSystem)
    {
        this.projectCreateSystem = projectCreateSystem;
    }
    
    public void Display()
    {
        if (!active)
        {
            return;
        }

        ImGui.Begin("New Project", ref active);

        ImGui.PushId("ProjectCreateDialog");
        StringBuilder sb = new(projectCreateSystem.projectsRootDir);
        if (ImGui.InputText("Projects Root Directory", sb))
        {
            projectCreateSystem.projectsRootDir = sb.ToString();
        }
        ImGui.SameLine();
        if (ImGui.Button("..."))
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    projectCreateSystem.projectsRootDir = fbd.SelectedPath;
                }
            }
        }
        ImGui.PopId();
        
        sb = new(projectCreateSystem.projectName);
        if (ImGui.InputText("Project Name", sb))
        {
            projectCreateSystem.projectName = sb.ToString();
        }
        
        if (ImGui.Button("Create"))
        {
            if (projectCreateSystem.Create())
            {
                active = false;
            }
        }
        
        ImGui.End();
    }
}