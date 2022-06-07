using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class QuestionAnswers
    {
        public Quizz Quizz { get; set; }
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
