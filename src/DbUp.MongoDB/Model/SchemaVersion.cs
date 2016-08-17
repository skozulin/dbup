using System;
using MongoDB.Bson.Serialization.Attributes;

namespace DbUp.MongoDB.Model
{
    public class SchemaVersion
    {
        [BsonId]
        public int SchemaVersionId { get; set; }
        public string ScriptName { get; set; }
        public DateTime Applied { get; set; }
    }
}
