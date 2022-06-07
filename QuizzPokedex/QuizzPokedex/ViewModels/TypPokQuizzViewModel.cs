using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
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
        private readonly IAnswerService _answerService;
        private readonly IPokemonService _pokemonService;
        #endregion

        #region Constructor
        public TypPokQuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IAnswerService answerService, IQuestionTypeService questionTypeService)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _questionService = questionService;
            _questionTypeService = questionTypeService;
            _answerService = answerService;
            _pokemonService = pokemonService;
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
            LoadQuestionAnswersTask = MvxNotifyTask.Create(LoadQuestionAnswersAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task LoadQuestionAnswersAsync()
        {
            Question = QuestionAnswers.Question;

            QuestionType questionType = await _questionTypeService.GetByIdAsync(Question.QuestionTypeID);
            QuestionLibelle = questionType.Libelle;

            Pokemon = await _pokemonService.GetByIdAsync(QuestionAnswers.Answers.Find(m => m.IsCorrect.Equals(true)).IsCorrectID);

            AnswerOne = QuestionAnswers.Answers[0];
            AnswerTwo = QuestionAnswers.Answers[1];
            AnswerThree = QuestionAnswers.Answers[2];
            AnswerFour = QuestionAnswers.Answers[3];
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

        private string _questionLibelle;

        public string QuestionLibelle
        {
            get { return _questionLibelle; }
            set { SetProperty(ref _questionLibelle, value); }
        }

        private Answer _answerOne;

        public Answer AnswerOne
        {
            get { return _answerOne; }
            set { SetProperty(ref _answerOne, value); }
        }

        private Answer _answerTwo;

        public Answer AnswerTwo
        {
            get { return _answerTwo; }
            set { SetProperty(ref _answerTwo, value); }
        }

        private Answer _answerThree;

        public Answer AnswerThree
        {
            get { return _answerThree; }
            set { SetProperty(ref _answerThree, value); }
        }

        private Answer _answerFour;

        public Answer AnswerFour
        {
            get { return _answerFour; }
            set { SetProperty(ref _answerFour, value); }
        }
        #endregion
    }
}
