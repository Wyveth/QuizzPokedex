namespace QuizzPokedex.Models
{
    public class CorrectionQuizzSimple
    {
        public Question Question { get; set; }
        public QuestionType QuestionType { get; set; }
        public Pokemon Pokemon { get; set; }
        public TypePok TypePok { get; set; }
        public Talent Talent { get; set; }

        public string CorrectAnswer { get; set; }
        public byte[] ByteTypePok { get; set; }
        public byte[] ByteDetectiveP { get; set; }
        public byte[] ByteResult { get; set; }
        public string[] FormatLibelleQuestion { get; set; }

        public bool IsQTypPok { get; set; }
        public bool IsQTypTyp { get; set; }
        public bool IsQTypPokStat { get; set; }
        public bool IsQTypPokDesc { get; set; }
        public bool IsQTypTypPok { get; set; }
        public bool IsQTypTalent { get; set; }     
        public bool IsQTypTypPokVarious { get; set; }
        public bool IsQTypWeakPokVarious { get; set; }
        public bool IsQTypPokTalentVarious { get; set; }
        public bool IsQTypPokFamilyVarious { get; set; }
        public bool IsQTypPokTypVarious { get; set; }
    }
}
