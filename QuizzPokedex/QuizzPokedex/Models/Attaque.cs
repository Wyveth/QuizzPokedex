using SQLite;

namespace QuizzPokedex.Models
{
    public class Attaque
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        [Indexed]
        public string Name { get; set; }

        public string Description { get; set; }
        
        public string Power { get; set; }
        
        public string Precision { get; set; }
        
        public string PP { get; set; }
        
        public int TypeAttaqueId { get; set; }
        
        public int TypePokId { get; set; }
    }
}
