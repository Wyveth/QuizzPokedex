using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuizzPokedex.Models
{
    public class PokemonAttaque
    {
        public int Id { get; set; }

        public int PokemonId { get; set; }

        public int AttaqueId { get; set; }

        public string TypeLearn { get; set; }

        public string Level { get; set; }
        
        public string CTCS { get; set; }
    }
}
