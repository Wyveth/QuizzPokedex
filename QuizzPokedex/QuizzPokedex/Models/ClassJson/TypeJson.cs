using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models.ClassJson
{
    [Serializable]
    public class TypeJson
    {
        //Nom Type
        public string Name = "";

        //Url Miniature Pokemon Go
        public string UrlMiniGo = "";

        //Url Fond Pokemon Go
        public string UrlFondGo = "";

        //Url Miniature Pokemon Home
        public string UrlMiniHome = "";

        //Url Icone Pokemon Home
        public string UrlIconHome = "";

        //Url Autocollant Pokemon Home
        public string UrlAutoHome = "";

        //Couleur de l'image du fond
        public string imgColor = "";

        //Couleur du Type
        public string infoColor = "";

        //Couleur Background Type
        public string typeColor = "";
    }
}
