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

            if (!string.IsNullOrEmpty(Pokemon.Talent))
            {
                TalentIsVisible = true;
                if (Pokemon.Talent.Contains(",") && Pokemon.DescriptionTalent.Contains(";"))
                {
                    SecondTalentIsVisible = true;
                    string[] talentTab = Pokemon.Talent.Split(',');
                    TalentOne = talentTab[0];
                    TalentTwo = talentTab[1];
                    string[] descTalentTab = Pokemon.DescriptionTalent.Split(';');
                    TalentDescriptionOne = descTalentTab[0];
                    TalentDescriptionTwo = descTalentTab[1];
                }
                else
                {
                    TalentOne = Pokemon.Talent;
                    TalentDescriptionOne = Pokemon.DescriptionTalent;
                    SecondTalentIsVisible = false;
                }
            }
            else
            {
                TalentIsVisible=false;
            }

            if (!pokemon.Updated)
                pokemon = await _pokemonService.UpdateEvolutionWithJson(Pokemon);

            base.Prepare();
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }

        private async Task LoadPokemonAsync()
        {
            var resultFamilyEvolution = await _pokemonService.GetFamilyWithoutVariantAsync(Pokemon.Evolutions);
            FamilyEvolIsVisible = GetVisible(resultFamilyEvolution.Count);
            CountFamilyEvol = GetNbSpan(resultFamilyEvolution.Count);//Check
            HeightFamilyEvol = GetHeightSection(resultFamilyEvolution.Count);//Check
            FamilyEvolution = new MvxObservableCollection<Pokemon>(resultFamilyEvolution);

            var resultMegaEvolution = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.MegaEvolution);
            if (resultMegaEvolution.Count != 0)
                MegaEvolIsVisible = true;
            MegaEvolution = new MvxObservableCollection<Pokemon>(resultMegaEvolution);

            var resultGigamaxEvolution = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.GigaEvolution);
            if (resultGigamaxEvolution.Count != 0)
                GigaEvolIsVisible = true;
            GigamaxEvolution = new MvxObservableCollection<Pokemon>(resultGigamaxEvolution);

            var resultAlolaVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Alola);
            if (resultAlolaVariant.Count != 0)
                AlolalIsVisible = true;
            AlolaVariant = new MvxObservableCollection<Pokemon>(resultAlolaVariant);

            var resultGalarVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Galar);
            if (resultGalarVariant.Count != 0)
                GalarIsVisible = true;
            GalarVariant = new MvxObservableCollection<Pokemon>(resultGalarVariant);

            var resultHisuiVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Hisui);
            if (resultHisuiVariant.Count != 0)
                HisuiIsVisible = true;
            HisuiVariant = new MvxObservableCollection<Pokemon>(resultHisuiVariant);

            var resultVarianteSexe = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.VarianteSexe);
            if (resultVarianteSexe.Count != 0)
                VarianteSexeIsVisible = true;
            VarianteSexe = new MvxObservableCollection<Pokemon>(resultVarianteSexe);

            var resultVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Variant);
            if (resultVariant.Count != 0)
                VariantIsVisible = true;
            Variant = new MvxObservableCollection<Pokemon>(resultVariant);

            FirstType = await _typePokService.GetByIdAsync(Pokemon.Types.Split(',')[0]);
            SetColor(FirstType);

            var resultTypes = await _typePokService.GetTypesAsync(Pokemon.Types);
            Types = new MvxObservableCollection<TypePok>(resultTypes);

            var resultWeakness = await _typePokService.GetTypesAsync(Pokemon.Weakness);
            Weakness = new MvxObservableCollection<TypePok>(resultWeakness);
        }

        private bool GetVisible(int count)
        {
            if (count != 0)
                return true;
            else
                return false;
        }

        private int GetNbSpan(int count)
        {
            if (count <= 2)
                return count;
            else
                return 3;
        }

        private int GetHeightSection(int count)
        {
            if (count <= 3)
                return 150;
            else if (count <= 6)
                return 280;
            else if (count <= 9)
                return 420;
            else
                return 150;
        }

        private void SetColor(TypePok typePok)
        {
            switch (typePok.Name)
            {
                case Constantes.Steel:
                    ImgColor = Constantes.ImgColorSteel;
                    InfoColor = Constantes.InfoColorSteel;
                    break;
                case Constantes.Fighting:
                    ImgColor = Constantes.ImgColorFighting;
                    InfoColor = Constantes.InfoColorFighting;
                    break;
                case Constantes.Dragon:
                    ImgColor = Constantes.ImgColorDragon;
                    InfoColor = Constantes.InfoColorDragon;
                    break;
                case Constantes.Water:
                    ImgColor = Constantes.ImgColorWater;
                    InfoColor = Constantes.InfoColorWater;
                    break;
                case Constantes.Electric:
                    ImgColor = Constantes.ImgColorElectric;
                    InfoColor = Constantes.InfoColorElectric;
                    break;
                case Constantes.Fairy:
                    ImgColor = Constantes.ImgColorFairy;
                    InfoColor = Constantes.InfoColorFairy;
                    break;
                case Constantes.Fire:
                    ImgColor = Constantes.ImgColorFire;
                    InfoColor = Constantes.InfoColorFire;
                    break;
                case Constantes.Ice:
                    ImgColor = Constantes.ImgColorIce;
                    InfoColor = Constantes.InfoColorIce;
                    break;
                case Constantes.Bug:
                    ImgColor = Constantes.ImgColorBug;
                    InfoColor = Constantes.InfoColorBug;
                    break;
                case Constantes.Normal:
                    ImgColor = Constantes.ImgColorNormal;
                    InfoColor = Constantes.InfoColorNormal;
                    break;
                case Constantes.Grass:
                    ImgColor = Constantes.ImgColorGrass;
                    InfoColor = Constantes.InfoColorGrass;
                    break;
                case Constantes.Poison:
                    ImgColor = Constantes.ImgColorPoison;
                    InfoColor = Constantes.InfoColorPoison;
                    break;
                case Constantes.Psychic:
                    ImgColor = Constantes.ImgColorPsychic;
                    InfoColor = Constantes.InfoColorPsychic;
                    break;
                case Constantes.Rock:
                    ImgColor = Constantes.Rock;
                    InfoColor = Constantes.InfoColorRock;
                    break;
                case Constantes.Ground:
                    ImgColor = Constantes.ImgColorGround;
                    InfoColor = Constantes.InfoColorGround;
                    break;
                case Constantes.Ghost:
                    ImgColor = Constantes.ImgColorGhost;
                    InfoColor = Constantes.InfoColorGhost;
                    break;
                case Constantes.Dark:
                    ImgColor = Constantes.ImgColorDark;
                    InfoColor = Constantes.InfoColorDark;
                    break;
                case Constantes.Flying:
                    ImgColor = Constantes.ImgColorSteel;
                    InfoColor = Constantes.InfoColorSteel;
                    break;
            }
        }

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }
        #endregion

        #region PROPERTIES
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        #region Liste
        private Pokemon _pokemon;
        public Pokemon Pokemon
        {
            get { return _pokemon; }
            set { SetProperty(ref _pokemon, value); }
        }

        private MvxObservableCollection<Pokemon> _familyEvolution;

        public MvxObservableCollection<Pokemon> FamilyEvolution
        {
            get { return _familyEvolution; }
            set { SetProperty(ref _familyEvolution, value); }
        }

        private MvxObservableCollection<Pokemon> _megaEvolution;

        public MvxObservableCollection<Pokemon> MegaEvolution
        {
            get { return _megaEvolution; }
            set { SetProperty(ref _megaEvolution, value); }
        }

        private MvxObservableCollection<Pokemon> _gigamaxEvolution;

        public MvxObservableCollection<Pokemon> GigamaxEvolution
        {
            get { return _gigamaxEvolution; }
            set { SetProperty(ref _gigamaxEvolution, value); }
        }

        private MvxObservableCollection<Pokemon> _alolaVariant;

        public MvxObservableCollection<Pokemon> AlolaVariant
        {
            get { return _alolaVariant; }
            set { SetProperty(ref _alolaVariant, value); }
        }

        private MvxObservableCollection<Pokemon> _galarVariant;

        public MvxObservableCollection<Pokemon> GalarVariant
        {
            get { return _galarVariant; }
            set { SetProperty(ref _galarVariant, value); }
        }

        private MvxObservableCollection<Pokemon> _hisuiVariant;

        public MvxObservableCollection<Pokemon> HisuiVariant
        {
            get { return _hisuiVariant; }
            set { SetProperty(ref _hisuiVariant, value); }
        }

        private MvxObservableCollection<Pokemon> _varianteSexe;

        public MvxObservableCollection<Pokemon> VarianteSexe
        {
            get { return _varianteSexe; }
            set { SetProperty(ref _varianteSexe, value); }
        }

        private MvxObservableCollection<Pokemon> _variant;

        public MvxObservableCollection<Pokemon> Variant
        {
            get { return _variant; }
            set { SetProperty(ref _variant, value); }
        }

        private MvxObservableCollection<TypePok> _types;

        public MvxObservableCollection<TypePok> Types
        {
            get { return _types; }
            set { SetProperty(ref _types, value); }
        }

        private MvxObservableCollection<TypePok> _weakness;

        public MvxObservableCollection<TypePok> Weakness
        {
            get { return _weakness; }
            set { SetProperty(ref _weakness, value); }
        }
        #endregion

        #region Data
        private TypePok _firstType;

        public TypePok FirstType
        {
            get { return _firstType; }
            set { SetProperty(ref _firstType, value); }
        }

        private string _talentOne;

        public string TalentOne
        {
            get { return _talentOne; }
            set { _talentOne = value; }
        }

        private string _talentDescriptionOne;

        public string TalentDescriptionOne
        {
            get { return _talentDescriptionOne; }
            set { _talentDescriptionOne = value; }
        }

        private string _talentTwo;

        public string TalentTwo
        {
            get { return _talentTwo; }
            set { _talentTwo = value; }
        }

        private string _talentDescriptionTwo;

        public string TalentDescriptionTwo
        {
            get { return _talentDescriptionTwo; }
            set { _talentDescriptionTwo = value; }
        }
        #endregion

        #region Affichage Span List
        private int _countFamilyEvol;

        public int CountFamilyEvol
        {
            get { return _countFamilyEvol; }
            set { SetProperty(ref _countFamilyEvol, value); }
        }

        private int _heightFamilyEvol;

        public int HeightFamilyEvol
        {
            get { return _heightFamilyEvol; }
            set { SetProperty(ref _heightFamilyEvol, value); }
        }

        private int _countVariantEvol;

        public int CountVariantEvol
        {
            get { return _countVariantEvol; }
            set { SetProperty(ref _countVariantEvol, value); }
        }

        private int _heightVariantEvol;

        public int HeightVariantEvol
        {
            get { return _heightVariantEvol; }
            set { SetProperty(ref _heightVariantEvol, value); }
        }
        #endregion

        #region IsVisible
        private bool _talentIsVisible;

        public bool TalentIsVisible
        {
            get { return _talentIsVisible; }
            set { SetProperty(ref _talentIsVisible, value); }
        }

        private bool _2ndTalentIsVisible;

        public bool SecondTalentIsVisible
        {
            get { return _2ndTalentIsVisible; }
            set { SetProperty(ref _2ndTalentIsVisible, value); }
        }

        private bool _familyEvolIsVisible;
        public bool FamilyEvolIsVisible
        {
            get { return _familyEvolIsVisible; }
            set { SetProperty(ref _familyEvolIsVisible, value); }
        }

        private bool _megaEvolIsVisible;
        public bool MegaEvolIsVisible
        {
            get { return _megaEvolIsVisible; }
            set { SetProperty(ref _megaEvolIsVisible, value); }
        }

        private bool _gigaEvolIsVisible;
        public bool GigaEvolIsVisible
        {
            get { return _gigaEvolIsVisible; }
            set { SetProperty(ref _gigaEvolIsVisible, value); }
        }

        private bool _alolaIsVisible;
        public bool AlolalIsVisible
        {
            get { return _alolaIsVisible; }
            set { SetProperty(ref _alolaIsVisible, value); }
        }

        private bool _galarlIsVisible;
        public bool GalarIsVisible
        {
            get { return _galarlIsVisible; }
            set { SetProperty(ref _galarlIsVisible, value); }
        }

        private bool _hisuiIsVisible;
        public bool HisuiIsVisible
        {
            get { return _hisuiIsVisible; }
            set { SetProperty(ref _hisuiIsVisible, value); }
        }

        private bool _variantIsVisible;
        public bool VariantIsVisible
        {
            get { return _variantIsVisible; }
            set { SetProperty(ref _variantIsVisible, value); }
        }

        private bool _varianteSexeIsVisible;
        public bool VarianteSexeIsVisible
        {
            get { return _varianteSexeIsVisible; }
            set { SetProperty(ref _varianteSexeIsVisible, value); }
        }
        #endregion

        #region Color
        private string _imgColor;

        public string ImgColor
        {
            get { return _imgColor; }
            set { SetProperty(ref _imgColor, value); }
        }

        private string _infoColor;

        public string InfoColor
        {
            get { return _infoColor; }
            set { SetProperty(ref _infoColor, value); }
        }
        #endregion
        #endregion
    }
}
