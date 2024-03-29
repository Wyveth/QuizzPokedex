﻿using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizzPokedex.Models
{
    public class Pokemon
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Numéro du Pokémon
        [Indexed]
        public string Number { get; set; }

        //Nom du Pokémon
        [Indexed]
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
        public string PathImg { get; set; }

        //Url du Sprite
        public string UrlSprite { get; set; }
        public string PathSprite { get; set; }

        //Url du Son
        public string UrlSound { get; set; }
        public string PathSound { get; set; }

        //Taille du Pokémon
        public string Size { get; set; }

        //Catégorie du Pokémon
        public string Category { get; set; }

        //Poids du Pokémon
        public string Weight { get; set; }

        //Evolution/Famille du Pokémon
        public string Evolutions { get; set; }

        //Type Evolution : Normal, Méga, Gigamax, Alola, Galar, Variant
        public string TypeEvolution { get; set; }

        //Savoir Quand où comment le pokémon évolue
        public string WhenEvolution { get; set; }

        //Statistique PV
        public int StatPv { get; set; }

        //Statistique Attaque
        public int StatAttaque { get; set; }

        //Statistique Défense
        public int StatDefense { get; set; }

        //Statistique Attaque Spéciale
        public int StatAttaqueSpe { get; set; }

        //Statistique Défense Spéciale
        public int StatDefenseSpe { get; set; }

        //Statistique Vitesse
        public int StatVitesse { get; set; }

        //Statistique Total
        public int StatTotal { get; set; }

        //Generation Number
        public int Generation { get; set; }

        //Egg Moves
        public string EggMoves { get; set; }

        //Capture Rate
        public string CaptureRate { get; set; }

        //Basic Happiness
        public string BasicHappiness { get; set; }

        //Game Version
        public string Game { get; set; }

        //Prochain Pokémon
        public string NextUrl { get; set; }

        //Update Bool
        public bool Updated { get; set; }

        //Check Picture Bool
        public bool Check { get; set; }

        //Favorite Profile
        [NotMapped]
        public bool Favorite { get; set; }

        [NotMapped]
        public byte[] ImgFavorite { get; set; }
    }
}
