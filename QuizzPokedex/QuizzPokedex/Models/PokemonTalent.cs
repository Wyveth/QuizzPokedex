using SQLite;

namespace QuizzPokedex.Models
{
    public class PokemonTalent
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public int PokemonId { get; set; }

        [Indexed]
        public int TalentId { get; set; }

        public bool isHidden { get; set; }
    }
}
