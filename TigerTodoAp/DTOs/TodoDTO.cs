using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TigerTodoAp.DTOs
{
    public class TodoDTO 
    {
        [BsonId]
        public string id { get; set; }
        public bool isSelected { get; set; }
        public string todoName { get; set; }
        public string todoNote { get; set; }
        public string created { get; set; }
        public string lastModified { get; set; }
    }
}