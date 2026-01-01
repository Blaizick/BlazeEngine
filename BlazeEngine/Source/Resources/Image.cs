using StbImageSharp;

namespace BlazeEngine.ResourcesManagement;

public class Image
{
    public Vec2Int size = Vec2Int.Zero;
    public byte[] data = new byte[0];

    public Image() {}
    public Image(Vec2Int size, byte[] data)
    {
        this.size = size;
        this.data = data;
    }
    public static Image LoadFromFile(string path, IResourceLoader<Image> loader)
    {
        return loader.LoadFromFile(path);
    }
    public static Image LoadFromFile(string path)
    {
        return new ImageLoader().LoadFromFile(path);
    }
}

public class ImageLoader : IResourceLoader<Image>
{
    public Image LoadFromFile(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        Image image = new();
        using (var stream = File.Open(path, FileMode.Open))
        {
            var stbImage = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            image.size = new Vec2Int(stbImage.Width, stbImage.Height);
            image.data = stbImage.Data;
        }
        return image;
    }
}
