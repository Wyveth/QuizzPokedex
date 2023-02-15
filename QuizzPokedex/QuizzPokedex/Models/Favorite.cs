using SQLite;

namespace QuizzPokedex.Models
{
    public class Favorite
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }
        public int PokemonID { get; set; }
        public int ProfileID { get; set; }
    }
}
