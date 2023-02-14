using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models.ClassJson
{
    public class TypeAttaqueJson
    {
        public string Name { get; set; }
        public string Description { get; set; }

        //Url de l'Image
        public string UrlImg { get; set; }
        
        //Url de l'Image Interne
        public string PathImg { get; set; }
    }
}
