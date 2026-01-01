using BlazeEngine;

namespace BlazeEnvironment;

public class ProjectInfoData
{
    public const string DefaultFileName = "ProjectInfoData.yaml";
    
    public string projectRoot = string.Empty;
    public string projectName = string.Empty;
}
public class ProjectInfoSystem
{
    public ProjectInfoData projectInfo;

    public ProjectInfoSystem(ProjectInfoData projectInfo)
    {
        this.projectInfo = projectInfo;
    }

    public void Save()
    {
        var path = Path.Combine(projectInfo.projectRoot, ProjectInfoData.DefaultFileName);
        YAML.SerializeToFile(path, projectInfo);
    }

    public void Load(string projectRoot)
    {
        var path = Path.Combine(projectRoot, ProjectInfoData.DefaultFileName);
        projectInfo = YAML.DeserializeFromFile<ProjectInfoData>(path);
    }
}


public class FileMetaData
{
    public string path = string.Empty;
    
    public FileMetaData() {}

    public FileMetaData(string path)
    {
        this.path = path; 
    }
}

public class ProjectMetaData
{
    public const string DefaultFileName = "ProjectMetaData.yaml";

    public string resourcesRootDirectory = string.Empty;
    public List<FileMetaData> files = new();
}

public class ProjectMetaSystem
{
    public ProjectMetaData projectMeta;
    public ProjectInfoSystem projectInfoSystem;

    public ProjectMetaSystem(ProjectMetaData projectMeta, ProjectInfoSystem projectInfoSystem)
    {
        this.projectMeta = projectMeta;
        this.projectInfoSystem = projectInfoSystem;
    }

    public void Save()
    {
        var path = Path.Combine(projectInfoSystem.projectInfo.projectRoot, ProjectMetaData.DefaultFileName);
        YAML.SerializeToFile(path, projectMeta);
    }
    public void Load()
    {
        var path = Path.Combine(projectInfoSystem.projectInfo.projectRoot, ProjectMetaData.DefaultFileName);
        projectMeta = YAML.DeserializeFromFile<ProjectMetaData>(path);
    }
}