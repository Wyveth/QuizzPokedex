using SQLite;

namespace QuizzPokedex.Models
{
    public class Question
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int Id { get; set; }

        //Numéro d'ordre du Quizz
        public int Order { get; set; }

        //Data Object ID
        public int DataObjectID { get; set; }

        //ID des réponses
        public string AnswersID { get; set; }

        //Identifiant Type Question
        public int QuestionTypeID {get;set;}

        //Savoir si la question est terminé
        public bool Done { get; set; }
    }
}
