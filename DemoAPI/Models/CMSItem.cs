using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace DemoAPI.Models
{
    public class CMSItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
    }
}
