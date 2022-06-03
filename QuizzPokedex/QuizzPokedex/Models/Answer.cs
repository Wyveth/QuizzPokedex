using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class Answer
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Numéro d'ordre Des Réponses
        public int Order { get; set; }

        //Libellé des réponses
        public string libelle { get; set; }

        //Si la réponse est sélectionnée
        public bool IsSelected { get; set; }

        //Si réponse correcte
        public bool IsCorrect { get; set; }
    }
}
