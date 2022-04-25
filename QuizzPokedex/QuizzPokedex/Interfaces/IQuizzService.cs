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
        Task<int> CreateAsync(Quizz quizz);
        Task<int> DeleteAsync(Quizz quizz);
        Task<int> UpdateAsync(Quizz quizz);
    }
}
