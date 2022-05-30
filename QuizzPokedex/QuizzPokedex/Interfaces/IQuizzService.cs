using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IQuizzService
    {
        Task<List<Quizz>> GetAllAsync();
        Task<List<Quizz>> GetAllByProfileAsync(string profileId);
        Task<Quizz> GetByIdAsync(string identifiant);
        Task<int> CreateAsync(Quizz quizz);
        Task<int> DeleteAsync(Quizz quizz);
        Task<int> UpdateAsync(Quizz quizz);
    }
}
