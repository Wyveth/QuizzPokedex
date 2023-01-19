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
        Task<List<Quizz>> GetAllByProfileAsync(int profileId);
        Task<Quizz> GetByIdAsync(int id);
        Task<int> CreateAsync(Quizz quizz);
        Task<int> DeleteAsync(Quizz quizz);
        Task<int> UpdateAsync(Quizz quizz);
        Task<List<Quizz>> GetUnfinishedQuizzByProfile(int profileId);
        Task<Quizz> GenerateQuizz(Profile profile, bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool gen9, bool genArceus, bool easy, bool normal, bool hard);
    }
}
