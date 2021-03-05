using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace ApiTarea.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nombre")]
        [JsonProperty("Username")]
        public string Username { get; set; }

        public string img { get; set; }

        public string password { get; set; }
        public DateTime fecha { get; set; }
        public Boolean estado { get; set; }
        public Boolean carritoflag { get; set; }
        public double telefono { get; set; }
        public string cp { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string direccion { get; set; }
        public string AccessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshTokenExpiryTime { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string carrito { get; set; }
    }
}








 
  
