namespace QuizzPokedex.Models
{
    public class QuizzDifficulty
    {
        public Quizz Quizz { get; set; }

        public byte[] ImgEasy { get; set; }

        public byte[] ImgNormal { get; set; }

        public byte[] ImgHard { get; set; }

        public byte[] ImgResume { get; set; }

        public string ResumeQuestion { get; set; }
    }
}
