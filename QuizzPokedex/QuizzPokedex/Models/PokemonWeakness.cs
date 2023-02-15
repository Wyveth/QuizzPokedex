using SQLite;

namespace QuizzPokedex.Models
{
    public class PokemonWeakness
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public int PokemonId { get; set; }

        [Indexed]
        public int TypePokId { get; set; }
    }
}
