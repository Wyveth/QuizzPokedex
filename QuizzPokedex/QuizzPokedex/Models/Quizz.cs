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

        //Savoir si le questionnaire est terminé
        public bool Terminate { get; set; }

        //Identifiant du Profil
        public int ProfileID { get; set; }
    }
}
