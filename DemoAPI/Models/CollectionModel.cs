using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DemoAPI.Models
{
    public class CollectionModel
    {
        [BsonId]

        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
    }
}
