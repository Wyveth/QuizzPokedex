﻿using SQLite;

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

        //Si Image Noir et Blanc
        public bool IsGrayscale { get; set; }

        //Si Image Noir
        public bool IsHide { get; set; }

        //Si Réponse de Même Type
        public bool IsSameType { get; set; }

        //ID Difficulté
        public int DifficultyID { get; set; }
    }
}
