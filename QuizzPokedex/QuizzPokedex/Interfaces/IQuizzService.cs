using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IQuizzService
    {
        Task<List<Quizz>> GetQuizzsAsync();
        Task<int> CreateQuizzAsync(Quizz quizz);
        Task<int> DeleteQuizzAsync(Quizz quizz);
        Task<int> UpdateQuizzAsync(Quizz quizz);
    }
}
