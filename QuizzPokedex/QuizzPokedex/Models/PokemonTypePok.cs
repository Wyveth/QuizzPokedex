using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class PokemonTypePok
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public int PokemonId { get; set; }

        [Indexed]
        public int TypePokId { get; set; }
    }
}
