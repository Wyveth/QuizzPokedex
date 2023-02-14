using QuizzPokedex.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.Interfaces
{
    public interface IQuestionTypeService
    {
        Task Populate();
        Task<List<QuestionType>> GetAllAsync();
        Task<int> GetAllCountAsync();
        Task<QuestionType> GetByIdAsync(int id);
        Task<int> CreateAsync(QuestionType questionType);
        Task<int> DeleteAsync(QuestionType questionType);
        Task<int> UpdateAsync(QuestionType questionType);
        Task<QuestionType> GetQuestionTypeRandom(bool easy, bool normal, bool hard);
    }
}
