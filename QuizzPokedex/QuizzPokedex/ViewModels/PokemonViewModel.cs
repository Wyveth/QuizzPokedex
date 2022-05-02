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
using Xamarin.Essentials;

namespace QuizzPokedex.ViewModels
{
    public class PokemonViewModel : MvxViewModel<Pokemon>
    {

        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;

        public PokemonViewModel(IMvxNavigationService navigation, IMvxIoCProvider logger, IPokemonService pokemonService, ITypePokService typeService)
        {
            _navigation = navigation;
            _logger = logger;
            _pokemonService = pokemonService;
            _typePokService = typeService;
        }

        public async override void Prepare(Pokemon pokemon)
        {
            Pokemon = pokemon;

            FamilyEvolution = await _pokemonService.GetFamilyWithoutVariantAsync(pokemon.Evolutions);
            MegaEvolution = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.MegaEvolution);
            GigamaxEvolution = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.GigaEvolution);
            AlolaVariant = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.Alola);
            GalarVariant = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.Galar);
            VarianteSexe = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.VarianteSexe);
            Variant = await _pokemonService.GetAllVariantAsync(pokemon.Number, Constantes.Variant);

            FirstType = await _typePokService.GetByIdAsync(pokemon.Types.Split(',')[0]);
            Types = await _typePokService.GetTypesAsync(pokemon.Types);
            Weakness = await _typePokService.GetTypesAsync(pokemon.Weakness);

            base.Prepare();
        }


        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }
        #endregion

        #region PROPERTIES
        private Pokemon _pokemon;
        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }

        private List<Pokemon> _familyEvolution;

        public List<Pokemon> FamilyEvolution
        {
            get { return _familyEvolution; }
            set { SetProperty(ref _familyEvolution, value); }
        }

        private List<Pokemon> _megaEvolution;

        public List<Pokemon> MegaEvolution
        {
            get { return _megaEvolution; }
            set { SetProperty(ref _megaEvolution, value); }
        }

        private List<Pokemon> _gigamaxEvolution;

        public List<Pokemon> GigamaxEvolution
        {
            get { return _gigamaxEvolution; }
            set { SetProperty(ref _gigamaxEvolution, value); }
        }

        private List<Pokemon> _alolaVariant;

        public List<Pokemon> AlolaVariant
        {
            get { return _alolaVariant; }
            set { SetProperty(ref _alolaVariant, value); }
        }

        private List<Pokemon> _galarVariant;

        public List<Pokemon> GalarVariant
        {
            get { return _galarVariant; }
            set { SetProperty(ref _galarVariant, value); }
        }

        private List<Pokemon> _varianteSexe;

        public List<Pokemon> VarianteSexe
        {
            get { return _varianteSexe; }
            set { SetProperty(ref _varianteSexe, value); }
        }

        private List<Pokemon> _variant;

        public List<Pokemon> Variant
        {
            get { return _variant; }
            set { SetProperty(ref _variant, value); }
        }

        private TypePok _firstType;

        public TypePok FirstType
        {
            get { return _firstType; }
            set { SetProperty(ref _firstType, value); }
        }

        private List<TypePok> _types;

        public List<TypePok> Types
        {
            get { return _types; }
            set { SetProperty(ref _types, value); }
        }

        private List<TypePok> _weakness;

        public List<TypePok> Weakness
        {
            get { return _weakness; }
            set { SetProperty(ref _weakness, value); }
        }
        #endregion
    }
}
