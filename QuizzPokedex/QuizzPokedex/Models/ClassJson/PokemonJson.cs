using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    [Serializable]
    public class PokemonJson
    {
        //Numéro du Pokémon
        public string Number { get; set; }

        //Nom du Pokémon
        public string Name { get; set; }

        //Nom Affiché
        public string DisplayName { get; set; }

        //NameFile
        public string NameEN { get; set; }

        //Description du Pokémon Version X
        public string DescriptionVx { get; set; }

        //Description du Pokémon Version
        public string DescriptionVy { get; set; }

        //Url de l'Image
        public string UrlImg { get; set; }

        //Url du Sprite
        public string UrlSprite { get; set; }

        //Url du Son
        public string UrlSound { get; set; }

        //Taille du Pokémon
        public string Size { get; set; }

        //Catégorie du Pokémon
        public string Category { get; set; }

        //Poids du Pokémon
        public string Weight { get; set; }

        //Talents du Pokémon
        public List<SkillJson> Talents { get; set; }

        //Nom des Types
        public List<TypePokJson> Types { get; set; }

        //Nom des Faiblesses
        public List<TypePokJson> Weakness { get; set; }

        //Attaque
        public List<AttackJson> Attaques { get; set; }

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

        //Generation Game
        public string Game { get; set; }

        //Prochain Pokémon
        public string NextUrl { get; set; }
    }

    public class TypePokJson
    {
        public string Name { get; set; }
    }

    public class SkillJson
    {
        public string Name { get; set; }
        public bool isHidden { get; set; }
    }

    public class AttackJson
    {
        public string Name { get; set; }
        public string TypeLearn { get; set; }
        public string Level { get; set; }
        public string CTCS { get; set; }
    }
}
