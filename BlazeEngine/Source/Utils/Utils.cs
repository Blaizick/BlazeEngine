namespace BlazeEngine.Utils;

public static class FileUtils
{
    public static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var filePath in Directory.GetFiles(source))
        {
            var fileName = Path.GetFileName(filePath);
            File.Copy(filePath, Path.Combine(destination, fileName), true);            
        }
        foreach (var dirPath in Directory.GetDirectories(source))
        {
            var dirName = Path.GetFileName(dirPath);
            CopyDirectory(dirPath, Path.Combine(destination, dirName));
        }
    }
}