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
        Task<string> GenerateCorrectAnswers(QuestionType questionType, List<Pokemon> pokemonsAnswer);
        Task<string> GenerateCorrectAnswers(QuestionType questionType, List<TypePok> typesAnswer);
        Task<List<Answer>> GenerateAnswers(Quizz quizz, QuestionType questionType, List<Answer> answers);
        Task<string> GenerateCorrectAnswersDesc(QuestionType questionType, List<Pokemon> pokemonsAnswer);
        Task<string> ConvertDescription(Pokemon pokemon);
    }
}
