using Microcharts;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using Plugin.SimpleAudioPlayer;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using SkiaSharp;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace QuizzPokedex.ViewModels
{
    public class PokemonViewModel : MvxViewModel<Pokemon>
    {
        #region Fields
        private readonly IMvxNavigationService _navigation;
        private readonly IMvxIoCProvider _logger;
        private readonly IPokemonService _pokemonService;
        private readonly ITalentService _talentService;
        private readonly ITypePokService _typePokService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMvxMessenger _messenger;
        #endregion

        #region Constructor
        public PokemonViewModel(IMvxNavigationService navigation, IMvxMessenger messenger, IMvxIoCProvider logger, IPokemonService pokemonService, ITalentService talentService, ITypePokService typeService, IFavoriteService favoriteService)
        {
            _navigation = navigation;
            _logger = logger;
            _pokemonService = pokemonService;
            _talentService = talentService;
            _typePokService = typeService;
            _favoriteService = favoriteService;
            _messenger = messenger;
        }
        #endregion

        #region Public Methods
        public override void Prepare(Pokemon pokemon)
        {
            Pokemon = pokemon;

            if (!Pokemon.StatTotal.Equals(0))
                StatisticIsVisible = true;
            else
                StatisticIsVisible = false;

            ChartStats = CreateChartStats(pokemon);

            base.Prepare();
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }
        #endregion

        #region Private Methods
        private async Task LoadPokemonAsync()
        {
            if (!Pokemon.Updated)
                await _pokemonService.UpdateEvolutionWithJson(Pokemon);

            #region Favorite
            IsFavorite = await _favoriteService.CheckIfFavoriteExist(Pokemon);
            if(IsFavorite)
                ImgFavorite = await Utils.GetByteAssetImage(Constantes.LoveFull);
            else
                ImgFavorite = await Utils.GetByteAssetImage(Constantes.Love);
            #endregion

            #region Talent
            if (!string.IsNullOrEmpty(Pokemon.TalentsID))
            {
                TalentIsVisible = true;
                if (Pokemon.TalentsID.Contains(","))
                {

                    SecondTalentIsVisible = true;
                    string[] talentIds = Pokemon.TalentsID.Split(',');

                    FirstTalent = await _talentService.GetByIdAsync(int.Parse(talentIds[0]));
                    SecondTalent = await _talentService.GetByIdAsync(int.Parse(talentIds[1]));
                }
                else
                {
                    FirstTalent = await _talentService.GetByIdAsync(int.Parse(Pokemon.TalentsID));
                    SecondTalentIsVisible = false;
                }
            }
            else
                TalentIsVisible = false;
            #endregion

            #region Type
            int typeID = int.Parse(Pokemon.TypesID.Split(',')[0]);
            FirstType = await _typePokService.GetByIdAsync(typeID);

            var resultTypes = await _typePokService.GetTypesAsync(Pokemon.TypesID);
            Types = new MvxObservableCollection<TypePok>(resultTypes);
            #endregion

            #region Family Evolution
            var resultFamilyEvolution = await _pokemonService.GetFamilyWithoutVariantAsync(Pokemon.Evolutions);
            FamilyEvolIsVisible = await GetVisible(resultFamilyEvolution.Count);
            CountFamilyEvol = await GetNbSpan(resultFamilyEvolution.Count);
            HeightFamilyEvol = await GetHeightSection(resultFamilyEvolution.Count);
            FamilyEvolution = new MvxObservableCollection<Pokemon>(resultFamilyEvolution);
            #endregion

            #region Mega Evolution
            var resultMegaEvolution = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.MegaEvolution);
            MegaEvolIsVisible = await GetVisible(resultMegaEvolution.Count);
            MegaEvolution = new MvxObservableCollection<Pokemon>(resultMegaEvolution);
            #endregion

            #region Gigamax Evolution
            var resultGigamaxEvolution = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.GigaEvolution);
            GigaEvolIsVisible = await GetVisible(resultGigamaxEvolution.Count);
            GigamaxEvolution = new MvxObservableCollection<Pokemon>(resultGigamaxEvolution);
            #endregion

            #region Alola Variant
            var resultAlolaVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Alola);
            AlolalIsVisible = await GetVisible(resultAlolaVariant.Count);
            AlolaVariant = new MvxObservableCollection<Pokemon>(resultAlolaVariant);
            #endregion

            #region Galar Variant
            var resultGalarVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Galar);
            GalarIsVisible = await GetVisible(resultGalarVariant.Count);
            GalarVariant = new MvxObservableCollection<Pokemon>(resultGalarVariant);
            #endregion

            #region Hisui Variant
            var resultHisuiVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Hisui);
            HisuiIsVisible = await GetVisible(resultHisuiVariant.Count);
            HisuiVariant = new MvxObservableCollection<Pokemon>(resultHisuiVariant);
            #endregion

            #region Paldea Variant
            var resultPaldeaVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Paldea);
            PaldeaIsVisible = await GetVisible(resultPaldeaVariant.Count);
            PaldeaVariant = new MvxObservableCollection<Pokemon>(resultPaldeaVariant);
            #endregion

            #region Sexe Variant
            var resultVarianteSexe = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.VarianteSexe);
            VarianteSexeIsVisible = await GetVisible(resultVarianteSexe.Count);
            VarianteSexe = new MvxObservableCollection<Pokemon>(resultVarianteSexe);
            #endregion

            #region Variant
            var resultVariant = await _pokemonService.GetAllVariantAsync(Pokemon.Number, Constantes.Variant);
            VariantIsVisible = await GetVisible(resultVariant.Count);
            CountVariantEvol = await GetNbSpan(resultVariant.Count);
            HeightVariantEvol = await GetHeightSectionVariant(resultVariant.Count);
            Variant = new MvxObservableCollection<Pokemon>(resultVariant);
            #endregion

            #region Weakness
            var resultWeakness = await _typePokService.GetTypesAsync(Pokemon.WeaknessID);
            HeightWeakness = await GetHeightSectionWeakness(resultWeakness.Count);
            CountWeakness = await GetNbSpan(resultWeakness.Count);
            Weakness = new MvxObservableCollection<TypePok>(resultWeakness);
            #endregion
        }

        #region Display/Size Variable + Chart
        private Chart CreateChartStats(Pokemon pokemon)
        {
            ChartEntry[] entries = GenerateEntriesChart(pokemon);

            return new RadarChart
            {
                Entries = entries,
                LabelTextSize = 42,
                LineSize = 5,
                PointSize = 20,
                BorderLineSize = 5,
                AnimationProgress = 7,
                AnimationDuration = TimeSpan.FromSeconds(7),
                IsAnimated = true,
                MaxValue = 255,
                PointMode = PointMode.Circle
            };
        }

        private ChartEntry[] GenerateEntriesChart(Pokemon pokemon)
        {
            return new[]
            {
                new ChartEntry(pokemon.StatPv)
                {
                    Label = "PV",
                    ValueLabel = pokemon.StatPv.ToString(),
                    Color = SKColor.Parse("#6BC563"),
                },
                new ChartEntry(pokemon.StatAttaque)
                {
                    Label = "Att.",
                    ValueLabel = pokemon.StatAttaque.ToString(),
                    Color = SKColor.Parse("#FFA75F"),
                },
                new ChartEntry(pokemon.StatDefense)
                {
                    Label = "Def.",
                    ValueLabel = pokemon.StatDefense.ToString(),
                    Color = SKColor.Parse("#579BE1"),
                },
                new ChartEntry(pokemon.StatAttaqueSpe)
                {
                    Label = "Att. Sp.",
                    ValueLabel = pokemon.StatAttaqueSpe.ToString(),
                    Color = SKColor.Parse("#F999F1"),
                },
                new ChartEntry(pokemon.StatDefenseSpe)
                {
                    Label = "Def. Sp.",
                    ValueLabel = pokemon.StatDefenseSpe.ToString(),
                    Color = SKColor.Parse("#FF7A7F"),
                },
                new ChartEntry(pokemon.StatVitesse)
                {
                    Label = "Vit.",
                    ValueLabel = pokemon.StatVitesse.ToString(),
                    Color = SKColor.Parse("#FDDD46"),
                }
            };
        }

        private async Task<bool> GetVisible(int count)
        {
            if (count != 0)
                return await Task.FromResult(true);
            else
                return await Task.FromResult(false);
        }

        private async Task<int> GetNbSpan(int count)
        {
            if (count <= 2)
                return await Task.FromResult(count);
            else
                return await Task.FromResult(3);
        }

        private async Task<int> GetHeightSection(int count)
        {
            if (Pokemon.Name.Contains(Constantes.Ningale) 
                || Pokemon.Name.Contains(Constantes.Ninjask) 
                || Pokemon.Name.Contains(Constantes.Munja))
                return await Task.FromResult(200);
            if (count <= 3)
                return await Task.FromResult(180);
            else if (count <= 6)
                return await Task.FromResult(340);
            else if (count <= 9)
                return await Task.FromResult(500);
            else
                return await Task.FromResult(180);
        }

        private async Task<int> GetHeightSectionVariant(int count)
        {
            if (Pokemon.Name.Contains(Constantes.Prismillon))
                return await Task.FromResult(320);
            else if (count <= 3)
                return await Task.FromResult(180);
            else if (count <= 6)
                return await Task.FromResult(290);
            else if (count <= 9)
                return await Task.FromResult(500);
            else
                return await Task.FromResult(180);
        }

        private async Task<int> GetHeightSectionWeakness(int count)
        {
            if (count <= 3)
                return await Task.FromResult(40);
            else if (count <= 6)
                return await Task.FromResult(70);
            else if (count <= 9)
                return await Task.FromResult(100);
            else
                return await Task.FromResult(40);
        }
        #endregion
        #endregion

        #region Command
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand FavoriteCommandAsync => new MvxAsyncCommand(FavoriteAsync);
        public IMvxAsyncCommand<Pokemon> DetailsPokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DetailsPokemonAsync);
        public IMvxAsyncCommand PlaySoundCommandAsync => new MvxAsyncCommand(PlaySoundAsync);

        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task FavoriteAsync()
        {
            if (IsFavorite)
            {
                IsFavorite = false;
                ImgFavorite = await Utils.GetByteAssetImage(Constantes.Love);

                Favorite favorite = await _favoriteService.GetFavorite(Pokemon);
                await _favoriteService.DeleteAsync(favorite);
            }
            else
            {
                IsFavorite = true;
                ImgFavorite = await Utils.GetByteAssetImage(Constantes.LoveFull);
                Favorite favorite = new Favorite()
                {
                    PokemonID = Pokemon.Id
                };
                await _favoriteService.CreateAsync(favorite);
            }

            var refresh = new MessageRefresh(this, true);
            _messenger.Publish(refresh);
        }

        private async Task DetailsPokemonAsync(Pokemon Pokemon)
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(Pokemon);
        }

        private async Task<bool> PlaySoundAsync()
        {
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + Pokemon.PathSound.Replace("/", "."));
                var audio = CrossSimpleAudioPlayer.Current;
                audio.Load(audioStream);
                audio.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return await Task.FromResult(true);
        }
        #endregion

        #region Properties
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        #region Collection
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

        private MvxObservableCollection<Pokemon> _paldeaVariant;

        public MvxObservableCollection<Pokemon> PaldeaVariant
        {
            get { return _paldeaVariant; }
            set { SetProperty(ref _paldeaVariant, value); }
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

        private Pokemon _selectedPokemon;
        public Pokemon SelectedPokemon
        {
            get { return _selectedPokemon; }
            set
            {
                _selectedPokemon = value;
                _ = DetailsPokemonAsync(_selectedPokemon);
            }
        }
        #endregion

        #region Data
        private TypePok _firstType;

        public TypePok FirstType
        {
            get { return _firstType; }
            set { SetProperty(ref _firstType, value); }
        }

        private Talent _firstTalent;

        public Talent FirstTalent
        {
            get { return _firstTalent; }
            set { SetProperty(ref _firstTalent, value); }
        }

        private Talent _secondTalent;

        public Talent SecondTalent
        {
            get { return _secondTalent; }
            set { SetProperty(ref _secondTalent, value); }
        }
        #endregion

        #region View Span List
        private int _countFamilyEvol = 3;

        public int CountFamilyEvol
        {
            get { return _countFamilyEvol; }
            set { SetProperty(ref _countFamilyEvol, value); }
        }

        private int _heightFamilyEvol = 150;

        public int HeightFamilyEvol
        {
            get { return _heightFamilyEvol; }
            set { SetProperty(ref _heightFamilyEvol, value); }
        }

        private int _countVariantEvol = 3;

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

        private int _countWeakness = 3;

        public int CountWeakness
        {
            get { return _countWeakness; }
            set { SetProperty(ref _countWeakness, value); }
        }

        private int _heightWeakness;

        public int HeightWeakness
        {
            get { return _heightWeakness; }
            set { SetProperty(ref _heightWeakness, value); }
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

        private bool _statisticIsVisible;

        public bool StatisticIsVisible
        {
            get { return _statisticIsVisible; }
            set { SetProperty(ref _statisticIsVisible, value); }
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

        private bool _paldeaIsVisible;
        public bool PaldeaIsVisible
        {
            get { return _paldeaIsVisible; }
            set { SetProperty(ref _paldeaIsVisible, value); }
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

        #region Chart Stat
        private Chart _chartStats;

        public Chart ChartStats
        {
            get { return _chartStats; }
            set { _chartStats = value; }
        }

        private ChartEntry[] _chartEntry;

        public ChartEntry[] ChartEntry
        {
            get { return _chartEntry; }
            set { SetProperty(ref _chartEntry, value); }
        }
        #endregion

        #region Favorite
        private bool _isFavorite = false;

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { SetProperty(ref _isFavorite, value); }
        }

        private byte[] _imgFavorite;

        public byte[] ImgFavorite
        {
            get { return _imgFavorite; }
            set { SetProperty(ref _imgFavorite, value); }
        }
        #endregion
        #endregion
    }
}
