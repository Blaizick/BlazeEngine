namespace BlazeEnvironment;

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