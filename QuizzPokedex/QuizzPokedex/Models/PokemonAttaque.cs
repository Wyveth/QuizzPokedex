using SQLite;

namespace QuizzPokedex.Models
{
    public class PokemonAttaque
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public int PokemonId { get; set; }

        [Indexed]
        public int AttaqueId { get; set; }

        public string TypeLearn { get; set; }

        public string Level { get; set; }
        
        public string CTCS { get; set; }
    }
}
