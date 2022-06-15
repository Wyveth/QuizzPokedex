using Microcharts;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class ResultQuizzViewModel : MvxViewModel<QuestionAnswers>
    {
        #region Field
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IQuizzService _quizzService;
        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IDifficultyService _difficultyService;
        private readonly IAnswerService _answerService;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;
        #endregion

        #region Constructor
        public ResultQuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IDifficultyService difficultyService, IAnswerService answerService, IQuestionTypeService questionTypeService, ITypePokService typePokService)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _difficultyService = difficultyService;
            _answerService = answerService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
        }
        #endregion

        #region Public Methods
        public override void Prepare(QuestionAnswers questionAnswers)
        {
            QuestionAnswers = questionAnswers;
            base.Prepare();
        }

        public async override Task Initialize()
        {
            BackGroundTask = MvxNotifyTask.Create(BackGroundAsync);
            LoadDataTask = MvxNotifyTask.Create(LoadDataAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task BackGroundAsync()
        {
            ImgPokedexUp = await Utils.GetByteAssetImage(Constantes.Pokedex_Up);
            ImgPokedexDown = await Utils.GetByteAssetImage(Constantes.Pokedex_Down);
        }

        private async Task LoadDataAsync()
        {
            //await CreateChartStats(QuestionAnswers.Quizz);

            await Task.Run(async () =>
            {
                List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(QuestionAnswers.Quizz.QuestionsID);
                List<Answer> answersCorrect = new List<Answer>();
                foreach (Question question in questions)
                {
                    List<Answer> answers = await _answerService.GetAllByAnswersIDAsync(question.AnswersID);
                    Answer answerCorrect = answers.Find(m => m.IsCorrect.Equals(true) && m.IsSelected.Equals(true));
                    await LoadImgAnswsers(question, answerCorrect);
                    if (answerCorrect != null)
                        answersCorrect.Add(answerCorrect);
                }
                Result = answersCorrect.Count + "/" + questions.Count;
            });
            
        }

        private async Task LoadImgAnswsers(Question question, Answer answer)
        {
            switch (question.Order)
            {
                case 1:
                    ByteResult1 = await GetByteImgAnswer(answer);
                    break;
                case 2:
                    ByteResult2 = await GetByteImgAnswer(answer);
                    break;
                case 3:
                    ByteResult3 = await GetByteImgAnswer(answer);
                    break;
                case 4:
                    ByteResult4 = await GetByteImgAnswer(answer);
                    break;
                case 5:
                    ByteResult5 = await GetByteImgAnswer(answer);
                    break;
                case 6:
                    ByteResult6 = await GetByteImgAnswer(answer);
                    break;
                case 7:
                    ByteResult7 = await GetByteImgAnswer(answer);
                    break;
                case 8:
                    ByteResult8 = await GetByteImgAnswer(answer);
                    break;
                case 9:
                    ByteResult9 = await GetByteImgAnswer(answer);
                    break;
                case 10:
                    ByteResult10 = await GetByteImgAnswer(answer);
                    break;
                case 11:
                    ByteResult11 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion11 = await GetVisibleQuestion(question);
                    break;
                case 12:
                    ByteResult12 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion12 = await GetVisibleQuestion(question);
                    break;
                case 13:
                    ByteResult13 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion13 = await GetVisibleQuestion(question);
                    break;
                case 14:
                    ByteResult14 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion14 = await GetVisibleQuestion(question);
                    break;
                case 15:
                    ByteResult15 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion15 = await GetVisibleQuestion(question);
                    break;
                case 16:
                    ByteResult16 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion16 = await GetVisibleQuestion(question);
                    break;
                case 17:
                    ByteResult17 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion17 = await GetVisibleQuestion(question);
                    break;
                case 18:
                    ByteResult18 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion18 = await GetVisibleQuestion(question);
                    break;
                case 19:
                    ByteResult19 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion19 = await GetVisibleQuestion(question);
                    break;
                case 20:
                    ByteResult20 = await GetByteImgAnswer(answer);
                    IsVisibleQuestion20 = await GetVisibleQuestion(question);
                    break;
                default:
                    break;
            }
        }

        private Task<bool> GetVisibleQuestion(Question question)
        {
            if (question != null)
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }

        private async Task<byte[]> GetByteImgAnswer(Answer answer)
        {
            if(answer != null)
                return await Utils.GetByteAssetImage(Constantes.StarSuccess);
            else
                return await Utils.GetByteAssetImage(Constantes.StarWrong);
        }

        private async Task<Chart> CreateChartStats(Quizz quizz)
        {
            ChartEntry[] entries = await GenerateEntriesChart(quizz);

            return await Task.FromResult(new BarChart
            {
                Entries = entries,
                LabelTextSize = 42,
                AnimationProgress = 7,
                AnimationDuration = TimeSpan.FromSeconds(7),
                IsAnimated = true,
                MaxValue = 255,
            });
        }

        private async Task<ChartEntry[]> GenerateEntriesChart(Quizz quizz)
        {
            ChartEntry[] chartEntries = new ChartEntry[3];
            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(quizz.QuestionsID);

            int i = 0;
            foreach (QuestionType questionType in await _questionTypeService.GetAllAsync())
            {
                List<Question> questionFilter = questions.FindAll(m => m.QuestionTypeID.Equals(questionType.Id));

                chartEntries[i].Label = "Test: " + questionType.Libelle;
                chartEntries[i].ValueLabel = questionType.Libelle.ToString();
                chartEntries[i].Color = SKColor.Parse("#6BC563");
                
                i++;
            }

            return await Task.FromResult(chartEntries);
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationValidationCommandAsync => new MvxAsyncCommand(NavigationValidationAsync);

        private async Task NavigationValidationAsync()
        {
            await _navigation.Close(this);
        }
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask BackGroundTask { get; private set; }
        public MvxNotifyTask LoadDataTask { get; private set; }
        #endregion

        #region Data
        private QuestionAnswers _questionAnswers;

        public QuestionAnswers QuestionAnswers
        {
            get { return _questionAnswers; }
            set { SetProperty(ref _questionAnswers, value); }
        }

        private string _result;

        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }
        #endregion

        #region Image Background
        private byte[] _imgPokedexUp;

        public byte[] ImgPokedexUp
        {
            get { return _imgPokedexUp; }
            set { SetProperty(ref _imgPokedexUp, value); }
        }

        private byte[] _imgPokedexDown;

        public byte[] ImgPokedexDown
        {
            get { return _imgPokedexDown; }
            set { SetProperty(ref _imgPokedexDown, value); }
        }
        #endregion

        #region Questions
        #region Question 1
        private Question _Question1;

        public Question Question1
        {
            get { return _Question1; }
            set { SetProperty(ref _Question1, value); }
        }

        private bool _isVisibleQuestion1 = true;

        public bool IsVisibleQuestion1
        {
            get { return _isVisibleQuestion1; }
            set { SetProperty(ref _isVisibleQuestion1, value); }
        }

        private byte[] _byteResult1;

        public byte[] ByteResult1
        {
            get { return _byteResult1; }
            set { SetProperty(ref _byteResult1, value); }
        }
        #endregion

        #region Question 2
        private Question _Question2;

        public Question Question2
        {
            get { return _Question2; }
            set { SetProperty(ref _Question2, value); }
        }

        private bool _isVisibleQuestion2 = true;

        public bool IsVisibleQuestion2
        {
            get { return _isVisibleQuestion2; }
            set { SetProperty(ref _isVisibleQuestion2, value); }
        }

        private byte[] _byteResult2;

        public byte[] ByteResult2
        {
            get { return _byteResult2; }
            set { SetProperty(ref _byteResult2, value); }
        }
        #endregion

        #region Question 3
        private Question _Question3;

        public Question Question3
        {
            get { return _Question3; }
            set { SetProperty(ref _Question3, value); }
        }

        private bool _isVisibleQuestion3 = true;

        public bool IsVisibleQuestion3
        {
            get { return _isVisibleQuestion3; }
            set { SetProperty(ref _isVisibleQuestion3, value); }
        }

        private byte[] _byteResult3;

        public byte[] ByteResult3
        {
            get { return _byteResult3; }
            set { SetProperty(ref _byteResult3, value); }
        }
        #endregion

        #region Question 4
        private Question question4;

        public Question Question4
        {
            get { return question4; }
            set { SetProperty(ref question4, value); }
        }

        private bool _isVisibleQuestion4 = true;

        public bool IsVisibleQuestion4
        {
            get { return _isVisibleQuestion4; }
            set { SetProperty(ref _isVisibleQuestion4, value); }
        }

        private byte[] _byteResult4;

        public byte[] ByteResult4
        {
            get { return _byteResult4; }
            set { SetProperty(ref _byteResult4, value); }
        }
        #endregion

        #region Question 5
        private Question question5;

        public Question Question5
        {
            get { return question5; }
            set { SetProperty(ref question5, value); }
        }

        private bool _isVisibleQuestion5 = true;

        public bool IsVisibleQuestion5
        {
            get { return _isVisibleQuestion5; }
            set { SetProperty(ref _isVisibleQuestion5, value); }
        }

        private byte[] _byteResult5;

        public byte[] ByteResult5
        {
            get { return _byteResult5; }
            set { SetProperty(ref _byteResult5, value); }
        }
        #endregion

        #region Question 6
        private Question question6;

        public Question Question6
        {
            get { return question6; }
            set { SetProperty(ref question6, value); }
        }

        private bool _isVisibleQuestion6 = true;

        public bool IsVisibleQuestion6
        {
            get { return _isVisibleQuestion6; }
            set { SetProperty(ref _isVisibleQuestion6, value); }
        }

        private byte[] _byteResult6;

        public byte[] ByteResult6
        {
            get { return _byteResult6; }
            set { SetProperty(ref _byteResult6, value); }
        }
        #endregion

        #region Question 7
        private Question question7;

        public Question Question7
        {
            get { return question7; }
            set { SetProperty(ref question7, value); }
        }

        private bool _isVisibleQuestion7 = true;

        public bool IsVisibleQuestion7
        {
            get { return _isVisibleQuestion7; }
            set { SetProperty(ref _isVisibleQuestion7, value); }
        }

        private byte[] _byteResult7;

        public byte[] ByteResult7
        {
            get { return _byteResult7; }
            set { SetProperty(ref _byteResult7, value); }
        }
        #endregion

        #region Question 8
        private Question question8;

        public Question Question8
        {
            get { return question8; }
            set { SetProperty(ref question8, value); }
        }

        private bool _isVisibleQuestion8 = true;

        public bool IsVisibleQuestion8
        {
            get { return _isVisibleQuestion8; }
            set { SetProperty(ref _isVisibleQuestion8, value); }
        }

        private byte[] _byteResult8;

        public byte[] ByteResult8
        {
            get { return _byteResult8; }
            set { SetProperty(ref _byteResult8, value); }
        }
        #endregion

        #region Question 9
        private Question question9;

        public Question Question9
        {
            get { return question9; }
            set { SetProperty(ref question9, value); }
        }

        private bool _isVisibleQuestion9 = true;

        public bool IsVisibleQuestion9
        {
            get { return _isVisibleQuestion9; }
            set { SetProperty(ref _isVisibleQuestion9, value); }
        }

        private byte[] _byteResult9;

        public byte[] ByteResult9
        {
            get { return _byteResult9; }
            set { SetProperty(ref _byteResult9, value); }
        }
        #endregion

        #region Question 10
        private Question question10;

        public Question Question10
        {
            get { return question10; }
            set { SetProperty(ref question10, value); }
        }

        private bool _isVisibleQuestion10 = true;

        public bool IsVisibleQuestion10
        {
            get { return _isVisibleQuestion10; }
            set { SetProperty(ref _isVisibleQuestion10, value); }
        }

        private byte[] _byteResult10;

        public byte[] ByteResult10
        {
            get { return _byteResult10; }
            set { SetProperty(ref _byteResult10, value); }
        }
        #endregion

        #region Question 11
        private Question question11;

        public Question Question11
        {
            get { return question11; }
            set { SetProperty(ref question11, value); }
        }

        private bool _isVisibleQuestion11 = false;

        public bool IsVisibleQuestion11
        {
            get { return _isVisibleQuestion11; }
            set { SetProperty(ref _isVisibleQuestion11, value); }
        }

        private byte[] _byteResult11;

        public byte[] ByteResult11
        {
            get { return _byteResult11; }
            set { SetProperty(ref _byteResult11, value); }
        }
        #endregion

        #region Question 12
        private Question question12;

        public Question Question12
        {
            get { return question12; }
            set { SetProperty(ref question12, value); }
        }

        private bool _isVisibleQuestion12 = false;

        public bool IsVisibleQuestion12
        {
            get { return _isVisibleQuestion12; }
            set { SetProperty(ref _isVisibleQuestion12, value); }
        }

        private byte[] _byteResult12;

        public byte[] ByteResult12
        {
            get { return _byteResult12; }
            set { SetProperty(ref _byteResult12, value); }
        }
        #endregion

        #region Question 13
        private Question question13;

        public Question Question13
        {
            get { return question13; }
            set { SetProperty(ref question13, value); }
        }

        private bool _isVisibleQuestion13 = false;

        public bool IsVisibleQuestion13
        {
            get { return _isVisibleQuestion13; }
            set { SetProperty(ref _isVisibleQuestion13, value); }
        }

        private byte[] _byteResult13;

        public byte[] ByteResult13
        {
            get { return _byteResult13; }
            set { SetProperty(ref _byteResult13, value); }
        }
        #endregion

        #region Question 14
        private Question question14;

        public Question Question14
        {
            get { return question14; }
            set { SetProperty(ref question14, value); }
        }

        private bool _isVisibleQuestion14 = false;

        public bool IsVisibleQuestion14
        {
            get { return _isVisibleQuestion14; }
            set { SetProperty(ref _isVisibleQuestion14, value); }
        }

        private byte[] _byteResult14;

        public byte[] ByteResult14
        {
            get { return _byteResult14; }
            set { SetProperty(ref _byteResult14, value); }
        }
        #endregion

        #region Question 15
        private Question question15;

        public Question Question15
        {
            get { return question15; }
            set { SetProperty(ref question15, value); }
        }

        private bool _isVisibleQuestion15 = false;

        public bool IsVisibleQuestion15
        {
            get { return _isVisibleQuestion15; }
            set { SetProperty(ref _isVisibleQuestion15, value); }
        }

        private byte[] _byteResult15;

        public byte[] ByteResult15
        {
            get { return _byteResult15; }
            set { SetProperty(ref _byteResult15, value); }
        }
        #endregion

        #region Question 16
        private Question question16;

        public Question Question16
        {
            get { return question16; }
            set { SetProperty(ref question16, value); }
        }

        private bool _isVisibleQuestion16 = false;

        public bool IsVisibleQuestion16
        {
            get { return _isVisibleQuestion16; }
            set { SetProperty(ref _isVisibleQuestion16, value); }
        }

        private byte[] _byteResult16;

        public byte[] ByteResult16
        {
            get { return _byteResult16; }
            set { SetProperty(ref _byteResult16, value); }
        }
        #endregion

        #region Question 17
        private Question question17;

        public Question Question17
        {
            get { return question17; }
            set { SetProperty(ref question17, value); }
        }

        private bool _isVisibleQuestion17 = false;

        public bool IsVisibleQuestion17
        {
            get { return _isVisibleQuestion17; }
            set { SetProperty(ref _isVisibleQuestion17, value); }
        }

        private byte[] _byteResult17;

        public byte[] ByteResult17
        {
            get { return _byteResult17; }
            set { SetProperty(ref _byteResult17, value); }
        }
        #endregion

        #region Question 18
        private Question question18;

        public Question Question18
        {
            get { return question18; }
            set { SetProperty(ref question18, value); }
        }

        private bool _isVisibleQuestion18 = false;

        public bool IsVisibleQuestion18
        {
            get { return _isVisibleQuestion18; }
            set { SetProperty(ref _isVisibleQuestion18, value); }
        }

        private byte[] _byteResult18;

        public byte[] ByteResult18
        {
            get { return _byteResult18; }
            set { SetProperty(ref _byteResult18, value); }
        }
        #endregion

        #region Question 19
        private Question question19;

        public Question Question19
        {
            get { return question19; }
            set { SetProperty(ref question19, value); }
        }

        private bool _isVisibleQuestion19 = false;

        public bool IsVisibleQuestion19
        {
            get { return _isVisibleQuestion19; }
            set { SetProperty(ref _isVisibleQuestion19, value); }
        }

        private byte[] _byteResult19;

        public byte[] ByteResult19
        {
            get { return _byteResult19; }
            set { SetProperty(ref _byteResult19, value); }
        }
        #endregion

        #region Question 20

        private Question question20;

        public Question Question20
        {
            get { return question20; }
            set { SetProperty(ref question20, value); }
        }

        private bool _isVisibleQuestion20 = false;

        public bool IsVisibleQuestion20
        {
            get { return _isVisibleQuestion20; }
            set { SetProperty(ref _isVisibleQuestion20, value); }
        }

        private byte[] _byteResult20;

        public byte[] ByteResult20
        {
            get { return _byteResult20; }
            set { SetProperty(ref _byteResult20, value); }
        }
        #endregion
        #endregion
        #endregion
    }
}
