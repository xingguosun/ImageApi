using ImagesApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace ImagesApi.Services
{
    public class ImageService
    {
        private readonly IMongoCollection<Image> _images;

        public ImageService(IImagesDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _images = database.GetCollection<Image>(settings.ImagesCollectionName);
        }

        public List<Image> Get() =>
            _images.Find(image => true).ToList();

        public Image Get(string id) =>
            _images.Find<Image>(image => image.Id == id).FirstOrDefault();

        public Image Create(Image image)
        {
            _images.InsertOne(image);
            return image;
        }

        public void Update(string id, Image imageIn) =>
            _images.ReplaceOne(image => image.Id == id, imageIn);

        public void Remove(Image imageIn) =>
            _images.DeleteOne(image => image.Id == imageIn.Id);

        public void Remove(string id) => 
            _images.DeleteOne(image => image.Id == id);
    }
}