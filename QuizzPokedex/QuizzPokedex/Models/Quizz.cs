using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class Quizz
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
