using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DemoAPI.Entities
{
    public class Collection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
    }
}
