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

        public string Code { get; set; }

        public string Libelle { get; set; }

        public string Difficulty { get; set; }
    }
}
