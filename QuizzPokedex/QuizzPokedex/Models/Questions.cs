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

        public int Order { get; set; }

        public Pokemon Pokemon { get; set; }

        public int QuestionTypeID {get;set;}
    }
}
