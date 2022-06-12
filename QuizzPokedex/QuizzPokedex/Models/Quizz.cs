using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class Quizz
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Identifiant des questions
        public string QuestionsID { get; set; }

        //Si Question Facile
        public bool Easy { get; set; }

        //Si Question Normal
        public bool Normal { get; set; }

        //Si Question Hard
        public bool Hard { get; set; }

        //Savoir si le questionnaire est terminé
        public bool Done { get; set; }

        //Identifiant du Profil
        public int ProfileID { get; set; }
    }
}
