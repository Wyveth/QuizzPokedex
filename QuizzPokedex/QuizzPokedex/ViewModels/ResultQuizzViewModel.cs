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
                    //Answer answerCorrect = answers.Find(m => m.IsCorrect.Equals(true));
                    //Answer answerWrong = answers.Find(m => m.IsCorrect.Equals(false) && m.IsSelected.Equals(true));
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
                    IsVisibleSelectedNull = true;
                }
                else
                    IsVisibleSelectedNull = false;
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
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Steel_BW);
                    break;
                case Constantes.Fighting:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fighting_BW);
                    break;
                case Constantes.Dragon:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Dragon_BW);
                    break;
                case Constantes.Water:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Water_BW);
                    break;
                case Constantes.Electric:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Electric_BW);
                    break;
                case Constantes.Fairy:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fairy_BW);
                    break;
                case Constantes.Fire:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Fire_BW);
                    break;
                case Constantes.Ice:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ice_BW);
                    break;
                case Constantes.Bug:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Bug_BW);
                    break;
                case Constantes.Normal:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Normal_BW);
                    break;
                case Constantes.Grass:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Grass_BW);
                    break;
                case Constantes.Poison:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Poison_BW);
                    break;
                case Constantes.Psychic:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Psychic_BW);
                    break;
                case Constantes.Rock:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Rock_BW);
                    break;
                case Constantes.Ground:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ground_BW);
                    break;
                case Constantes.Ghost:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Ghost_BW);
                    break;
                case Constantes.Dark:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Dark_BW);
                    break;
                case Constantes.Flying:
                    typeByte = await Utils.GetByteAssetImage(Constantes.Icon_Flying_BW);
                    break;
            }
            #endregion
            return await Task.FromResult(typeByte);
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationValidationCommandAsync => new MvxAsyncCommand(NavigationValidationAsync);

        public IMvxAsyncCommand CloseModalCommandAsync => new MvxAsyncCommand(CloseModalAsync);

        public IMvxAsyncCommand<CorrectionQuizzSimple> CorrectionQuizzCommandAsync => new MvxAsyncCommand<CorrectionQuizzSimple>(CorrectionQuizzAsync);

        private async Task NavigationValidationAsync()
        {
            await _navigation.Close(this);
        }

        private async Task CorrectionQuizzAsync(CorrectionQuizzSimple correctionQuizz)
        {
            //await BackModalGenFilterAsync();
            //CorrectionQuizzSimple = correctionQuizz;
            //QuestionType = await _questionTypeService.GetByIdAsync(correctionQuizz.Question.QuestionTypeID);

            //if (QuestionType.Code.Equals(Constantes.QTypPok))
            //{
            //    IsVisiblePokemon = true;
            //    IsVisibleTypePok = false;

            //    if (correctionQuizz.CorrectAnswer != null)
            //    {
            //        Pokemon = await _pokemonService.GetByIdAsync(correctionQuizz.CorrectAnswer.IsCorrectID);
            //        TypePok = await _typePokService.GetByIdAsync(int.Parse(Pokemon.TypesID.Split(',')[0]));
            //        IsVisibleSelectedNull = true;
            //    }
            //    else
            //        IsVisibleSelectedNull = false;
            //}
            //else if(QuestionType.Code.Equals(Constantes.QTypTypPok) 
            //    || QuestionType.Code.Equals(Constantes.QTypTyp))
            //{
            //    IsVisiblePokemon = false;
            //    IsVisibleTypePok = true;

            //    if (correctionQuizz.CorrectAnswer != null)
            //    {
            //        TypePok = await _typePokService.GetByIdAsync(correctionQuizz.CorrectAnswer.IsCorrectID);
            //        await GetBytesTypesFilter(TypePok.Name);
            //        IsVisibleSelectedNull = true;
            //    }
            //    else
            //        IsVisibleSelectedNull = false;
            //}
        }

        private async Task CloseModalAsync()
        {
            await BackModalGenFilterAsync();
        }

        private async Task BackModalGenFilterAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModal = !IsVisibleBackgroundModal;
                IsVisibleModalCorrection = !IsVisibleModalCorrection;
            });
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

        #region Questions
        #region CorrectionQuizz 1
        private CorrectionQuizzSimple _CorrectionQuizz1;

        public CorrectionQuizzSimple CorrectionQuizz1
        {
            get { return _CorrectionQuizz1; }
            set { SetProperty(ref _CorrectionQuizz1, value); }
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

        #region CorrectionQuizz 2
        private CorrectionQuizzSimple _CorrectionQuizz2;

        public CorrectionQuizzSimple CorrectionQuizz2
        {
            get { return _CorrectionQuizz2; }
            set { SetProperty(ref _CorrectionQuizz2, value); }
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

        #region CorrectionQuizz 3
        private CorrectionQuizzSimple _CorrectionQuizz3;

        public CorrectionQuizzSimple CorrectionQuizz3
        {
            get { return _CorrectionQuizz3; }
            set { SetProperty(ref _CorrectionQuizz3, value); }
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

        #region CorrectionQuizz 4
        private CorrectionQuizzSimple correctionQuizz4;

        public CorrectionQuizzSimple CorrectionQuizz4
        {
            get { return correctionQuizz4; }
            set { SetProperty(ref correctionQuizz4, value); }
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

        #region CorrectionQuizz 5
        private CorrectionQuizzSimple correctionQuizz5;

        public CorrectionQuizzSimple CorrectionQuizz5
        {
            get { return correctionQuizz5; }
            set { SetProperty(ref correctionQuizz5, value); }
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

        #region CorrectionQuizz 6
        private CorrectionQuizzSimple correctionQuizz6;

        public CorrectionQuizzSimple CorrectionQuizz6
        {
            get { return correctionQuizz6; }
            set { SetProperty(ref correctionQuizz6, value); }
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

        #region CorrectionQuizz 7
        private CorrectionQuizzSimple correctionQuizz7;

        public CorrectionQuizzSimple CorrectionQuizz7
        {
            get { return correctionQuizz7; }
            set { SetProperty(ref correctionQuizz7, value); }
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

        #region CorrectionQuizz 8
        private CorrectionQuizzSimple correctionQuizz8;

        public CorrectionQuizzSimple CorrectionQuizz8
        {
            get { return correctionQuizz8; }
            set { SetProperty(ref correctionQuizz8, value); }
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

        #region CorrectionQuizz 9
        private CorrectionQuizzSimple correctionQuizz9;

        public CorrectionQuizzSimple CorrectionQuizz9
        {
            get { return correctionQuizz9; }
            set { SetProperty(ref correctionQuizz9, value); }
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

        #region CorrectionQuizz 10
        private CorrectionQuizzSimple correctionQuizz10;

        public CorrectionQuizzSimple CorrectionQuizz10
        {
            get { return correctionQuizz10; }
            set { SetProperty(ref correctionQuizz10, value); }
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

        #region CorrectionQuizz 11
        private CorrectionQuizzSimple correctionQuizz11;

        public CorrectionQuizzSimple CorrectionQuizz11
        {
            get { return correctionQuizz11; }
            set { SetProperty(ref correctionQuizz11, value); }
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

        #region CorrectionQuizz 12
        private CorrectionQuizzSimple correctionQuizz12;

        public CorrectionQuizzSimple CorrectionQuizz12
        {
            get { return correctionQuizz12; }
            set { SetProperty(ref correctionQuizz12, value); }
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

        #region CorrectionQuizz 13
        private CorrectionQuizzSimple correctionQuizz13;

        public CorrectionQuizzSimple CorrectionQuizz13
        {
            get { return correctionQuizz13; }
            set { SetProperty(ref correctionQuizz13, value); }
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

        #region CorrectionQuizz 14
        private CorrectionQuizzSimple correctionQuizz14;

        public CorrectionQuizzSimple CorrectionQuizz14
        {
            get { return correctionQuizz14; }
            set { SetProperty(ref correctionQuizz14, value); }
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

        #region CorrectionQuizz 15
        private CorrectionQuizzSimple correctionQuizz15;

        public CorrectionQuizzSimple CorrectionQuizz15
        {
            get { return correctionQuizz15; }
            set { SetProperty(ref correctionQuizz15, value); }
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

        #region CorrectionQuizz 16
        private CorrectionQuizzSimple correctionQuizz16;

        public CorrectionQuizzSimple CorrectionQuizz16
        {
            get { return correctionQuizz16; }
            set { SetProperty(ref correctionQuizz16, value); }
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

        #region CorrectionQuizz 17
        private CorrectionQuizzSimple correctionQuizz17;

        public CorrectionQuizzSimple CorrectionQuizz17
        {
            get { return correctionQuizz17; }
            set { SetProperty(ref correctionQuizz17, value); }
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

        #region CorrectionQuizz 18
        private CorrectionQuizzSimple correctionQuizz18;

        public CorrectionQuizzSimple CorrectionQuizz18
        {
            get { return correctionQuizz18; }
            set { SetProperty(ref correctionQuizz18, value); }
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

        #region CorrectionQuizz 19
        private CorrectionQuizzSimple correctionQuizz19;

        public CorrectionQuizzSimple CorrectionQuizz19
        {
            get { return correctionQuizz19; }
            set { SetProperty(ref correctionQuizz19, value); }
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

        #region CorrectionQuizz 20

        private CorrectionQuizzSimple correctionQuizz20;

        public CorrectionQuizzSimple CorrectionQuizz20
        {
            get { return correctionQuizz20; }
            set { SetProperty(ref correctionQuizz20, value); }
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

        #region Visibility

        private bool _isVisibleBackgroundModal = false;

        public bool IsVisibleBackgroundModal
        {
            get { return _isVisibleBackgroundModal; }
            set { SetProperty(ref _isVisibleBackgroundModal, value); }
        }

        private bool _isVisibleModalCorrection = false;

        public bool IsVisibleModalCorrection
        {
            get { return _isVisibleModalCorrection; }
            set { SetProperty(ref _isVisibleModalCorrection, value); }
        }

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

        private bool _isVisibleCorrect = false;

        public bool IsVisibleCorrect
        {
            get { return _isVisibleCorrect; }
            set { SetProperty(ref _isVisibleCorrect, value); }
        }

        private bool _isVisibleWrong = false;

        public bool IsVisibleWrong
        {
            get { return _isVisibleWrong; }
            set { SetProperty(ref _isVisibleWrong, value); }
        }

        private bool _isVisibleSelectedNull = false;

        public bool IsVisibleSelectedNull
        {
            get { return _isVisibleSelectedNull; }
            set { SetProperty(ref _isVisibleSelectedNull, value); }
        }
        #endregion
        #endregion
    }
}
