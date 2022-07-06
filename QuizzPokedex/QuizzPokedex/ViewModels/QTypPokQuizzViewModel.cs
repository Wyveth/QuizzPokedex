using FFImageLoading.Work;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class QTypPokQuizzViewModel : MvxViewModel<QuestionAnswers>
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
        private readonly IMvxMessenger _messenger;
        #endregion

        #region Constructor
        public QTypPokQuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IDifficultyService difficultyService, IAnswerService answerService, IQuestionTypeService questionTypeService, ITypePokService typePokService, IMvxMessenger messenger)
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
            _messenger = messenger;
        }
        #endregion

        #region Public Methods
        public override void Prepare(QuestionAnswers questionAnswers)
        {
            QuestionAnswers = questionAnswers;
            Order = questionAnswers.Question.Order;

            Transformations = new List<ITransformation>();

            FormatQuestion = new string[] { Order.ToString(), questionAnswers.Quizz.QuestionsID.Split(',').Length.ToString() };

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
            ImgPokedexUp = await Utils.GetByteAssetImage(Constantes.Pokedex_Up);
            ImgPokedexDown = await Utils.GetByteAssetImage(Constantes.Pokedex_Down);
        }

        private async Task LoadQuestionAnswersAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            QuestionType = await _questionTypeService.GetByIdAsync(QuestionAnswers.Question.QuestionTypeID);
            Transformations = await Utils.GetTransformationImage(QuestionType);
            Delay = await Utils.GetTransformationImageDelay(QuestionType);

            Pokemon = await _pokemonService.GetByIdAsync(QuestionAnswers.Answers.Find(m => m.IsCorrect.Equals(true)).IsCorrectID);

            int typeID = int.Parse(Pokemon.TypesID.Split(',')[0]);
            TypePok = await _typePokService.GetByIdAsync(typeID);

            Difficulty difficulty = await _difficultyService.GetByIdAsync(QuestionType.DifficultyID);
            DifficultyByte = await Utils.GetBytesDifficulty(difficulty);
            await LoadDataDifficulty(difficulty);
            await LoadUIDifficulty(difficulty);
        }

        private async Task LoadDataDifficulty(Difficulty difficulty)
        {
            await Task.Run(() =>
            {
                if (difficulty.Libelle.Equals(Constantes.EasyTQ)
                || difficulty.Libelle.Equals(Constantes.NormalTQ)
                || difficulty.Libelle.Equals(Constantes.HardTQ))
                {
                    Answer1 = QuestionAnswers.Answers.Find(m => m.Order.Equals(1));
                    Answer2 = QuestionAnswers.Answers.Find(m => m.Order.Equals(2));
                    Answer3 = QuestionAnswers.Answers.Find(m => m.Order.Equals(3));
                    Answer4 = QuestionAnswers.Answers.Find(m => m.Order.Equals(4));
                }

                if (difficulty.Libelle.Equals(Constantes.NormalTQ)
                    || difficulty.Libelle.Equals(Constantes.HardTQ))
                {
                    Answer5 = QuestionAnswers.Answers.Find(m => m.Order.Equals(5));
                    Answer6 = QuestionAnswers.Answers.Find(m => m.Order.Equals(6));
                    Answer7 = QuestionAnswers.Answers.Find(m => m.Order.Equals(7));
                    Answer8 = QuestionAnswers.Answers.Find(m => m.Order.Equals(8));
                }

                if (difficulty.Libelle.Equals(Constantes.HardTQ))
                {
                    Answer9 = QuestionAnswers.Answers.Find(m => m.Order.Equals(9));
                    Answer10 = QuestionAnswers.Answers.Find(m => m.Order.Equals(10));
                    Answer11 = QuestionAnswers.Answers.Find(m => m.Order.Equals(11));
                    Answer12 = QuestionAnswers.Answers.Find(m => m.Order.Equals(12));
                }
            });
        }

        private async Task LoadUIDifficulty(Difficulty difficulty)
        {
            await Task.Run(() =>
            {
                if (difficulty.Libelle.Equals(Constantes.EasyTQ))
                    EasyQ = true;
                else if (difficulty.Libelle.Equals(Constantes.NormalTQ))
                    NormalQ = true;
                else if (difficulty.Libelle.Equals(Constantes.HardTQ))
                    HardQ = true;
            });
        }

        private async Task ResetData()
        {
            SelectedAnswer.IsSelected = false;
            await UpdateUIByOrder(SelectedAnswer);
        }

        private async Task<bool> UpdateUIByOrder(Answer answer)
        {
            bool update = false;
            switch (answer.Order)
            {
                case 1:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer1 = TypePok.InfoColor;
                        TextColorAnswer1 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer1 = Constantes.WhiteHexa;
                        TextColorAnswer1 = Constantes.BlackHexa;
                    }
                    break;
                case 2:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer2 = TypePok.InfoColor;
                        TextColorAnswer2 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer2 = Constantes.WhiteHexa;
                        TextColorAnswer2 = Constantes.BlackHexa;
                    }
                    break;
                case 3:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer3 = TypePok.InfoColor;
                        TextColorAnswer3 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer3 = Constantes.WhiteHexa;
                        TextColorAnswer3 = Constantes.BlackHexa;
                    }
                    break;
                case 4:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer4 = TypePok.InfoColor;
                        TextColorAnswer4 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer4 = Constantes.WhiteHexa;
                        TextColorAnswer4 = Constantes.BlackHexa;
                    }
                    break;
                case 5:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer5 = TypePok.InfoColor;
                        TextColorAnswer5 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer5 = Constantes.WhiteHexa;
                        TextColorAnswer5 = Constantes.BlackHexa;
                    }
                    break;
                case 6:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer6 = TypePok.InfoColor;
                        TextColorAnswer6 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer6 = Constantes.WhiteHexa;
                        TextColorAnswer6 = Constantes.BlackHexa;
                    }
                    break;
                case 7:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer7 = TypePok.InfoColor;
                        TextColorAnswer7 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer7 = Constantes.WhiteHexa;
                        TextColorAnswer7 = Constantes.BlackHexa;
                    }
                    break;
                case 8:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer8 = TypePok.InfoColor;
                        TextColorAnswer8 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer8 = Constantes.WhiteHexa;
                        TextColorAnswer8 = Constantes.BlackHexa;
                    }
                    break;
                case 9:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer9 = TypePok.InfoColor;
                        TextColorAnswer9 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer9 = Constantes.WhiteHexa;
                        TextColorAnswer9 = Constantes.BlackHexa;
                    }
                    break;
                case 10:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer10 = TypePok.InfoColor;
                        TextColorAnswer10 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer10 = Constantes.WhiteHexa;
                        TextColorAnswer10 = Constantes.BlackHexa;
                    }
                    break;
                case 11:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer11 = TypePok.InfoColor;
                        TextColorAnswer11 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer11 = Constantes.WhiteHexa;
                        TextColorAnswer11 = Constantes.BlackHexa;
                    }
                    break;
                case 12:
                    if (answer.IsSelected)
                    {
                        BackgroundColorAnswer12 = TypePok.InfoColor;
                        TextColorAnswer12 = Constantes.WhiteHexa;
                    }
                    else
                    {
                        BackgroundColorAnswer12 = Constantes.WhiteHexa;
                        TextColorAnswer12 = Constantes.BlackHexa;
                    }
                    break;
                default:
                    break;
            }
            update = true;
            return await Task.FromResult(update);
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationNextCommandAsync => new MvxAsyncCommand(NavigationNextAsync);
        public IMvxAsyncCommand<Answer> SelectedAnswerCommandAsync => new MvxAsyncCommand<Answer>(SelectedAnswerAsync);

        private async Task NavigationNextAsync()
        {
            await UpdateUIGenerateAsync();

            #region Update
            QuestionAnswers.Question.Done = true;
            if (SelectedAnswer != null)
            {
                if (SelectedAnswer.IsCorrect)
                    await _answerService.UpdateAsync(SelectedAnswer);
                else
                    await _answerService.CreateAsync(SelectedAnswer);

                string IDs = "";

                int i = 0;
                foreach (Answer answer in QuestionAnswers.Answers.FindAll(m => !m.Id.Equals(0)))
                {
                    if (i == 0)
                    {
                        IDs = answer.Id.ToString();
                        i++;
                    }
                    else
                    {
                        IDs += ',' + answer.Id.ToString();
                    }
                }
                QuestionAnswers.Question.AnswersID = IDs;
            }
            await _questionService.UpdateAsync(QuestionAnswers.Question);
            #endregion

            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(QuestionAnswers.Quizz.QuestionsID);
            Question question = questions.Find(m => m.Order.Equals(Order + 1));
            if (question != null)
            {
                QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
                List<Answer> answersCorrect = await _answerService.GetAllByAnswersIDAsync(question.AnswersID);
                List<Answer> answers = await _answerService.GenerateAnswers(QuestionAnswers.Quizz, questionType, answersCorrect);

                QuestionAnswers questionAnswers = new QuestionAnswers()
                {
                    Quizz = QuestionAnswers.Quizz,
                    Question = question,
                    Answers = answers
                };

                await Utils.RedirectQuizz(_navigation, questionAnswers, question, questionType);
            }
            else
            {
                QuestionAnswers.Quizz.Done = true;
                await _quizzService.UpdateAsync(QuestionAnswers.Quizz);
                await Utils.RedirectQuizz(_navigation, QuestionAnswers);
            }

            var refresh = new MessageRefresh(this, true);
            _messenger.Publish(refresh);

            await _navigation.Close(this);

            await UpdateUIGenerateAsync();
        }

        private async Task SelectedAnswerAsync(Answer answer)
        {
            if (SelectedAnswer != null)
                await ResetData();

            //Update Data
            answer.IsSelected = !answer.IsSelected;
            SelectedAnswer = answer;

            //Update UI
            await UpdateUIByOrder(answer);
        }

        private async Task UpdateUIGenerateAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleLoadingQuizz = !IsVisibleLoadingQuizz;
            });
        }
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask BackGroundTask { get; private set; }
        public MvxNotifyTask LoadQuestionAnswersTask { get; private set; }
        #endregion

        #region Data
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

        private string[] _formatQuestion;

        public string[] FormatQuestion
        {
            get { return _formatQuestion; }
            set { SetProperty(ref _formatQuestion, value); }
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

        private QuestionType _questionType;

        public QuestionType QuestionType
        {
            get { return _questionType; }
            set { SetProperty(ref _questionType, value); }
        }

        private Answer _selectedAnswer;

        public Answer SelectedAnswer
        {
            get { return _selectedAnswer; }
            set { _selectedAnswer = value; }
        }

        private byte[] _difficultyByte;

        public byte[] DifficultyByte
        {
            get { return _difficultyByte; }
            set { SetProperty(ref _difficultyByte, value); }
        }
        #endregion

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

        #region Transformation

        private List<ITransformation> _iTransformations;

        public List<ITransformation> Transformations
        {
            get { return _iTransformations; }
            set { SetProperty(ref _iTransformations, value); }
        }

        private int _delay;

        public int Delay
        {
            get { return _delay; }
            set { SetProperty(ref _delay, value); }
        }

        #endregion

        #region IsVisible
        private bool _isVisibleBackgroundModalFilter = false;

        public bool IsVisibleBackgroundModalFilter
        {
            get { return _isVisibleBackgroundModalFilter; }
            set { SetProperty(ref _isVisibleBackgroundModalFilter, value); }
        }

        private bool _isVisibleLoadingQuizz = false;

        public bool IsVisibleLoadingQuizz
        {
            get { return _isVisibleLoadingQuizz; }
            set { SetProperty(ref _isVisibleLoadingQuizz, value); }
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

        private bool _isEnabledAnswer7 = true;

        public bool IsEnabledAnswer7
        {
            get { return _isEnabledAnswer7; }
            set { SetProperty(ref _isEnabledAnswer7, value); }
        }

        private bool _isVisibleAnswer7 = true;

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

        private bool _isEnabledAnswer8 = true;

        public bool IsEnabledAnswer8
        {
            get { return _isEnabledAnswer8; }
            set { SetProperty(ref _isEnabledAnswer8, value); }
        }

        private bool _isVisibleAnswer8 = true;

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
        #endregion
        #endregion
    }
}
