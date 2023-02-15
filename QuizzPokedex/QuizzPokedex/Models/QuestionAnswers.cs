using System.Collections.Generic;

namespace QuizzPokedex.Models
{
    public class QuestionAnswers
    {
        public Quizz Quizz { get; set; }
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
