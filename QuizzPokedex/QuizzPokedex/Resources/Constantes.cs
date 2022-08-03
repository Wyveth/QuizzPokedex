using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Resources
{
    public class Constantes
    {
        #region Evolution Type
        public const string MegaEvolution = "Mega";
        public const string GigaEvolution = "Gigamax";
        public const string NormalEvolution = "Normal";
        public const string Alola = "Alola";
        public const string Galar = "Galar";
        public const string VarianteSexe = "VarianteSexe";
        public const string Variant = "Variant";
        public const string Hisui = "Hisui";
        #endregion

        #region Pokemon Affichage Distinct
        public const string Type_0 = "Type:0";
        public const string Type0 = "Type 0";
        public const string Ningale = "Ningale";
        public const string Ninjask = "Ninjask";
        public const string Munja = "Munja";
        public const string Prismillon = "Prismillon";
        public const string Charpenti = "Charpenti";
        #endregion

        #region Color Hexa
        public const string BlackHexa = "#000000";
        public const string WhiteHexa = "#ffffff";
        #endregion

        #region Type
        public const string Steel = "Acier";
        public const string Fighting = "Combat";
        public const string Dragon = "Dragon";
        public const string Water = "Eau";
        public const string Electric = "Électrik";
        public const string Fairy = "Fée";
        public const string Fire = "Feu";
        public const string Ice = "Glace";
        public const string Bug = "Insecte";
        public const string Normal = "Normal";
        public const string Grass = "Plante";
        public const string Poison = "Poison";
        public const string Psychic = "Psy";
        public const string Rock = "Roche";
        public const string Ground = "Sol";
        public const string Ghost = "Spectre";
        public const string Dark = "Ténèbres";
        public const string Flying = "Vol";
        #endregion

        #region FileExtension
        public const string ExtensionImage = ".png";
        #endregion

        #region Icon Type
        public const string Icon_Steel = "Icon_Steel" + ExtensionImage;
        public const string Icon_Steel_BW = "Icon_Steel_BW" + ExtensionImage;
        public const string Icon_Fighting = "Icon_Fighting" + ExtensionImage;
        public const string Icon_Fighting_BW = "Icon_Fighting_BW" + ExtensionImage;
        public const string Icon_Dragon = "Icon_Dragon" + ExtensionImage;
        public const string Icon_Dragon_BW = "Icon_Dragon_BW" + ExtensionImage;
        public const string Icon_Water = "Icon_Water" + ExtensionImage;
        public const string Icon_Water_BW = "Icon_Water_BW" + ExtensionImage;
        public const string Icon_Electric = "Icon_Electric" + ExtensionImage;
        public const string Icon_Electric_BW = "Icon_Electric_BW" + ExtensionImage;
        public const string Icon_Fairy = "Icon_Fairy" + ExtensionImage;
        public const string Icon_Fairy_BW = "Icon_Fairy_BW" + ExtensionImage;
        public const string Icon_Fire = "Icon_Fire" + ExtensionImage;
        public const string Icon_Fire_BW = "Icon_Fire_BW" + ExtensionImage;
        public const string Icon_Ice = "Icon_Ice" + ExtensionImage;
        public const string Icon_Ice_BW = "Icon_Ice_BW" + ExtensionImage;
        public const string Icon_Bug = "Icon_Bug" + ExtensionImage;
        public const string Icon_Bug_BW = "Icon_Bug_BW" + ExtensionImage;
        public const string Icon_Normal = "Icon_Normal" + ExtensionImage;
        public const string Icon_Normal_BW = "Icon_Normal_BW" + ExtensionImage;
        public const string Icon_Grass = "Icon_Grass" + ExtensionImage;
        public const string Icon_Grass_BW = "Icon_Grass_BW" + ExtensionImage;
        public const string Icon_Poison = "Icon_Poison" + ExtensionImage;
        public const string Icon_Poison_BW = "Icon_Poison_BW" + ExtensionImage;
        public const string Icon_Psychic = "Icon_Psychic" + ExtensionImage;
        public const string Icon_Psychic_BW = "Icon_Psychic_BW" + ExtensionImage;
        public const string Icon_Rock = "Icon_Rock" + ExtensionImage;
        public const string Icon_Rock_BW = "Icon_Rock_BW" + ExtensionImage;
        public const string Icon_Ground = "Icon_Ground" + ExtensionImage;
        public const string Icon_Ground_BW = "Icon_Ground_BW" + ExtensionImage;
        public const string Icon_Ghost = "Icon_Ghost" + ExtensionImage;
        public const string Icon_Ghost_BW = "Icon_Ghost_BW" + ExtensionImage;
        public const string Icon_Dark = "Icon_Dark" + ExtensionImage;
        public const string Icon_Dark_BW = "Icon_Dark_BW" + ExtensionImage;
        public const string Icon_Flying = "Icon_Flying" + ExtensionImage;
        public const string Icon_Flying_BW = "Icon_Flying_BW" + ExtensionImage;
        #endregion

        #region Other Image Assets
        public const string Pokedex_Up = "Pokedex_Up" + ExtensionImage;
        public const string Pokedex_Down = "Pokedex_Down" + ExtensionImage;
        public const string Easy_White = "OneStarWhite_Medium" + ExtensionImage;
        public const string Easy_Color = "OneStarFull_Medium" + ExtensionImage;
        public const string Normal_White = "TwoStarWhite_Medium" + ExtensionImage;
        public const string Normal_Color = "TwoStarFull_Medium" + ExtensionImage;
        public const string Hard_White = "ThreeStarWhite_Medium" + ExtensionImage;
        public const string Hard_Color = "ThreeStarFull_Medium" + ExtensionImage;
        public const string Filter = "Filter" + ExtensionImage;
        public const string Resume = "Resume" + ExtensionImage;
        public const string Love = "Love" + ExtensionImage;
        public const string LoveFull = "Lovefull" + ExtensionImage;
        public const string StarSuccess = "StarSuccess" + ExtensionImage;
        public const string StarWrong = "StarWrong" + ExtensionImage;
        public const string DetectivePikachu = "Detective_Pikachu" + ExtensionImage;
        #endregion

        #region Difficulty
        public const string EasyTQ = "Facile";
        public const string NormalTQ = "Normal";
        public const string HardTQ = "Difficile";
        #endregion

        #region Statistic
        public const string Pv = "Pv";
        public const string Attaque = "Attaque";
        public const string Defense = "Defense";
        public const string AttaqueSpe = "AttaqueSpe";
        public const string DefenseSpe = "DefenseSpe";
        public const string Vitesse = "Vitesse";
        #endregion

        #region Code Type Question
        public const string QTypPok = "Pok"; //OK
        public const string QTypPokBlurred = "PokBlurred"; //OK
        public const string QTypPokBlack = "PokBlack"; //OK
        public const string QTypPokFamily = "PokFamily";
        public const string QTypPokTyp = "PokTyp";
        public const string QTypTypPok = "TypPok"; //OK
        public const string QTypTypPokVarious = "TypPokVarious"; //OK
        public const string QTypWeakPokVarious = "TypWeakPokVarious"; //OK
        public const string QTypTalentPokVarious = "TypTalentPokVarious";
        public const string QTypTyp = "Typ"; //OK
        public const string QTypPokDesc = "PokDesc"; //OK
        public const string QTypPokDescReverse = "PokDescReverse"; //OK
        public const string QTypPokTalent = "PokTalent"; //OK
        public const string QTypPokTalentReverse = "PokTalentReverse";//OK
        public const string QTypPokStat = "PokStat"; //OK
        public const string QTypTalent = "Talent"; //OK
        public const string QTypTalentReverse = "TalentReverse"; //OK
        #endregion

        //Variable pour différencier les tests
        public const bool IsGenerateDB = false;
    }
}
