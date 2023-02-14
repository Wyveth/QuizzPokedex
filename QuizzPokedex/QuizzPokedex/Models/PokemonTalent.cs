using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class PokemonTalent
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public int IdPokemon { get; set; }

        [Indexed]
        public int IdTalent { get; set; }

        public bool isHidden { get; set; }
    }
}
