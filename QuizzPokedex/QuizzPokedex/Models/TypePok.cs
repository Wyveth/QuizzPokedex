using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class TypePok
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //Nom Type
        public string Name { get; set; }

        //Url Img
        public string UrlImg { get; set; }
    }
}
