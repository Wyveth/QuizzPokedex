using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class QuestionType
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Code des Type de question (Type/Pokémon)
        public string Code { get; set; }

        //Libellé de la Question
        public string Libelle { get; set; }

        //Nombre de Réponse Disponible
        public int NbAnswers { get; set; }

        //Plusieurs réponse possible
        public bool IsMultipleAnswers { get; set; }

        //Nombre de bonne réponse
        public int NbAnswersPossible { get; set; }

        //Si Image Floue
        public bool IsBlurred { get; set; }

        //Si Image Noir & Blanc
        public bool IsGrayScale { get; set; }

        //Si Réponse de Même Type
        public bool IsSameType { get; set; }

        //ID Difficulté
        public int DifficultyID { get; set; }
    }
}
