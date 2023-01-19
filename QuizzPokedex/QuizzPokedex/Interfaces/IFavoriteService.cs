using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> CheckIfFavoriteExist(Pokemon pokemon);
        Task<Favorite> GetFavorite(Pokemon pokemon);
        Task<List<Pokemon>> GetAllByProfileAsync(string filter, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, bool steel, bool fighting, bool dragon, bool water, bool electric, bool fairy, bool fire, bool ice, bool bug, bool normal, bool grass, bool poison, bool psychic, bool rock, bool ground, bool ghost, bool dark, bool flying, bool descending);
        Task<int> CreateAsync(Favorite favorite);
        Task<int> DeleteAsync(Favorite favorite);
        Task<int> UpdateAsync(Favorite favorite);
    }
}
