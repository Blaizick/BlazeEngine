using System.Text;
using BlazeEngine;
using BlazeEngine.ResourcesManagement;
using BlazeEngine.Utils;

namespace BlazeEnvironment;

public class ProjectBuildSystem
{
    public ProjectMetaSystem projectMetaSystem;
    public ProjectInfoSystem projectInfoSystem;

    public const string DefaultBuildDirectoryName = "Build";
    
    public ProjectBuildSystem(ProjectMetaSystem projectMetaSystem, ProjectInfoSystem projectInfoSystem)
    {
        this.projectMetaSystem = projectMetaSystem;
        this.projectInfoSystem = projectInfoSystem;
    }

    public void Build()
    {
        string buildDir = Path.Combine(projectInfoSystem.projectInfo.projectRoot, DefaultBuildDirectoryName);
        Directory.CreateDirectory(buildDir);
        
        SystemUtils.System("dotnet", 
            $"build \"{Path.Combine(projectInfoSystem.projectInfo.projectRoot, projectInfoSystem.projectInfo.projectName + ".sln")}\" -c Release -o {buildDir}");
        
        YAML.SerializeToFile(Path.Combine(buildDir, RuntimeData.RuntimeDataFileName), 
            new RuntimeData{assemblyPath = Path.Combine(".", projectInfoSystem.projectInfo.projectName + ".dll")});

        if (!string.IsNullOrEmpty(projectMetaSystem.projectMeta.resourcesRootDirectory))
        {
            FileUtils.CopyDirectory(Path.Combine(projectInfoSystem.projectInfo.projectRoot, projectMetaSystem.projectMeta.resourcesRootDirectory), 
                Path.Combine(buildDir, ResourcesCore.DefaultRootDirectory));    
        }
    }

    public void Clean()
    {
        string buildDir = Path.Combine(projectInfoSystem.projectInfo.projectRoot, DefaultBuildDirectoryName);
        Directory.Delete(buildDir, true);
    }
}

public class ProjectBuildWindow
{
    public ProjectBuildSystem projectBuildSystem;
    
    public bool active = false;

    public ProjectBuildWindow(ProjectBuildSystem projectBuildSystem)
    {
        this.projectBuildSystem = projectBuildSystem;
    }
    
    public void Display()
    {
        if (!active)
        {
            return;
        }

        ImGui.Begin("Build", ref active);
        ImGui.PushId("BuildWindow");

        StringBuilder sb = new(projectBuildSystem.projectMetaSystem.projectMeta.resourcesRootDirectory);
        if (ImGui.InputText("Resources Root Directory", sb))
        {
            projectBuildSystem.projectMetaSystem.projectMeta.resourcesRootDirectory = sb.ToString();
            projectBuildSystem.projectMetaSystem.Save();
        }
        ImGui.SameLine();
        if (ImGui.Button("..."))
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    projectBuildSystem.projectMetaSystem.projectMeta.resourcesRootDirectory = fbd.SelectedPath;
                    projectBuildSystem.projectMetaSystem.Save();
                }
            }
        }
        
        if (ImGui.Button("Build"))
        {
            projectBuildSystem.Build();
        }
        ImGui.SameLine();
        if (ImGui.Button("Clean"))
        {
            projectBuildSystem.Clean();
        }
        
        ImGui.PopId();
        ImGui.End();
    }
}