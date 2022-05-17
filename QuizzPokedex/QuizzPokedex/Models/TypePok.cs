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

        //Url Miniature Pokemon Go
        public string UrlMiniGo { get; set; }
        public byte[] DataMiniGo { get; set; }

        //Url Fond Pokemon Go
        public string UrlFondGo { get; set; }
        public byte[] DataFondGo { get; set; }

        //Url Miniature Pokemon Home
        public string UrlMiniHome { get; set; }
        public byte[] DataMiniHome { get; set; }

        //Url Icone Pokemon Home
        public string UrlIconHome { get; set; }
        public byte[] DataIconHome { get; set; }

        //Url Autocollant Pokemon Home
        public string UrlAutoHome { get; set; }
        public byte[] DataAutoHome { get; set; }

        //Couleur de l'image du fond
        public string ImgColor { get; set; }

        //Couleur du Type
        public string InfoColor { get; set; }

        //Couleur Background Type
        public string TypeColor { get; set; }
    }
}
