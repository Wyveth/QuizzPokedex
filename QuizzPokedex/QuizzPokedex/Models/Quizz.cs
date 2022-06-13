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

        //Si Filtre Gen 1 Active
        public bool Gen1 { get; set; }

        //Si Filtre Gen 2 Active
        public bool Gen2 { get; set; }

        //Si Filtre Gen 3 Active
        public bool Gen3 { get; set; }

        //Si Filtre Gen 4 Active
        public bool Gen4 { get; set; }

        //Si Filtre Gen 5 Active
        public bool Gen5 { get; set; }

        //Si Filtre Gen 6 Active
        public bool Gen6 { get; set; }

        //Si Filtre Gen 7 Active
        public bool Gen7 { get; set; }

        //Si Filtre Gen 8 Active
        public bool Gen8 { get; set; }

        //Si Filtre Gen Arceus Active
        public bool GenArceus { get; set; }

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
