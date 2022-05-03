﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Content.Res;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Services;
using QuizzPokedex.ViewModels;

namespace QuizzPokedex.CustomStart
{
    public class AppStart: MvxAppStart
    {
        private readonly ISqliteConnectionService _connectionService;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;

        public AppStart(IMvxApplication app, IMvxNavigationService mvxNavigationService, ISqliteConnectionService connectionService, IPokemonService pokemonService, ITypePokService typePokService)
            : base(app, mvxNavigationService)
        {
            _connectionService = connectionService;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
        }

        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            //initialisation des tables par défaut
            //_connectionService.GetAsyncConnection().DropTableAsync<TypePok>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Pokemon>().Wait();
            //_connectionService.GetAsyncConnection().DropTableAsync<Profile>().Wait();

            _connectionService.GetAsyncConnection().CreateTableAsync<TypePok>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Pokemon>().Wait();
            _connectionService.GetAsyncConnection().CreateTableAsync<Profile>().Wait();

            populateDb();

            return NavigationService.Navigate<WelcomeViewModel>();
        }

        protected async void populateDb()
        {
            int nbTypePok = await _typePokService.GetNumberAsync();

            await Task.Run(() =>
            {
                if (nbTypePok.Equals(0))
                    _typePokService.Populate();
            });

            int nbPokMax = _pokemonService.GetNumberPokJsonAsync();
            int nbPok = await _pokemonService.GetNumberInDbAsync();
            int nbPokNotUpdated = await _pokemonService.GetNumberPokUpdateAsync();

            await Task.Run(() =>
            {
                if(!nbPok.Equals(nbPokMax))
                    _pokemonService.Populate(nbPok);

                if (!nbPokNotUpdated.Equals(0))
                    _pokemonService.PopulateUpdateEvolution();
            });
        }
    }
}
