using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IAnswerService
    {
        Task<List<Answer>> GetAllAsync();
        Task<List<Answer>> GetAllByAnswersIDAsync(string answersID);
        Task<Answer> GetByIdAsync(int id);
        Task<int> CreateAsync(Answer answer);
        Task<int> DeleteAsync(Answer answer);
        Task<int> UpdateAsync(Answer answer);
        Task<string> GenerateAnswers(List<Pokemon> pokemonsAnswer);
    }
}
