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
    public class QuizzViewModel : MvxViewModel<Quizz>
    {
        #region Field
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IQuizzService _quizzService;
        private readonly IProfileService _profileService;
        private readonly IQuestionService _questionService;
        private readonly IQuestionTypeService _questionTypeService;
        private readonly IAnswerService _answerService;
        private readonly IPokemonService _pokemonService;
        private readonly MvxSubscriptionToken _token;
        private readonly IMvxMessenger _messenger;
        #endregion

        #region Constructor
        public QuizzViewModel(IMvxNavigationService navigation, IMvxMessenger messenger, IMvxIoCProvider logger, IQuizzService quizzService, IProfileService profileService, IPokemonService pokemonService, IQuestionService questionService, IQuestionTypeService questionTypeService, IAnswerService answerService)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _profileService = profileService;
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
            _pokemonService = pokemonService;
            _token = messenger.Subscribe<MessageRefresh>(RefreshAsync);
            _messenger = messenger;
        }
        #endregion

        #region Public Methods
        public override void Prepare(Quizz quizz)
        {
            Quizz = quizz;
            base.Prepare();
        }

        public async override Task Initialize()
        {
            BackGroundTask = MvxNotifyTask.Create(BackGroundAsync);
            LoadQuizzTask = MvxNotifyTask.Create(LoadQuizzAsync);
            ProfileTask = MvxNotifyTask.Create(ProfileAsync);
            //InDevelopmentTask = MvxNotifyTask.Create(InDevelopmentAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task BackGroundAsync()
        {
            ImgPokedexUp = await Utils.GetByteAssetImage(Constantes.Pokedex_Up);
            ImgPokedexDown = await Utils.GetByteAssetImage(Constantes.Pokedex_Down);
        }

        private async Task LoadQuizzAsync()
        {
            ImgFilter = await Utils.GetByteAssetImage(Constantes.Filter);
            ImgResume = await Utils.GetByteAssetImage(Constantes.Resume);
            ImgEasy = Easy ? await Utils.GetByteAssetImage(Constantes.Easy_Color) : await Utils.GetByteAssetImage(Constantes.Easy_White);
            ImgNormal = Normal ? await Utils.GetByteAssetImage(Constantes.Normal_Color) : await Utils.GetByteAssetImage(Constantes.Normal_White);
            ImgHard = Hard ? await Utils.GetByteAssetImage(Constantes.Hard_Color) : await Utils.GetByteAssetImage(Constantes.Hard_White);
            await UpdateQuizzUnfinished();
        }

        private async Task ProfileAsync()
        {
            List<Profile> profiles = await _profileService.GetAllAsync();
            ActivatedProfile = profiles.Find(m => m.Activated.Equals(true));

            if (profiles.Count >= 1)
            {
                FirstProfileCreated = true;
                ActivatedPokemonProfile = await _pokemonService.GetByIdAsync(ActivatedProfile.PokemonID);
            }

            if (profiles.Count >= 2)
            {
                SecondProfileCreated = false;
                IsVisibleSecondProfile = false;
                IsVisibleThirdProfile = false;
            }

            if (profiles.Count == 3)
            {
                ThirdProfileCreated = false;
            }
        }

        private async Task UpdateQuizzUnfinished()
        {
            Profile profile = await _profileService.GetProfileActivatedAsync();
            List<Quizz> quizzs = await _quizzService.GetUnfinishedQuizzByProfile(profile.Id);

            List<QuizzDifficulty> quizzDifficulties = new List<QuizzDifficulty>();
            foreach (Quizz item in quizzs)
            {
                QuizzDifficulty quizzDifficulty = new QuizzDifficulty()
                {
                    Quizz = item,
                    ImgEasy = ImgEasy,
                    ImgNormal = await Utils.GetByteAssetImage(Constantes.Normal_Color),
                    ImgHard = await Utils.GetByteAssetImage(Constantes.Hard_Color),
                    ImgResume = ImgResume,
                    ResumeQuestion = await GetResume(item)
                };
                quizzDifficulties.Add(quizzDifficulty);
            }
            QuizzUnfinished = quizzDifficulties;
        }

        private async Task<string> GetResume(Quizz quizz)
        {
            string[] questionsID = quizz.QuestionsID.Split(',');
            int questionsDone = await _questionService.GetAllByQuestionsIDResumeAsync(questionsID);

            return await Task.FromResult(questionsDone.ToString() + "/" + questionsID.Length.ToString());
        }

        private async Task InDevelopmentAsync()
        {
            Pokemon = await _pokemonService.GetByNameAsync(Constantes.Charpenti);
        }

        private async Task ResumeQuizzAsync(QuizzDifficulty quizzDifficulty)
        {
            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(quizzDifficulty.Quizz.QuestionsID);
            Question question = questions.Find(m => m.Done.Equals(false));
            QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
            List<Answer> answers = await _answerService.GenerateAnswers(quizzDifficulty.Quizz, questionType, await _answerService.GetAllByAnswersIDAsync(question.AnswersID));

            QuestionAnswers questionAnswers = new QuestionAnswers()
            {
                Quizz = quizzDifficulty.Quizz,
                Question = question,
                Answers = answers
            };

            await Utils.RedirectQuizz(_navigation, questionAnswers, question, questionType);
        }

        private async void RefreshAsync(MessageRefresh msg)
        {
            if (msg.Refresh)
            {
                await LoadQuizzAsync();
                await ProfileAsync();
            }
        }
        #endregion

        #region Command
        #region Command Navigation
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand NavigationProfileCommandAsync => new MvxAsyncCommand(NavigationProfileAsync);
        #endregion

        #region Command Quizz
        public IMvxAsyncCommand ModalFilterCommandAsync => new MvxAsyncCommand(ModalFilterAsync);

        public IMvxAsyncCommand BackModalGenFilterCommandAsync => new MvxAsyncCommand(BackModalGenFilterAsync);

        public IMvxAsyncCommand EasyQuizzCommandAsync => new MvxAsyncCommand(EasyQuizzAsync);

        public IMvxAsyncCommand NormalQuizzCommandAsync => new MvxAsyncCommand(NormalQuizzAsync);

        public IMvxAsyncCommand HardQuizzCommandAsync => new MvxAsyncCommand(HardQuizzAsync);

        public IMvxAsyncCommand GenerateQuizzCommandAsync => new MvxAsyncCommand(GenerateQuizzAsync);
        #endregion

        #region Command Profile
        public IMvxAsyncCommand ShowHideOtherProfileCommandAsync => new MvxAsyncCommand(ShowHideOtherProfileAsync);
        public IMvxAsyncCommand ActivatedProfileLongCommandAsync => new MvxAsyncCommand(ActivatedProfileLongAsync);

        public IMvxAsyncCommand<Profile> OpenModalChangeProfileCommandAsync => new MvxAsyncCommand<Profile>(OpenModalChangeProfileAsync);

        public IMvxAsyncCommand CloseModalChangeProfileCommandAsync => new MvxAsyncCommand(CloseModalChangeProfileAsync);

        public IMvxAsyncCommand ChangeProfileCommandAsync => new MvxAsyncCommand(ChangeProfileAsync);
        #endregion

        #region Command Gen Filter
        public IMvxAsyncCommand FilterByGen1CommandAsync => new MvxAsyncCommand(FilterByGen1Async);
        public IMvxAsyncCommand FilterByGen2CommandAsync => new MvxAsyncCommand(FilterByGen2Async);
        public IMvxAsyncCommand FilterByGen3CommandAsync => new MvxAsyncCommand(FilterByGen3Async);
        public IMvxAsyncCommand FilterByGen4CommandAsync => new MvxAsyncCommand(FilterByGen4Async);
        public IMvxAsyncCommand FilterByGen5CommandAsync => new MvxAsyncCommand(FilterByGen5Async);
        public IMvxAsyncCommand FilterByGen6CommandAsync => new MvxAsyncCommand(FilterByGen6Async);
        public IMvxAsyncCommand FilterByGen7CommandAsync => new MvxAsyncCommand(FilterByGen7Async);
        public IMvxAsyncCommand FilterByGen8CommandAsync => new MvxAsyncCommand(FilterByGen8Async);
        public IMvxAsyncCommand FilterByGenArceusCommandAsync => new MvxAsyncCommand(FilterByGenArceusAsync);
        #endregion

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task EasyQuizzAsync()
        {
            if (Easy)
            {
                Easy = !Easy;
                ImgEasy = await Utils.GetByteAssetImage(Constantes.Easy_White);
            }
            else
            {
                Easy = !Easy;
                ImgEasy = await Utils.GetByteAssetImage(Constantes.Easy_Color);
            }
        }

        private async Task NormalQuizzAsync()
        {
            if (Normal)
            {
                Normal = !Normal;
                ImgNormal = await Utils.GetByteAssetImage(Constantes.Normal_White);
            }
            else
            {
                Normal = !Normal;
                ImgNormal = await Utils.GetByteAssetImage(Constantes.Normal_Color);
            }
        }

        private async Task HardQuizzAsync()
        {
            if (Hard)
            {
                Hard = !Hard;
                ImgHard = await Utils.GetByteAssetImage(Constantes.Hard_White);
            }
            else
            {
                Hard = !Hard;
                ImgHard = await Utils.GetByteAssetImage(Constantes.Hard_Color);
            }
        }

        private async Task GenerateQuizzAsync()
        {
            await UpdateUIGenerateAsync();

            Profile profile = await _profileService.GetProfileActivatedAsync();

            Task<Quizz> TaskQuizz = _quizzService.GenerateQuizz(profile, FiltreActiveGen1, FiltreActiveGen2, FiltreActiveGen3, FiltreActiveGen4, FiltreActiveGen5, FiltreActiveGen6, FiltreActiveGen7, FiltreActiveGen8, FiltreActiveGenArceus, Easy, Normal, Hard);
            await ProgressCreation();

            Quizz quizz = await TaskQuizz;
            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(quizz.QuestionsID);
            Question question = questions.Find(m => m.Order.Equals(1));
            QuestionType questionType = await _questionTypeService.GetByIdAsync(question.QuestionTypeID);
            List<Answer> answers = await _answerService.GenerateAnswers(quizz, questionType, await _answerService.GetAllByAnswersIDAsync(question.AnswersID));

            QuestionAnswers questionAnswers = new QuestionAnswers() {
                Quizz = quizz,
                Question = question,
                Answers = answers
            };

            await Utils.RedirectQuizz(_navigation, questionAnswers, question, questionType);

            await UpdateUIGenerateAsync();
        }

        private async Task ProgressCreation()
        {
            int nbMaxQuestion = await _questionService.GetNbQuestionByDifficulty(Easy, Normal, Hard);
            int nbQuestionBefore = await _questionService.GetCountAsync();
            int nbQuestionAfter = nbQuestionBefore + nbMaxQuestion;
            int dif = 0;
            ProgressGenerate = "Création en cours: " + dif + "/" + nbMaxQuestion;
            while (dif != nbMaxQuestion)
            {
                await Task.Delay(1000);
                nbQuestionAfter = await _questionService.GetCountAsync();
                dif = nbQuestionAfter - nbQuestionBefore;
                ProgressGenerate = "Création en cours: " + dif + "/" + nbMaxQuestion;
            }
        }

        #region Filter By Gen
        private async Task FilterByGen1Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen1)
                {
                    FiltreActiveGen1 = false;
                    BackgroundColorGen1 = Constantes.WhiteHexa;
                    TextColorGen1 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen1 = true;
                    BackgroundColorGen1 = Constantes.BlackHexa;
                    TextColorGen1 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen2Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen2)
                {
                    FiltreActiveGen2 = false;
                    BackgroundColorGen2 = Constantes.WhiteHexa;
                    TextColorGen2 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen2 = true;
                    BackgroundColorGen2 = Constantes.BlackHexa;
                    TextColorGen2 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen3Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen3)
                {
                    FiltreActiveGen3 = false;
                    BackgroundColorGen3 = Constantes.WhiteHexa;
                    TextColorGen3 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen3 = true;
                    BackgroundColorGen3 = Constantes.BlackHexa;
                    TextColorGen3 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen4Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen4)
                {
                    FiltreActiveGen4 = false;
                    BackgroundColorGen4 = Constantes.WhiteHexa;
                    TextColorGen4 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen4 = true;
                    BackgroundColorGen4 = Constantes.BlackHexa;
                    TextColorGen4 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen5Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen5)
                {
                    FiltreActiveGen5 = false;
                    BackgroundColorGen5 = Constantes.WhiteHexa;
                    TextColorGen5 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen5 = true;
                    BackgroundColorGen5 = Constantes.BlackHexa;
                    TextColorGen5 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen6Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen6)
                {
                    FiltreActiveGen6 = false;
                    BackgroundColorGen6 = Constantes.WhiteHexa;
                    TextColorGen6 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen6 = true;
                    BackgroundColorGen6 = Constantes.BlackHexa;
                    TextColorGen6 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen7Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen7)
                {
                    FiltreActiveGen7 = false;
                    BackgroundColorGen7 = Constantes.WhiteHexa;
                    TextColorGen7 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen7 = true;
                    BackgroundColorGen7 = Constantes.BlackHexa;
                    TextColorGen7 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGen8Async()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGen8)
                {
                    FiltreActiveGen8 = false;
                    BackgroundColorGen8 = Constantes.WhiteHexa;
                    TextColorGen8 = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGen8 = true;
                    BackgroundColorGen8 = Constantes.BlackHexa;
                    TextColorGen8 = Constantes.WhiteHexa;
                }
            });
        }

        private async Task FilterByGenArceusAsync()
        {
            await Task.Run(() =>
            {
                if (FiltreActiveGenArceus)
                {
                    FiltreActiveGenArceus = false;
                    BackgroundColorGenArceus = Constantes.WhiteHexa;
                    TextColorGenArceus = Constantes.BlackHexa;
                }
                else
                {
                    FiltreActiveGenArceus = true;
                    BackgroundColorGenArceus = Constantes.BlackHexa;
                    TextColorGenArceus = Constantes.WhiteHexa;
                }
            });
        }
        #endregion

        private async Task UpdateUIGenerateAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleLoadingQuizz = !IsVisibleLoadingQuizz;
                IsEnabledGenerate = !IsEnabledGenerate;
            });
        }

        private async Task BackModalGenFilterAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalFilter = !IsVisibleModalFilter;
            });
        }

        private async Task ModalFilterAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalFilter = !IsVisibleModalFilter;
            });
        }

        #region Profile
        private async Task NavigationProfileAsync()
        {
            await _navigation.Navigate<ProfileViewModel, Profile>(new Profile());
        }

        private async Task ShowHideOtherProfileAsync()
        {
            List<Profile> profiles = await _profileService.GetAllAsync();
            List<Profile> profileNotActivated = profiles.FindAll(m => m.Activated.Equals(false));

            if (profiles.Count >= 1)
            {
                ActivatedProfile = profiles.Find(m => m.Activated.Equals(true));
                ActivatedPokemonProfile = await _pokemonService.GetByIdAsync(ActivatedProfile.PokemonID);
                IsVisibleSecondProfile = !IsVisibleSecondProfile;
            }

            if (profiles.Count >= 2)
            {
                SecondProfileCreated = !SecondProfileCreated;
                NotActivatedFirstProfile = profileNotActivated[0];
                NotActivatedPokemonFirstProfile = await _pokemonService.GetByIdAsync(profileNotActivated[0].PokemonID);
                IsVisibleThirdProfile = !IsVisibleThirdProfile;
            }

            if (profiles.Count == 3)
            {
                ThirdProfileCreated = !ThirdProfileCreated;
                NotActivatedSecondProfile = profileNotActivated[1];
                NotActivatedPokemonSecondProfile = await _pokemonService.GetByIdAsync(profileNotActivated[1].PokemonID);
            }
        }

        private async Task ActivatedProfileLongAsync()
        {
            await Task.Run(() =>
            {
            });
        }

        private async Task OpenModalChangeProfileAsync(Profile profile)
        {
            SelectedProfileChange = profile;
            MsgChangeProfile = "Veux-tu activer le profil de " + profile.Name + "?";
            await ShowHideModal();
        }

        private async Task CloseModalChangeProfileAsync()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalChangeProfile = !IsVisibleModalChangeProfile;
                SelectedProfileChange = null;
            });
        }

        private async Task ChangeProfileAsync()
        {
            await _profileService.UpdateProfileActivatedAsync(SelectedProfileChange);
            SelectedProfileChange = null;

            var refresh = new MessageRefresh(this, true);
            _messenger.Publish(refresh);

            await ShowHideModal();
        }

        private async Task ShowHideModal()
        {
            await Task.Run(() =>
            {
                IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
                IsVisibleModalChangeProfile = !IsVisibleModalChangeProfile;
            });
        }
        #endregion
        #endregion

        #region Properties
        #region Collection
        public MvxNotifyTask BackGroundTask { get; private set; }
        public MvxNotifyTask LoadQuizzTask { get; private set; }
        public MvxNotifyTask ProfileTask { get; private set; }
        public MvxNotifyTask InDevelopmentTask { get; private set; }
        #endregion

        #region Data
        private Quizz _quizz;
        public Quizz Quizz
        {
            get { return _quizz; }
            set { SetProperty(ref _quizz, value); }
        }

        private List<QuizzDifficulty> _quizzUnfinished;
        public List<QuizzDifficulty> QuizzUnfinished
        {
            get { return _quizzUnfinished; }
            set { SetProperty(ref _quizzUnfinished, value); }
        }

        private Pokemon _pokemon;

        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }

        private string _progressGenerate;

        public string ProgressGenerate
        {
            get { return _progressGenerate; }
            set { SetProperty(ref _progressGenerate, value); }
        }

        #region Data
        private QuizzDifficulty _selectedQuizzUnfinished;
        public QuizzDifficulty SelectedQuizzUnfinished
        {
            get { return _selectedQuizzUnfinished; }
            set
            {
                _selectedQuizzUnfinished = value;
                _ = ResumeQuizzAsync(_selectedQuizzUnfinished);
            }
        }
        #endregion
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

        #region Difficulté
        private bool _easy = true;

        public bool Easy
        {
            get { return _easy; }
            set { SetProperty(ref _easy, value); }
        }

        private byte[] _imgEasy;

        public byte[] ImgEasy
        {
            get { return _imgEasy; }
            set { SetProperty(ref _imgEasy, value); }
        }

        private bool _normal = false;

        public bool Normal
        {
            get { return _normal; }
            set { SetProperty(ref _normal, value); }
        }

        private byte[] _imgNormal;

        public byte[] ImgNormal
        {
            get { return _imgNormal; }
            set { SetProperty(ref _imgNormal, value); }
        }

        private bool _hard = false;

        public bool Hard
        {
            get { return _hard; }
            set { SetProperty(ref _hard, value); }
        }

        private byte[] _imgHard;

        public byte[] ImgHard
        {
            get { return _imgHard; }
            set { SetProperty(ref _imgHard, value); }
        }
        #endregion

        #region Visibility Filter
        private bool _isEnabledGenerate = true;

        public bool IsEnabledGenerate
        {
            get { return _isEnabledGenerate; }
            set { SetProperty(ref _isEnabledGenerate, value); }
        }

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

        private bool _isVisibleModalFilter = false;

        public bool IsVisibleModalFilter
        {
            get { return _isVisibleModalFilter; }
            set { SetProperty(ref _isVisibleModalFilter, value); }
        }

        private bool _isVisibleModalChangeProfile = false;

        public bool IsVisibleModalChangeProfile
        {
            get { return _isVisibleModalChangeProfile; }
            set { SetProperty(ref _isVisibleModalChangeProfile, value); }
        }
        #endregion

        #region Profile
        private string _msgChangeProfile;

        public string MsgChangeProfile
        {
            get { return _msgChangeProfile; }
            set { SetProperty(ref _msgChangeProfile, value); }
        }

        private bool _firstProfileCreated = false;

        public bool FirstProfileCreated
        {
            get { return _firstProfileCreated; }
            set { SetProperty(ref _firstProfileCreated, value); }
        }

        private bool _secondProfileCreated = false;

        public bool SecondProfileCreated
        {
            get { return _secondProfileCreated; }
            set { SetProperty(ref _secondProfileCreated, value); }
        }

        private bool _isVisibleSecondProfile = false;

        public bool IsVisibleSecondProfile
        {
            get { return _isVisibleSecondProfile; }
            set { SetProperty(ref _isVisibleSecondProfile, value); }
        }

        private bool _thirdProfileCreated = false;

        public bool ThirdProfileCreated
        {
            get { return _thirdProfileCreated; }
            set { SetProperty(ref _thirdProfileCreated, value); }
        }

        private bool _isVisibleThirdProfile = false;

        public bool IsVisibleThirdProfile
        {
            get { return _isVisibleThirdProfile; }
            set { SetProperty(ref _isVisibleThirdProfile, value); }
        }

        private Profile _selectedProfileChange;

        public Profile SelectedProfileChange
        {
            get { return _selectedProfileChange; }
            set { SetProperty(ref _selectedProfileChange, value); }
        }

        private Profile _activatedProfile;

        public Profile ActivatedProfile
        {
            get { return _activatedProfile; }
            set { SetProperty(ref _activatedProfile, value); }
        }

        private Pokemon _activatedPokemonProfile;

        public Pokemon ActivatedPokemonProfile
        {
            get { return _activatedPokemonProfile; }
            set { SetProperty(ref _activatedPokemonProfile, value); }
        }

        private Profile _notActivatedFirstProfile;

        public Profile NotActivatedFirstProfile
        {
            get { return _notActivatedFirstProfile; }
            set { SetProperty(ref _notActivatedFirstProfile, value); }
        }

        private Pokemon _notActivatedPokemonFirstProfile;

        public Pokemon NotActivatedPokemonFirstProfile
        {
            get { return _notActivatedPokemonFirstProfile; }
            set { SetProperty(ref _notActivatedPokemonFirstProfile, value); }
        }

        private Profile _notActivatedSecondProfile;

        public Profile NotActivatedSecondProfile
        {
            get { return _notActivatedSecondProfile; }
            set { SetProperty(ref _notActivatedSecondProfile, value); }
        }

        private Pokemon _notActivatedPokemonSecondProfile;

        public Pokemon NotActivatedPokemonSecondProfile
        {
            get { return _notActivatedPokemonSecondProfile; }
            set { SetProperty(ref _notActivatedPokemonSecondProfile, value); }
        }
        #endregion

        #region Filter
        private byte[] _imgFilter;

        public byte[] ImgFilter
        {
            get { return _imgFilter; }
            set { SetProperty(ref _imgFilter, value); }
        }

        private byte[] _imgResume;

        public byte[] ImgResume
        {
            get { return _imgResume; }
            set { SetProperty(ref _imgResume, value); }
        }
        #endregion

        #region Filter Gen
        #region Generation 1
        private bool _filtreActiveGen1 = false;

        public bool FiltreActiveGen1
        {
            get { return _filtreActiveGen1; }
            set { SetProperty(ref _filtreActiveGen1, value); }
        }

        private string _backgroundColorGen1 = "#FFFFFF";

        public string BackgroundColorGen1
        {
            get { return _backgroundColorGen1; }
            set { SetProperty(ref _backgroundColorGen1, value); }
        }

        private string _textColorGen1 = "#000000";

        public string TextColorGen1
        {
            get { return _textColorGen1; }
            set { SetProperty(ref _textColorGen1, value); }
        }
        #endregion

        #region Generation 2
        private bool _filtreActiveGen2 = false;

        public bool FiltreActiveGen2
        {
            get { return _filtreActiveGen2; }
            set { SetProperty(ref _filtreActiveGen2, value); }
        }

        private string _backgroundColorGen2 = "#FFFFFF";

        public string BackgroundColorGen2
        {
            get { return _backgroundColorGen2; }
            set { SetProperty(ref _backgroundColorGen2, value); }
        }

        private string _textColorGen2 = "#000000";

        public string TextColorGen2
        {
            get { return _textColorGen2; }
            set { SetProperty(ref _textColorGen2, value); }
        }
        #endregion

        #region Generation 3
        private bool _filtreActiveGen3 = false;

        public bool FiltreActiveGen3
        {
            get { return _filtreActiveGen3; }
            set { SetProperty(ref _filtreActiveGen3, value); }
        }

        private string _backgroundColorGen3 = "#FFFFFF";

        public string BackgroundColorGen3
        {
            get { return _backgroundColorGen3; }
            set { SetProperty(ref _backgroundColorGen3, value); }
        }

        private string _textColorGen3 = "#000000";

        public string TextColorGen3
        {
            get { return _textColorGen3; }
            set { SetProperty(ref _textColorGen3, value); }
        }
        #endregion

        #region Generation 4
        private bool _filtreActiveGen4 = false;

        public bool FiltreActiveGen4
        {
            get { return _filtreActiveGen4; }
            set { SetProperty(ref _filtreActiveGen4, value); }
        }

        private string _backgroundColorGen4 = "#FFFFFF";

        public string BackgroundColorGen4
        {
            get { return _backgroundColorGen4; }
            set { SetProperty(ref _backgroundColorGen4, value); }
        }
        private string _textColorGen4 = "#000000";

        public string TextColorGen4
        {
            get { return _textColorGen4; }
            set { SetProperty(ref _textColorGen4, value); }
        }
        #endregion

        #region Generation 5
        private bool _filtreActiveGen5 = false;

        public bool FiltreActiveGen5
        {
            get { return _filtreActiveGen5; }
            set { SetProperty(ref _filtreActiveGen5, value); }
        }

        private string _backgroundColorGen5 = "#FFFFFF";

        public string BackgroundColorGen5
        {
            get { return _backgroundColorGen5; }
            set { SetProperty(ref _backgroundColorGen5, value); }
        }
        private string _textColorGen5 = "#000000";

        public string TextColorGen5
        {
            get { return _textColorGen5; }
            set { SetProperty(ref _textColorGen5, value); }
        }
        #endregion

        #region Generation 6
        private bool _filtreActiveGen6 = false;

        public bool FiltreActiveGen6
        {
            get { return _filtreActiveGen6; }
            set { SetProperty(ref _filtreActiveGen6, value); }
        }

        private string _backgroundColorGen6 = "#FFFFFF";

        public string BackgroundColorGen6
        {
            get { return _backgroundColorGen6; }
            set { SetProperty(ref _backgroundColorGen6, value); }
        }
        private string _textColorGen6 = "#000000";

        public string TextColorGen6
        {
            get { return _textColorGen6; }
            set { SetProperty(ref _textColorGen6, value); }
        }
        #endregion

        #region Generation 7
        private bool _filtreActiveGen7 = false;

        public bool FiltreActiveGen7
        {
            get { return _filtreActiveGen7; }
            set { SetProperty(ref _filtreActiveGen7, value); }
        }

        private string _backgroundColorGen7 = "#FFFFFF";

        public string BackgroundColorGen7
        {
            get { return _backgroundColorGen7; }
            set { SetProperty(ref _backgroundColorGen7, value); }
        }
        private string _textColorGen7 = "#000000";

        public string TextColorGen7
        {
            get { return _textColorGen7; }
            set { SetProperty(ref _textColorGen7, value); }
        }
        #endregion

        #region Generation 8
        private bool _filtreActiveGen8 = false;

        public bool FiltreActiveGen8
        {
            get { return _filtreActiveGen8; }
            set { SetProperty(ref _filtreActiveGen8, value); }
        }

        private string _backgroundColorGen8 = "#FFFFFF";

        public string BackgroundColorGen8
        {
            get { return _backgroundColorGen8; }
            set { SetProperty(ref _backgroundColorGen8, value); }
        }
        private string _textColorGen8 = "#000000";

        public string TextColorGen8
        {
            get { return _textColorGen8; }
            set { SetProperty(ref _textColorGen8, value); }
        }
        #endregion

        #region Generation Arceus
        private bool _filtreActiveGenArceus = false;

        public bool FiltreActiveGenArceus
        {
            get { return _filtreActiveGenArceus; }
            set { SetProperty(ref _filtreActiveGenArceus, value); }
        }

        private string _backgroundColorGenArceus = "#FFFFFF";

        public string BackgroundColorGenArceus
        {
            get { return _backgroundColorGenArceus; }
            set { SetProperty(ref _backgroundColorGenArceus, value); }
        }
        private string _textColorGenArceus = "#000000";

        public string TextColorGenArceus
        {
            get { return _textColorGenArceus; }
            set { SetProperty(ref _textColorGenArceus, value); }
        }
        #endregion
        #endregion
        #endregion
    }
}
