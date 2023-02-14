using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class TypeAttaque
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string UrlImg { get; set; }
        
        public string PathImg { get; set; }
    }
}
