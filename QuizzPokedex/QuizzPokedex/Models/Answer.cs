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

        public string libelle { get; set; }

        public bool IsSelected { get; set; }

        public bool IsCorrect { get; set; }
    }
}
