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
            List<CorrectionQuizzSimple> correctionQuizz = new List<CorrectionQuizzSimple>();
            await Task.Run(async () =>
            {
                List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(QuestionAnswers.Quizz.QuestionsID);
                List<Answer> answersCorrect = new List<Answer>();
                foreach (Question question in questions)
                {
                    List<Answer> answers = await _answerService.GetAllByAnswersIDAsync(question.AnswersID);
                    Answer answerIsCorrect = answers.Find(m => m.IsCorrect.Equals(true) && m.IsSelected.Equals(true));
                    correctionQuizz.Add(await LoadCorrectionQuizz(question, answers, answerIsCorrect));
                    if (answerIsCorrect != null)
                        answersCorrect.Add(answerIsCorrect);
                }
                Result = answersCorrect.Count + "/" + questions.Count;
            });

            CorrectionQuizz = new MvxObservableCollection<CorrectionQuizzSimple>(correctionQuizz);
        }

        private async Task<CorrectionQuizzSimple> LoadCorrectionQuizz(Question question, List<Answer> answers, Answer answerIsCorrect)
        {
            Answer answerCorrect = answers.Find(m => m.IsCorrect.Equals(true));
            Answer answerWrong = answers.Find(m => m.IsCorrect.Equals(false) && m.IsSelected.Equals(true));
            QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
            Pokemon pokemon = null;
            TypePok typePok = null;
            byte[] typePokByte = null;

            if (questionType.Code.Equals(Constantes.QTypPok))
            {
                IsVisiblePokemon = true;
                IsVisibleTypePok = false;

                if (answerCorrect != null)
                {
                    pokemon = await _pokemonService.GetByIdAsync(answerCorrect.IsCorrectID);
                    typePok = await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0]));
                }
            }
            else if (questionType.Code.Equals(Constantes.QTypTypPok))
            {
                IsVisiblePokemon = true;
                IsVisibleTypePok = false;

                if (answerCorrect != null)
                {
                    pokemon = await _pokemonService.GetByIdAsync(question.DataObjectID);
                    typePok = await _typePokService.GetByIdAsync(answerCorrect.IsCorrectID);
                }
            }
            else if (questionType.Code.Equals(Constantes.QTypTyp))
            {
                IsVisiblePokemon = false;
                IsVisibleTypePok = true;

                if (answerCorrect != null)
                {
                    typePok = await _typePokService.GetByIdAsync(answerCorrect.IsCorrectID);
                    typePokByte = await GetBytesTypesFilter(typePok.Name);
                }
            }

            CorrectionQuizzSimple correctionQuizz = new CorrectionQuizzSimple()
            {
                Question = question,
                CorrectAnswer = answerCorrect,
                WrongAnswer = answerWrong,
                QuestionType = questionType,
                TypePok = typePok,
                ByteTypePok = typePokByte,
                Pokemon = pokemon,
                IsQTypPok = IsVisiblePokemon,
                IsQTypTyp = IsVisibleTypePok,
                ByteResult = await GetByteImgAnswer(answerCorrect)
            };

            return await Task.FromResult(correctionQuizz);
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
            if(answer.IsSelected)
                return await Utils.GetByteAssetImage(Constantes.StarSuccess);
            else
                return await Utils.GetByteAssetImage(Constantes.StarWrong);
        }

        private async Task<byte[]> GetBytesTypesFilter(string Name)
        {
            byte[] typeByte = null;
            #region Type Filter
            switch (Name)
            {
                case Constantes.Steel:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Steel);
                    break;
                case Constantes.Fighting:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fighting);
                    break;
                case Constantes.Dragon:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Dragon);
                    break;
                case Constantes.Water:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Water);
                    break;
                case Constantes.Electric:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Electric);
                    break;
                case Constantes.Fairy:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fairy);
                    break;
                case Constantes.Fire:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fire);
                    break;
                case Constantes.Ice:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ice);
                    break;
                case Constantes.Bug:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Bug);
                    break;
                case Constantes.Normal:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Normal);
                    break;
                case Constantes.Grass:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Grass);
                    break;
                case Constantes.Poison:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Poison);
                    break;
                case Constantes.Psychic:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Psychic);
                    break;
                case Constantes.Rock:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Rock);
                    break;
                case Constantes.Ground:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ground);
                    break;
                case Constantes.Ghost:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ghost);
                    break;
                case Constantes.Dark:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Dark);
                    break;
                case Constantes.Flying:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Flying);
                    break;
            }
            #endregion
            return await Task.FromResult(typeByte);
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

        private MvxObservableCollection<CorrectionQuizzSimple> _correctionQuizz;

        public MvxObservableCollection<CorrectionQuizzSimple> CorrectionQuizz
        {
            get { return _correctionQuizz; }
            set
            {
                SetProperty(ref _correctionQuizz, value);
                RaisePropertyChanged(() => CorrectionQuizz);
            }
        }

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

        private CorrectionQuizzSimple _correctionQuizzSimple;

        public CorrectionQuizzSimple CorrectionQuizzSimple
        {
            get { return _correctionQuizzSimple; }
            set { SetProperty(ref _correctionQuizzSimple, value); }
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

        #region Visibility
        private bool _isVisiblePokemon = false;

        public bool IsVisiblePokemon
        {
            get { return _isVisiblePokemon; }
            set { SetProperty(ref _isVisiblePokemon, value); }
        }

        private bool _isVisibleTypePok = false;

        public bool IsVisibleTypePok
        {
            get { return _isVisibleTypePok; }
            set { SetProperty(ref _isVisibleTypePok, value); }
        }
        #endregion
        #endregion
    }
}
