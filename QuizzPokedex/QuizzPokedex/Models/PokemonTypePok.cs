using SQLite;

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
