using SQLite;

namespace QuizzPokedex.Models
{
    public class TypePok
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Nom Type
        [Indexed]
        public string Name { get; set; }

        //Url Miniature Pokemon Go
        public string UrlMiniGo { get; set; }
        public string PathMiniGo { get; set; }

        //Url Fond Pokemon Go
        public string UrlFondGo { get; set; }
        public string PathFondGo { get; set; }

        //Url Miniature Pokemon Home
        public string UrlMiniHome { get; set; }
        public string PathMiniHome { get; set; }

        //Url Icone Pokemon Home
        public string UrlIconHome { get; set; }
        public string PathIconHome { get; set; }

        //Url Autocollant Pokemon Home
        public string UrlAutoHome { get; set; }
        public string PathAutoHome { get; set; }

        //Couleur de l'image du fond
        public string ImgColor { get; set; }

        //Couleur du Type
        public string InfoColor { get; set; }

        //Couleur Background Type
        public string TypeColor { get; set; }

        //Check Picture Bool
        public bool Check { get; set; }
    }
}
