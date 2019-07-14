namespace ImagesApi.Models
{
    public class ImagesDatabaseSettings : IImagesDatabaseSettings
    {
        public string ImagesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IImagesDatabaseSettings
    {
        string ImagesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}