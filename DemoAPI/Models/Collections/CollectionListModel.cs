using MongoDB.Bson.Serialization.Attributes;

namespace DemoAPI.Models.Collections
{
    public class CollectionListModel
    {
            [BsonId]
            public string Title { get; set; }
            public string Description { get; set; }
            public string Image { get; set; }
            public string Link { get; set; }
       
    }
}
