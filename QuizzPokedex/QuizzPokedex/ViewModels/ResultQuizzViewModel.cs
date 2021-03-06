using Microcharts;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
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
        private readonly ITalentService _talentService;
        private readonly IMvxMessenger _messenger;
        #endregion

        #region Constructor
        public ResultQuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IDifficultyService difficultyService, IAnswerService answerService, IQuestionTypeService questionTypeService, ITypePokService typePokService, ITalentService talentService, IMvxMessenger messenger)
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
            _talentService = talentService;
            _messenger = messenger;
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
            List<CorrectionQuizzSimple> correctionQuizz = new List<CorrectionQuizzSimple>();
            await Task.Run(async () =>
            {
                List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(QuestionAnswers.Quizz.QuestionsID);
                int answersCorrectCount = 0;

                foreach (Question question in questions)
                {
                    QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
                    List<Answer> answers = await _answerService.GetAllByAnswersIDAsync(question.AnswersID);
                    
                    if (!questionType.IsMultipleAnswers)
                    {
                        Answer answerIsCorrect = answers.Find(m => m.IsCorrect.Equals(true) && m.IsSelected.Equals(true));
                        Answer answerCorrect = answers.Find(m => m.IsCorrect.Equals(true));
                        correctionQuizz.Add(await LoadCorrectionQuizz(question, questionType, answerCorrect));
                        if (answerIsCorrect != null)
                            answersCorrectCount++;
                    }
                    else
                    {
                        List<Answer> answersIsCorrectSelected = answers.FindAll(m => m.IsCorrect.Equals(true) && m.IsSelected.Equals(true));
                        List<Answer> answersIsNotCorrect = answers.FindAll(m => m.IsCorrect.Equals(false) && m.IsSelected.Equals(true));
                        List<Answer> answersIsCorrect = answers.FindAll(m => m.IsCorrect.Equals(true));

                        correctionQuizz.Add(await LoadCorrectionQuizz(question, questionType, null, answersIsCorrectSelected, answersIsNotCorrect, answersIsCorrect));

                        if (answersIsCorrectSelected.Count.Equals(answersIsCorrect.Count) && answersIsNotCorrect.Count.Equals(0))
                            answersCorrectCount++;
                        
                    }
                }
                Result = answersCorrectCount.ToString() + "/" + questions.Count;
            });

            CorrectionQuizz = new MvxObservableCollection<CorrectionQuizzSimple>(correctionQuizz);
        }

        private async Task<CorrectionQuizzSimple> LoadCorrectionQuizz(Question question, QuestionType questionType, Answer answerCorrect = null, List<Answer> answersIsCorrectSelected = null, List<Answer> answersIsNotCorrect = null, List<Answer> answersIsCorrect = null)
        {
            Pokemon pokemon = null;
            TypePok typePok = null;
            Talent talent = null;
            byte[] typePokByte = null;
            byte[] byteResult = null;
            string libelleAnswerCorrect = "";

            if (!questionType.IsMultipleAnswers)
            {
                if (questionType.Code.Equals(Constantes.QTypPok)
                    || questionType.Code.Equals(Constantes.QTypPokBlurred)
                    || questionType.Code.Equals(Constantes.QTypPokBlack))
                {
                    ResetIsVisible();
                    IsVisiblePok = true;

                    if (answerCorrect != null)
                    {
                        pokemon = await _pokemonService.GetByIdAsync(answerCorrect.IsCorrectID);
                        typePok = await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0]));
                    }
                }
                else if (questionType.Code.Equals(Constantes.QTypPokStat))
                {
                    ResetIsVisible();
                    IsVisiblePokStat = true;

                    if (answerCorrect != null)
                    {
                        pokemon = await _pokemonService.GetByIdAsync(answerCorrect.IsCorrectID);
                        typePok = await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0]));

                        FormatLibelleQuestion = new string[] { answerCorrect.Type, pokemon.Name };
                    }
                }
                else if (questionType.Code.Equals(Constantes.QTypTypPok))
                {
                    ResetIsVisible();
                    IsVisibleTypPok = true;

                    if (answerCorrect != null)
                    {
                        pokemon = await _pokemonService.GetByIdAsync(question.DataObjectID);
                        typePok = await _typePokService.GetByIdAsync(answerCorrect.IsCorrectID);

                        FormatLibelleQuestion = new string[] { pokemon.Name };
                    }
                }
                else if (questionType.Code.Equals(Constantes.QTypTyp))
                {
                    ResetIsVisible();
                    IsVisibleTyp = true;

                    if (answerCorrect != null)
                    {
                        typePok = await _typePokService.GetByIdAsync(answerCorrect.IsCorrectID);
                        typePokByte = await GetBytesTypesFilter(typePok.Name);
                    }
                }
                else if (questionType.Code.Equals(Constantes.QTypPokDesc)
                    || questionType.Code.Equals(Constantes.QTypPokDescReverse))
                {
                    ResetIsVisible();
                    IsVisiblePokDesc = true;

                    if (answerCorrect != null)
                    {
                        pokemon = await _pokemonService.GetByIdAsync(answerCorrect.IsCorrectID);
                        typePok = await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0]));
                    }
                }
                else if (questionType.Code.Equals(Constantes.QTypTalent)
                    || questionType.Code.Equals(Constantes.QTypTalentReverse))
                {
                    ResetIsVisible();
                    IsVisibleTalent = true;
                    DetectiveP = await Utils.GetByteAssetImage(Constantes.DetectivePikachu);

                    if (answerCorrect != null)
                    {
                        talent = await _talentService.GetByIdAsync(answerCorrect.IsCorrectID);
                    }
                }

                libelleAnswerCorrect = answerCorrect.Libelle;
                byteResult = await GetByteImgAnswer(answerCorrect.IsSelected);
            }
            else
            {
                if (questionType.Code.Equals(Constantes.QTypPokFamily)){

                }
                else if(questionType.Code.Equals(Constantes.QTypPokTyp))
                {

                }
                else if (questionType.Code.Equals(Constantes.QTypTypPokVarious)
                    || questionType.Code.Equals(Constantes.QTypWeakPokVarious))
                {
                    ResetIsVisible();
                    IsVisibleTypPokVarious = true;

                    pokemon = await _pokemonService.GetByIdAsync(question.DataObjectID);
                    typePok = await _typePokService.GetByIdAsync(int.Parse(pokemon.TypesID.Split(',')[0]));

                    FormatLibelleQuestion = new string[] { pokemon.Name };
                }

                libelleAnswerCorrect = await GetAnswerLibelle(answersIsCorrect);

                if (answersIsCorrectSelected.Count.Equals(answersIsCorrect.Count) && answersIsNotCorrect.Count.Equals(0))
                    byteResult = await GetByteImgAnswer(true);
                else
                    byteResult = await GetByteImgAnswer(false);
            }

            CorrectionQuizzSimple correctionQuizz = new CorrectionQuizzSimple()
            {
                Question = question,
                CorrectAnswer = libelleAnswerCorrect,
                QuestionType = questionType,
                TypePok = typePok,
                ByteTypePok = typePokByte,
                Pokemon = pokemon,
                Talent = talent,
                ByteDetectiveP = DetectiveP,
                IsQTypPok = IsVisiblePok,
                IsQTypTyp = IsVisibleTyp,
                IsQTypPokStat = IsVisiblePokStat,
                IsQTypPokDesc = IsVisiblePokDesc,
                IsQTypTypPok = IsVisibleTypPok,
                IsQTypTalent = IsVisibleTalent,
                IsQTypTypPokVarious = IsVisibleTypPokVarious,
                ByteResult = byteResult,
                FormatLibelleQuestion = FormatLibelleQuestion
            };

            return await Task.FromResult(correctionQuizz);
        }

        private void ResetIsVisible()
        {
            #region Single
            IsVisiblePok = false;
            IsVisibleTyp = false;
            IsVisiblePokStat = false;
            IsVisiblePokDesc = false;
            IsVisibleTypPok = false;
            IsVisibleTalent = false;
            #endregion

            #region Multiple
            IsVisibleTypPokVarious = false;
            #endregion
        }

        private async Task<byte[]> GetByteImgAnswer(bool answerCorrectIsSelected)
        {
            if (answerCorrectIsSelected)
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

        private async Task<string> GetAnswerLibelle(List<Answer> answers)
        {
            string libelle = "";
            int i = 0;
            foreach (Answer item in answers)
            {
                if (i == 0) {
                    libelle = item.Libelle;
                    i++;
                }
                else
                {
                    libelle += ", " + item.Libelle;
                }
            }

            return await Task.FromResult(libelle);
        }
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationValidationCommandAsync => new MvxAsyncCommand(NavigationValidationAsync);

        private async Task NavigationValidationAsync()
        {
            await _navigation.Close(this);
        }

        private async Task RefreshAsync()
        {
            var refresh = new MessageRefresh(this, true);
            _messenger.Publish(refresh);

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

        private byte[] _detectiveP;

        public byte[] DetectiveP
        {
            get { return _detectiveP; }
            set { SetProperty(ref _detectiveP, value); }
        }

        private string[] _formatLibelleQuestion;

        public string[] FormatLibelleQuestion
        {
            get { return _formatLibelleQuestion; }
            set { SetProperty(ref _formatLibelleQuestion, value); }
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
        #region Single Answers
        private bool _isVisiblePok = false;

        public bool IsVisiblePok
        {
            get { return _isVisiblePok; }
            set { SetProperty(ref _isVisiblePok, value); }
        }

        private bool _isVisibleTyp = false;

        public bool IsVisibleTyp
        {
            get { return _isVisibleTyp; }
            set { SetProperty(ref _isVisibleTyp, value); }
        }

        private bool _isVisiblePokStat = false;

        public bool IsVisiblePokStat
        {
            get { return _isVisiblePokStat; }
            set { SetProperty(ref _isVisiblePokStat, value); }
        }

        private bool _isVisibleTypPok = false;

        public bool IsVisibleTypPok
        {
            get { return _isVisibleTypPok; }
            set { SetProperty(ref _isVisibleTypPok, value); }
        }

        private bool _isVisiblePokDesc = false;

        public bool IsVisiblePokDesc
        {
            get { return _isVisiblePokDesc; }
            set { SetProperty(ref _isVisiblePokDesc, value); }
        }

        private bool _isVisibleTalent = false;

        public bool IsVisibleTalent
        {
            get { return _isVisibleTalent; }
            set { SetProperty(ref _isVisibleTalent, value); }
        }
        #endregion

        #region Multiple Answer
        //private bool _isVisibleFamily = false;

        //public bool IsVisibleFamily
        //{
        //    get { return _isVisibleFamily; }
        //    set { SetProperty(ref _isVisibleFamily, value); }
        //}

        private bool _isVisibleTypPokVarious = false;

        public bool IsVisibleTypPokVarious
        {
            get { return _isVisibleTypPokVarious; }
            set { SetProperty(ref _isVisibleTypPokVarious, value); }
        }
        #endregion
        #endregion
        #endregion
    }
}
