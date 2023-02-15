using SQLite;

namespace QuizzPokedex.Models
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Nom du Profil
        [Indexed]
        public string Name { get; set; }

        //Date Naissance
        public string BirthDate { get; set; }
        
        //ID Pokemon
        public int PokemonID { get; set; }

        //Actif
        public bool Activated { get; set; }
    }
}
