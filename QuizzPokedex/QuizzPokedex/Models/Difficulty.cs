using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public  class Difficulty
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Libellé de la Difficulté
        public string Libelle { get; set; }
    }
}
