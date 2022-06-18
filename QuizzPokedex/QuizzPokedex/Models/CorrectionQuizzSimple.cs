using System;
using System.Collections.Generic;
using System.Text;

namespace QuizzPokedex.Models
{
    public class CorrectionQuizzSimple
    {
        public Question Question { get; set; }
        public QuestionType QuestionType { get; set; }
        public bool IsQTypPok { get; set; }
        public Pokemon Pokemon { get; set; }
        public bool IsQTypTyp { get; set; }
        public TypePok TypePok { get; set; }
        public Answer CorrectAnswer { get; set; }
        public Answer WrongAnswer { get; set; }
        public byte[] ByteTypePok { get; set; }
        public byte[] ByteResult { get; set; }
    }
}
