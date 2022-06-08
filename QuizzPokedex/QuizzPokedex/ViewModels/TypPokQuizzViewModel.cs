using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class TypPokQuizzViewModel : MvxViewModel<QuestionAnswers>
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
        public TypPokQuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IDifficultyService difficultyService, IAnswerService answerService, IQuestionTypeService questionTypeService, ITypePokService typePokService)
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
            Order = questionAnswers.Question.Order;
            base.Prepare();
        }

        public async override Task Initialize()
        {
            BackGroundTask = MvxNotifyTask.Create(BackGroundAsync);
            LoadQuestionAnswersTask = MvxNotifyTask.Create(LoadQuestionAnswersAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task BackGroundAsync()
        {
            ImgPokedexUp = await Utils.getByteAssetImage(Constantes.Pokedex_Up);
            ImgPokedexDown = await Utils.getByteAssetImage(Constantes.Pokedex_Down);
        }

        private async Task LoadQuestionAnswersAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            QuestionType = await _questionTypeService.GetByIdAsync(QuestionAnswers.Question.QuestionTypeID);
            Pokemon = await _pokemonService.GetByIdAsync(QuestionAnswers.Answers.Find(m => m.IsCorrect.Equals(true)).IsCorrectID);
            int typeID = int.Parse(Pokemon.TypesID.Split(',')[0]);
            TypePok = await _typePokService.GetByIdAsync(typeID);

            await LoadDataDifficulty();
        }

        private async Task LoadDataDifficulty()
        {
            //A revoir
            Difficulty difficulty = await _difficultyService.GetByIdAsync(QuestionType.DifficultyID);
            if (difficulty.Libelle.Equals(Constantes.EasyTQ))
            {
                EasyQ = true;
                Answer1 = QuestionAnswers.Answers[0];
                Answer2 = QuestionAnswers.Answers[1];
                Answer3 = QuestionAnswers.Answers[2];
                Answer4 = QuestionAnswers.Answers[3];
            }
            else if (difficulty.Libelle.Equals(Constantes.NormalTQ))
            {
                NormalQ = true;
                Answer1 = QuestionAnswers.Answers[0];
                Answer2 = QuestionAnswers.Answers[1];
                Answer3 = QuestionAnswers.Answers[2];
                Answer4 = QuestionAnswers.Answers[3];
                Answer5 = QuestionAnswers.Answers[4];
                Answer6 = QuestionAnswers.Answers[5];
                Answer7 = QuestionAnswers.Answers[6];
                Answer8 = QuestionAnswers.Answers[7];
            }
            else if (difficulty.Libelle.Equals(Constantes.HardTQ))
            {
                HardQ = true;
                Answer1 = QuestionAnswers.Answers[0];
                Answer2 = QuestionAnswers.Answers[1];
                Answer3 = QuestionAnswers.Answers[2];
                Answer4 = QuestionAnswers.Answers[3];
                Answer5 = QuestionAnswers.Answers[4];
                Answer6 = QuestionAnswers.Answers[5];
                Answer7 = QuestionAnswers.Answers[6];
                Answer8 = QuestionAnswers.Answers[7];
                Answer9 = QuestionAnswers.Answers[8];
                Answer10 = QuestionAnswers.Answers[9];
                Answer11 = QuestionAnswers.Answers[10];
                Answer12 = QuestionAnswers.Answers[11];
            }
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationValidationCommandAsync => new MvxAsyncCommand(NavigationValidationAsync);

        public IMvxAsyncCommand NavigationNextCommandAsync => new MvxAsyncCommand(NavigationNextAsync);

        private async Task NavigationValidationAsync()
        {
            await _navigation.Close(this);
        }

        private async Task NavigationNextAsync()
        {
            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(QuestionAnswers.Quizz.QuestionsID);
            Question question = questions.Find(m => m.Order.Equals(Order + 1));
            QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
            List<Answer> answers = new List<Answer>();
            answers.AddRange(await _answerService.GetAllByAnswersIDAsync(question.AnswersID));

            QuestionAnswers questionAnswers = new QuestionAnswers()
            {
                Quizz = QuestionAnswers.Quizz,
                Question = question,
                Answers = answers
            };

            await _navigation.Close(this);
            await _navigation.Navigate<TypPokQuizzViewModel, QuestionAnswers>(questionAnswers);
        }
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask BackGroundTask { get; private set; }
        public MvxNotifyTask LoadQuestionAnswersTask { get; private set; }
        #endregion

        private QuestionAnswers _questionAnswers;

        public QuestionAnswers QuestionAnswers
        {
            get { return _questionAnswers; }
            set { SetProperty(ref _questionAnswers, value); }
        }

        private int _order;

        public int Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }

        private Pokemon _pokemon;

        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }

        private TypePok _typePok;

        public TypePok TypePok
        {
            get { return _typePok; }
            set { SetProperty(ref _typePok, value); }
        }

        private Question _question;

        public Question Question
        {
            get { return _question; }
            set { SetProperty(ref _question, value); }
        }

        private QuestionType _questionType;

        public QuestionType QuestionType
        {
            get { return _questionType; }
            set { SetProperty(ref _questionType, value); }
        }

        #region Difficulty
        private bool _easyQ = false;

        public bool EasyQ
        {
            get { return _easyQ; }
            set { SetProperty(ref _easyQ, value); }
        }

        private bool _normalQ = false;

        public bool NormalQ
        {
            get { return _normalQ; }
            set { SetProperty(ref _normalQ, value); }
        }

        private bool _hardQ = false;

        public bool HardQ
        {
            get { return _hardQ; }
            set { SetProperty(ref _hardQ, value); }
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

        #region Answers
        #region Answer 1
        private string _backgroundColorAnswer1 = "#FFFFFF";

        public string BackgroundColorAnswer1
        {
            get { return _backgroundColorAnswer1; }
            set { SetProperty(ref _backgroundColorAnswer1, value); }
        }

        private Answer _Answer1;

        public Answer Answer1
        {
            get { return _Answer1; }
            set { SetProperty(ref _Answer1, value); }
        }

        private string _textColorAnswer1 = "#000000";

        public string TextColorAnswer1
        {
            get { return _textColorAnswer1; }
            set { SetProperty(ref _textColorAnswer1, value); }
        }

        private bool _isEnabledAnswer1 = true;

        public bool IsEnabledAnswer1
        {
            get { return _isEnabledAnswer1; }
            set { SetProperty(ref _isEnabledAnswer1, value); }
        }

        private bool _isVisibleAnswer1 = true;

        public bool IsVisibleAnswer1
        {
            get { return _isVisibleAnswer1; }
            set { SetProperty(ref _isVisibleAnswer1, value); }
        }
        #endregion

        #region Answer 2
        private string _backgroundColorAnswer2 = "#FFFFFF";

        public string BackgroundColorAnswer2
        {
            get { return _backgroundColorAnswer2; }
            set { SetProperty(ref _backgroundColorAnswer2, value); }
        }

        private Answer _Answer2;

        public Answer Answer2
        {
            get { return _Answer2; }
            set { SetProperty(ref _Answer2, value); }
        }

        private string _textColorAnswer2 = "#000000";

        public string TextColorAnswer2
        {
            get { return _textColorAnswer2; }
            set { SetProperty(ref _textColorAnswer2, value); }
        }

        private bool _isEnabledAnswer2 = true;

        public bool IsEnabledAnswer2
        {
            get { return _isEnabledAnswer2; }
            set { SetProperty(ref _isEnabledAnswer2, value); }
        }

        private bool _isVisibleAnswer2 = true;

        public bool IsVisibleAnswer2
        {
            get { return _isVisibleAnswer2; }
            set { SetProperty(ref _isVisibleAnswer2, value); }
        }
        #endregion

        #region Answer 3
        private string _backgroundColorAnswer3 = "#FFFFFF";

        public string BackgroundColorAnswer3
        {
            get { return _backgroundColorAnswer3; }
            set { SetProperty(ref _backgroundColorAnswer3, value); }
        }

        private Answer _Answer3;

        public Answer Answer3
        {
            get { return _Answer3; }
            set { SetProperty(ref _Answer3, value); }
        }

        private string _textColorAnswer3 = "#000000";

        public string TextColorAnswer3
        {
            get { return _textColorAnswer3; }
            set { SetProperty(ref _textColorAnswer3, value); }
        }

        private bool _isEnabledAnswer3 = true;

        public bool IsEnabledAnswer3
        {
            get { return _isEnabledAnswer3; }
            set { SetProperty(ref _isEnabledAnswer3, value); }
        }

        private bool _isVisibleAnswer3 = true;

        public bool IsVisibleAnswer3
        {
            get { return _isVisibleAnswer3; }
            set { SetProperty(ref _isVisibleAnswer3, value); }
        }
        #endregion

        #region Answer 4
        private string _backgroundColorAnswer4 = "#FFFFFF";

        public string BackgroundColorAnswer4
        {
            get { return _backgroundColorAnswer4; }
            set { SetProperty(ref _backgroundColorAnswer4, value); }
        }

        private Answer answer4;

        public Answer Answer4
        {
            get { return answer4; }
            set { SetProperty(ref answer4, value); }
        }

        private string _textColorAnswer4 = "#000000";

        public string TextColorAnswer4
        {
            get { return _textColorAnswer4; }
            set { SetProperty(ref _textColorAnswer4, value); }
        }

        private bool _isEnabledAnswer4 = true;

        public bool IsEnabledAnswer4
        {
            get { return _isEnabledAnswer4; }
            set { SetProperty(ref _isEnabledAnswer4, value); }
        }

        private bool _isVisibleAnswer4 = true;

        public bool IsVisibleAnswer4
        {
            get { return _isVisibleAnswer4; }
            set { SetProperty(ref _isVisibleAnswer4, value); }
        }
        #endregion

        #region Answer 5
        private string _backgroundColorAnswer5 = "#FFFFFF";

        public string BackgroundColorAnswer5
        {
            get { return _backgroundColorAnswer5; }
            set { SetProperty(ref _backgroundColorAnswer5, value); }
        }

        private Answer answer5;

        public Answer Answer5
        {
            get { return answer5; }
            set { SetProperty(ref answer5, value); }
        }

        private string _textColorAnswer5 = "#000000";

        public string TextColorAnswer5
        {
            get { return _textColorAnswer5; }
            set { SetProperty(ref _textColorAnswer5, value); }
        }

        private bool _isEnabledAnswer5 = true;

        public bool IsEnabledAnswer5
        {
            get { return _isEnabledAnswer5; }
            set { SetProperty(ref _isEnabledAnswer5, value); }
        }

        private bool _isVisibleAnswer5 = true;

        public bool IsVisibleAnswer5
        {
            get { return _isVisibleAnswer5; }
            set { SetProperty(ref _isVisibleAnswer5, value); }
        }
        #endregion

        #region Answer 6
        private string _backgroundColorAnswer6 = "#FFFFFF";

        public string BackgroundColorAnswer6
        {
            get { return _backgroundColorAnswer6; }
            set { SetProperty(ref _backgroundColorAnswer6, value); }
        }

        private Answer answer6;

        public Answer Answer6
        {
            get { return answer6; }
            set { SetProperty(ref answer6, value); }
        }

        private string _textColorAnswer6 = "#000000";

        public string TextColorAnswer6
        {
            get { return _textColorAnswer6; }
            set { SetProperty(ref _textColorAnswer6, value); }
        }

        private bool _isEnabledAnswer6 = true;

        public bool IsEnabledAnswer6
        {
            get { return _isEnabledAnswer6; }
            set { SetProperty(ref _isEnabledAnswer6, value); }
        }

        private bool _isVisibleAnswer6 = true;

        public bool IsVisibleAnswer6
        {
            get { return _isVisibleAnswer6; }
            set { SetProperty(ref _isVisibleAnswer6, value); }
        }
        #endregion

        #region Answer 7
        private string _backgroundColorAnswer7 = "#FFFFFF";

        public string BackgroundColorAnswer7
        {
            get { return _backgroundColorAnswer7; }
            set { SetProperty(ref _backgroundColorAnswer7, value); }
        }

        private Answer answer7;

        public Answer Answer7
        {
            get { return answer7; }
            set { SetProperty(ref answer7, value); }
        }

        private string _textColorAnswer7 = "#000000";

        public string TextColorAnswer7
        {
            get { return _textColorAnswer7; }
            set { SetProperty(ref _textColorAnswer7, value); }
        }

        private bool _isEnabledAnswer7;

        public bool IsEnabledAnswer7
        {
            get { return _isEnabledAnswer7; }
            set { SetProperty(ref _isEnabledAnswer7, value); }
        }

        private bool _isVisibleAnswer7;

        public bool IsVisibleAnswer7
        {
            get { return _isVisibleAnswer7; }
            set { SetProperty(ref _isVisibleAnswer7, value); }
        }
        #endregion

        #region Answer 8
        private string _backgroundColorAnswer8 = "#FFFFFF";

        public string BackgroundColorAnswer8
        {
            get { return _backgroundColorAnswer8; }
            set { SetProperty(ref _backgroundColorAnswer8, value); }
        }

        private Answer answer8;

        public Answer Answer8
        {
            get { return answer8; }
            set { SetProperty(ref answer8, value); }
        }

        private string _textColorAnswer8 = "#000000";

        public string TextColorAnswer8
        {
            get { return _textColorAnswer8; }
            set { SetProperty(ref _textColorAnswer8, value); }
        }

        private bool _isEnabledAnswer8;

        public bool IsEnabledAnswer8
        {
            get { return _isEnabledAnswer8; }
            set { SetProperty(ref _isEnabledAnswer8, value); }
        }

        private bool _isVisibleAnswer8;

        public bool IsVisibleAnswer8
        {
            get { return _isVisibleAnswer8; }
            set { SetProperty(ref _isVisibleAnswer8, value); }
        }
        #endregion

        #region Answer 9
        private string _backgroundColorAnswer9 = "#FFFFFF";

        public string BackgroundColorAnswer9
        {
            get { return _backgroundColorAnswer9; }
            set { SetProperty(ref _backgroundColorAnswer9, value); }
        }

        private Answer answer9;

        public Answer Answer9
        {
            get { return answer9; }
            set { SetProperty(ref answer9, value); }
        }

        private string _textColorAnswer9 = "#000000";

        public string TextColorAnswer9
        {
            get { return _textColorAnswer9; }
            set { SetProperty(ref _textColorAnswer9, value); }
        }

        private bool _isEnabledAnswer9 = true;

        public bool IsEnabledAnswer9
        {
            get { return _isEnabledAnswer9; }
            set { SetProperty(ref _isEnabledAnswer9, value); }
        }

        private bool _isVisibleAnswer9 = true;

        public bool IsVisibleAnswer9
        {
            get { return _isVisibleAnswer9; }
            set { SetProperty(ref _isVisibleAnswer9, value); }
        }
        #endregion

        #region Answer 10
        private string _backgroundColorAnswer10 = "#FFFFFF";

        public string BackgroundColorAnswer10
        {
            get { return _backgroundColorAnswer10; }
            set { SetProperty(ref _backgroundColorAnswer10, value); }
        }

        private Answer answer10;

        public Answer Answer10
        {
            get { return answer10; }
            set { SetProperty(ref answer10, value); }
        }

        private string _textColorAnswer10 = "#000000";

        public string TextColorAnswer10
        {
            get { return _textColorAnswer10; }
            set { SetProperty(ref _textColorAnswer10, value); }
        }

        private bool _isEnabledAnswer10 = true;

        public bool IsEnabledAnswer10
        {
            get { return _isEnabledAnswer10; }
            set { SetProperty(ref _isEnabledAnswer10, value); }
        }

        private bool _isVisibleAnswer10 = true;

        public bool IsVisibleAnswer10
        {
            get { return _isVisibleAnswer10; }
            set { SetProperty(ref _isVisibleAnswer10, value); }
        }
        #endregion

        #region Answer 11
        private string _backgroundColorAnswer11 = "#FFFFFF";

        public string BackgroundColorAnswer11
        {
            get { return _backgroundColorAnswer11; }
            set { SetProperty(ref _backgroundColorAnswer11, value); }
        }

        private Answer answer11;

        public Answer Answer11
        {
            get { return answer11; }
            set { SetProperty(ref answer11, value); }
        }

        private string _textColorAnswer11 = "#000000";

        public string TextColorAnswer11
        {
            get { return _textColorAnswer11; }
            set { SetProperty(ref _textColorAnswer11, value); }
        }

        private bool _isEnabledAnswer11 = true;

        public bool IsEnabledAnswer11
        {
            get { return _isEnabledAnswer11; }
            set { SetProperty(ref _isEnabledAnswer11, value); }
        }

        private bool _isVisibleAnswer11 = true;

        public bool IsVisibleAnswer11
        {
            get { return _isVisibleAnswer11; }
            set { SetProperty(ref _isVisibleAnswer11, value); }
        }
        #endregion

        #region Answer 12
        private string _backgroundColorAnswer12 = "#FFFFFF";

        public string BackgroundColorAnswer12
        {
            get { return _backgroundColorAnswer12; }
            set { SetProperty(ref _backgroundColorAnswer12, value); }
        }

        private Answer answer12;

        public Answer Answer12
        {
            get { return answer12; }
            set { SetProperty(ref answer12, value); }
        }

        private string _textColorAnswer12 = "#000000";

        public string TextColorAnswer12
        {
            get { return _textColorAnswer12; }
            set { SetProperty(ref _textColorAnswer12, value); }
        }

        private bool _isEnabledAnswer12 = true;

        public bool IsEnabledAnswer12
        {
            get { return _isEnabledAnswer12; }
            set { SetProperty(ref _isEnabledAnswer12, value); }
        }

        private bool _isVisibleAnswer12 = true;

        public bool IsVisibleAnswer12
        {
            get { return _isVisibleAnswer12; }
            set { SetProperty(ref _isVisibleAnswer12, value); }
        }
        #endregion

        #region Answer 13
        private string _backgroundColorAnswer13 = "#FFFFFF";

        public string BackgroundColorAnswer13
        {
            get { return _backgroundColorAnswer13; }
            set { SetProperty(ref _backgroundColorAnswer13, value); }
        }

        private Answer answer13;

        public Answer Answer13
        {
            get { return answer13; }
            set { SetProperty(ref answer13, value); }
        }

        private string _textColorAnswer13 = "#000000";

        public string TextColorAnswer13
        {
            get { return _textColorAnswer13; }
            set { SetProperty(ref _textColorAnswer13, value); }
        }

        private bool _isEnabledAnswer13 = true;

        public bool IsEnabledAnswer13
        {
            get { return _isEnabledAnswer13; }
            set { SetProperty(ref _isEnabledAnswer13, value); }
        }

        private bool _isVisibleAnswer13 = true;

        public bool IsVisibleAnswer13
        {
            get { return _isVisibleAnswer13; }
            set { SetProperty(ref _isVisibleAnswer13, value); }
        }
        #endregion

        #region Answer 14
        private string _backgroundColorAnswer14 = "#FFFFFF";

        public string BackgroundColorAnswer14
        {
            get { return _backgroundColorAnswer14; }
            set { SetProperty(ref _backgroundColorAnswer14, value); }
        }

        private Answer answer14;

        public Answer Answer14
        {
            get { return answer14; }
            set { SetProperty(ref answer14, value); }
        }

        private string _textColorAnswer14 = "#000000";

        public string TextColorAnswer14
        {
            get { return _textColorAnswer14; }
            set { SetProperty(ref _textColorAnswer14, value); }
        }

        private bool _isEnabledAnswer14 = true;

        public bool IsEnabledAnswer14
        {
            get { return _isEnabledAnswer14; }
            set { SetProperty(ref _isEnabledAnswer14, value); }
        }

        private bool _isVisibleAnswer14 = true;

        public bool IsVisibleAnswer14
        {
            get { return _isVisibleAnswer14; }
            set { SetProperty(ref _isVisibleAnswer14, value); }
        }
        #endregion

        #region Answer 15
        private string _backgroundColorAnswer15 = "#FFFFFF";

        public string BackgroundColorAnswer15
        {
            get { return _backgroundColorAnswer15; }
            set { SetProperty(ref _backgroundColorAnswer15, value); }
        }

        private Answer answer15;

        public Answer Answer15
        {
            get { return answer15; }
            set { SetProperty(ref answer15, value); }
        }

        private string _textColorAnswer15 = "#000000";

        public string TextColorAnswer15
        {
            get { return _textColorAnswer15; }
            set { SetProperty(ref _textColorAnswer15, value); }
        }

        private bool _isEnabledAnswer15 = true;

        public bool IsEnabledAnswer15
        {
            get { return _isEnabledAnswer15; }
            set { SetProperty(ref _isEnabledAnswer15, value); }
        }

        private bool _isVisibleAnswer15 = true;

        public bool IsVisibleAnswer15
        {
            get { return _isVisibleAnswer15; }
            set { SetProperty(ref _isVisibleAnswer15, value); }
        }
        #endregion

        #region Answer 16
        private string _backgroundColorAnswer16 = "#FFFFFF";

        public string BackgroundColorAnswer16
        {
            get { return _backgroundColorAnswer16; }
            set { SetProperty(ref _backgroundColorAnswer16, value); }
        }

        private Answer answer16;

        public Answer Answer16
        {
            get { return answer16; }
            set { SetProperty(ref answer16, value); }
        }

        private string _textColorAnswer16 = "#000000";

        public string TextColorAnswer16
        {
            get { return _textColorAnswer16; }
            set { SetProperty(ref _textColorAnswer16, value); }
        }

        private bool _isEnabledAnswer16 = true;

        public bool IsEnabledAnswer16
        {
            get { return _isEnabledAnswer16; }
            set { SetProperty(ref _isEnabledAnswer16, value); }
        }

        private bool _isVisibleAnswer16 = true;

        public bool IsVisibleAnswer16
        {
            get { return _isVisibleAnswer16; }
            set { SetProperty(ref _isVisibleAnswer16, value); }
        }
        #endregion

        #region Answer 17
        private string _backgroundColorAnswer17 = "#FFFFFF";

        public string BackgroundColorAnswer17
        {
            get { return _backgroundColorAnswer17; }
            set { SetProperty(ref _backgroundColorAnswer17, value); }
        }

        private Answer answer17;

        public Answer Answer17
        {
            get { return answer17; }
            set { SetProperty(ref answer17, value); }
        }

        private string _textColorAnswer17 = "#000000";

        public string TextColorAnswer17
        {
            get { return _textColorAnswer17; }
            set { SetProperty(ref _textColorAnswer17, value); }
        }

        private bool _isEnabledAnswer17 = true;

        public bool IsEnabledAnswer17
        {
            get { return _isEnabledAnswer17; }
            set { SetProperty(ref _isEnabledAnswer17, value); }
        }

        private bool _isVisibleAnswer17 = true;

        public bool IsVisibleAnswer17
        {
            get { return _isVisibleAnswer17; }
            set { SetProperty(ref _isVisibleAnswer17, value); }
        }
        #endregion

        #region Answer 18
        private string _backgroundColorAnswer18 = "#FFFFFF";

        public string BackgroundColorAnswer18
        {
            get { return _backgroundColorAnswer18; }
            set { SetProperty(ref _backgroundColorAnswer18, value); }
        }

        private Answer answer18;

        public Answer Answer18
        {
            get { return answer18; }
            set { SetProperty(ref answer18, value); }
        }

        private string _textColorAnswer18 = "#000000";

        public string TextColorAnswer18
        {
            get { return _textColorAnswer18; }
            set { SetProperty(ref _textColorAnswer18, value); }
        }

        private bool _isEnabledAnswer18 = true;

        public bool IsEnabledAnswer18
        {
            get { return _isEnabledAnswer18; }
            set { SetProperty(ref _isEnabledAnswer18, value); }
        }

        private bool _isVisibleAnswer18 = true;

        public bool IsVisibleAnswer18
        {
            get { return _isVisibleAnswer18; }
            set { SetProperty(ref _isVisibleAnswer18, value); }
        }
        #endregion

        #endregion
        #endregion
    }
}
