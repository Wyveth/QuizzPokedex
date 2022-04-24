using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //Nom du Profil
        public string Name { get; set; }

        //Date Naissance
        public string BirthDate { get; set; }
        
        //Photo
        public byte[] Picture { get; set; }
    }
}
