using QuizzPokedex.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAllAsync();
        Task<List<Question>> GetAllByQuestionsIDAsync(string questionsID);
        Task<Question> GetByIdAsync(int id);
        Task<int> GetCountAsync();
        Task<int> CreateAsync(Question question);
        Task<int> DeleteAsync(Question question);
        Task<int> UpdateAsync(Question question);
        Task<string> GenerateQuestions(bool gen1, bool gen2, bool gen3, bool gen4, bool gen5, bool gen6, bool gen7, bool gen8, bool genArceus, bool easy, bool normal, bool hard);
        Task<int> GetNbQuestionByDifficulty(bool easy, bool normal, bool hard);
    }
}
