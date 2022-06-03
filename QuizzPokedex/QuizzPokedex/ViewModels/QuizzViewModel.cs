using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
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
    public class QuizzViewModel : MvxViewModel<Quizz>
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IQuizzService _quizzService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IPokemonService _pokemonService;

        public QuizzViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IQuizzService quizzService, IPokemonService pokemonService, IQuestionService questionService, IAnswerService answerService)
        {
            _navigation = navigation;
            _logger = logger;
            _quizzService = quizzService;
            _questionService = questionService;
            _answerService = answerService;
            _pokemonService = pokemonService;
        }

        public override void Prepare(Quizz quizz)
        {
            Quizz = quizz;

            base.Prepare();
        }

        public async override Task Initialize()
        {
            Pokemon = await _pokemonService.GetByNameAsync(Constantes.Charpenti);
            await base.Initialize();
        }

        #region Command
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand TestCommandAsync => new MvxAsyncCommand(TestAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task TestAsync()
        {
            Quizz quizz = await _quizzService.GenerateQuizz(1, false, false, false, false, false, false, false, false, false, false, false, false);

            List<Question> questions = await _questionService.GetAllByQuestionsIDAsync(quizz.QuestionsID);

            Dictionary<Question, List<Answer>> result = new Dictionary<Question, List<Answer>>();
            foreach (Question question in questions)
            {
                List<Answer> answers = new List<Answer>();
                answers.AddRange(await _answerService.GetAllByAnswersIDAsync(question.AnswersID));
                result.Add(question, answers);
            }

            await NavigationBackAsync();
        }
        #endregion

        #region Properties
        private Quizz _quizz;
        public Quizz Quizz
        {
            get { return _quizz; }
            set { SetProperty(ref _quizz, value); }
        }

        private Pokemon _pokemon;

        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }
        #endregion
    }
}
