using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuizzPokedex.Models
{
    public class Pokemon
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //Numéro du Pokémon
        [Indexed]
        public string Number { get; set; }

        //Nom du Pokémon
        public string Name { get; set; }

        [Indexed]
        //Nom Affiché
        public string DisplayName { get; set; }

        //Description du Pokémon Version X
        public string DescriptionVx { get; set; }

        //Description du Pokémon Version
        public string DescriptionVy { get; set; }

        //Url de l'Image
        public string UrlImg { get; set; }
        public byte[] DataImg { get; set; }

        //Url du Sprite
        public string UrlSprite { get; set; }
        public byte[] DataSprite { get; set; }

        //Taille du Pokémon
        public string Size { get; set; }

        //Catégorie du Pokémon
        public string Category { get; set; }

        //Poids du Pokémon
        public string Weight { get; set; }

        //Talent du Pokémon
        public string Talent { get; set; }

        //Description du Talent
        public string DescriptionTalent { get; set; }

        //Nom des Types
        public string Types { get; set; }

        //Nom des Faiblesses
        public string Weakness { get; set; }

        //Evolution/Famille du Pokémon
        public string Evolutions { get; set; }

        //Type Evolution : Normal, Méga, Gigamax, Alola, Galar, Variant
        public string TypeEvolution { get; set; }

        //Generation Number
        public int Generation { get; set; }

        //Prochain Pokémon
        public string NextUrl { get; set; }

        //Update Bool
        public bool Updated { get; set; }
    }
}
