using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models.ClassJson
{
    public class AttaqueJson
    {
        //Nom de l'attaque
        public string Name { get; set; }

        //Description de l'attaque
        public string Description { get; set; }

        //Puissance de l'attaque
        public string Power { get; set; }

        //Précision de l'attaque
        public string Precision { get; set; }

        //PP de l'attaque
        public string PP { get; set; }

        //Catégorie de l'attaque
        public string TypeAttaque { get; set; }

        //Type de l'attaque
        public string TypePok { get; set; }
    }
}
