using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class Questions
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Numéro d'ordre du Quizz
        public int Order { get; set; }

        public int PokemonID { get; set; }

        //Identifiant Type Question
        public int QuestionTypeID {get;set;}
    }
}
