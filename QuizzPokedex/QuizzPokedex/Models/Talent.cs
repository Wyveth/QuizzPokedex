using SQLite;

namespace QuizzPokedex.Models
{
    public class Talent
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Nom Talent
        [Indexed]
        public string Name { get; set; }

        //Description Talent
        public string Description { get; set; }
    }
}
