using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using QuizzPokedex.Interfaces;
using QuizzPokedex.Models;
using QuizzPokedex.Resources;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QuizzPokedex.ViewModels
{
    public class PokedexViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService _navigation;
        private readonly IPokemonService _pokemonService;
        private readonly ITypePokService _typePokService;

        public PokedexViewModel(IMvxNavigationService navigation, IPokemonService pokemonService, ITypePokService typePokService, IMvxMessenger messenger)
        {
            _navigation = navigation;
            _pokemonService = pokemonService;
            _typePokService = typePokService;
        }

        public override async Task Initialize()
        {
            LoadPokemonTask = MvxNotifyTask.Create(LoadPokemonAsync);
            await base.Initialize();
        }

        #region Private Method
        private async Task LoadPokemonAsync()
        {
            var resultPokemon = await _pokemonService.GetAllWithoutVariantAsync(SearchText
                , FiltreActiveGen1
                , FiltreActiveGen2
                , FiltreActiveGen3
                , FiltreActiveGen4
                , FiltreActiveGen5
                , FiltreActiveGen6
                , FiltreActiveGen7
                , FiltreActiveGen8
                , FiltreActiveGenArceus
                , FiltreActiveTypeSteel
                , FiltreActiveTypeFighting
                , FiltreActiveTypeDragon
                , FiltreActiveTypeWater
                , FiltreActiveTypeElectric
                , FiltreActiveTypeFairy
                , FiltreActiveTypeFire
                , FiltreActiveTypeIce
                , FiltreActiveTypeBug
                , FiltreActiveTypeNormal
                , FiltreActiveTypeGrass
                , FiltreActiveTypePoison
                , FiltreActiveTypePsychic
                , FiltreActiveTypeRock
                , FiltreActiveTypeGround
                , FiltreActiveTypeGhost
                , FiltreActiveTypeDark
                , FiltreActiveTypeFlying);
            Pokemons = new MvxObservableCollection<Pokemon>(resultPokemon);
            await GetBytesTypesFilter();
        }

        private async Task GetBytesTypesFilter()
        {
            #region Type Filter

            var types = await _typePokService.GetAllAsync();

            foreach (var type in types)
            {
                switch (type.Name)
                {
                    case Constantes.Steel:
                        if(ImgTypeSteelFilter == null)
                            ImgTypeSteelFilter = await getByteTypeIcon(Constantes.Icon_Steel_BW);
                        break;
                    case Constantes.Fighting:
                        if (ImgTypeFightingFilter == null)
                            ImgTypeFightingFilter = await getByteTypeIcon(Constantes.Icon_Fighting_BW);
                        break;
                    case Constantes.Dragon:
                        if (ImgTypeDragonFilter == null)
                            ImgTypeDragonFilter = await getByteTypeIcon(Constantes.Icon_Dragon_BW);
                        break;
                    case Constantes.Water:
                        if (ImgTypeWaterFilter == null)
                            ImgTypeWaterFilter = await getByteTypeIcon(Constantes.Icon_Water_BW);
                        break;
                    case Constantes.Electric:
                        if (ImgTypeElectricFilter == null)
                            ImgTypeElectricFilter = await getByteTypeIcon(Constantes.Icon_Electric_BW);
                        break;
                    case Constantes.Fairy:
                        if (ImgTypeFairyFilter == null)
                            ImgTypeFairyFilter = await getByteTypeIcon(Constantes.Icon_Fairy_BW);
                        break;
                    case Constantes.Fire:
                        if (ImgTypeFireFilter == null)
                            ImgTypeFireFilter = await getByteTypeIcon(Constantes.Icon_Fire_BW);
                        break;
                    case Constantes.Ice:
                        if (ImgTypeIceFilter == null)
                            ImgTypeIceFilter = await getByteTypeIcon(Constantes.Icon_Ice_BW);
                        break;
                    case Constantes.Bug:
                        if (ImgTypeBugFilter == null)
                            ImgTypeBugFilter = await getByteTypeIcon(Constantes.Icon_Bug_BW);
                        break;
                    case Constantes.Normal:
                        if (ImgTypeNormalFilter == null)
                            ImgTypeNormalFilter = await getByteTypeIcon(Constantes.Icon_Normal_BW);
                        break;
                    case Constantes.Grass:
                        if (ImgTypeGrassFilter == null)
                            ImgTypeGrassFilter = await getByteTypeIcon(Constantes.Icon_Grass_BW);
                        break;
                    case Constantes.Poison:
                        if (ImgTypePoisonFilter == null)
                            ImgTypePoisonFilter = await getByteTypeIcon(Constantes.Icon_Poison_BW);
                        break;
                    case Constantes.Psychic:
                        if (ImgTypePsychicFilter == null)
                            ImgTypePsychicFilter = await getByteTypeIcon(Constantes.Icon_Psychic_BW);
                        break;
                    case Constantes.Rock:
                        if (ImgTypeRockFilter == null)
                            ImgTypeRockFilter = await getByteTypeIcon(Constantes.Icon_Rock_BW);
                        break;
                    case Constantes.Ground:
                        if (ImgTypeGroundFilter == null)
                            ImgTypeGroundFilter = await getByteTypeIcon(Constantes.Icon_Ground_BW);
                        break;
                    case Constantes.Ghost:
                        if (ImgTypeGhostFilter == null)
                            ImgTypeGhostFilter = await getByteTypeIcon(Constantes.Icon_Ghost_BW);
                        break;
                    case Constantes.Dark:
                        if (ImgTypeDarkFilter == null)
                            ImgTypeDarkFilter = await getByteTypeIcon(Constantes.Icon_Dark_BW);
                        break;
                    case Constantes.Flying:
                        if (ImgTypeFlyingFilter == null)
                            ImgTypeFlyingFilter = await getByteTypeIcon(Constantes.Icon_Flying_BW);
                        break;
                }
            }
            #endregion
        }

        private async Task<byte[]> getByteTypeIcon(string fileName)
        {
            AssetManager assets = Android.App.Application.Context.Assets;
            const int maxReadSize = 256 * 1024;
            byte[] imgByte;
            using (BinaryReader br = new BinaryReader(assets.Open(fileName)))
            {
                imgByte = br.ReadBytes(maxReadSize);
            }

            return await Task.FromResult(imgByte);
        }
        #endregion

        #region COMMAND
        public IMvxAsyncCommand NavigationBackCommandAsync => new MvxAsyncCommand(NavigationBackAsync);
        public IMvxAsyncCommand ModalFilterCommandAsync => new MvxAsyncCommand(ModalFilterAsync);
        public IMvxAsyncCommand ModalTypeFilterCommandAsync => new MvxAsyncCommand(ModalTypeFilterAsync);
        public IMvxAsyncCommand ModalGenFilterCommandAsync => new MvxAsyncCommand(ModalGenFilterAsync);
        public IMvxAsyncCommand BackModalTypeFilterCommandAsync => new MvxAsyncCommand(BackModalTypeFilterAsync);
        public IMvxAsyncCommand CloseModalTypeFilterCommandAsync => new MvxAsyncCommand(CloseModalTypeFilterAsync);
        public IMvxAsyncCommand BackModalGenFilterCommandAsync => new MvxAsyncCommand(BackModalGenFilterAsync);
        public IMvxAsyncCommand CloseModalGenFilterCommandAsync => new MvxAsyncCommand(CloseModalGenFilterAsync);
        public IMvxAsyncCommand CreatePokemonCommandAsync => new MvxAsyncCommand(CreatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> UpdatePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(UpdatePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DeletePokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DeletePokemonAsync);
        public IMvxAsyncCommand<Pokemon> DetailsPokemonCommandAsync => new MvxAsyncCommand<Pokemon>(DetailsPokemonAsync);

        #region Command Filter
        public IMvxAsyncCommand FilterByNameCommandAsync => new MvxAsyncCommand(FilterByNameAsync);
        #region Command Type Filter
        public IMvxAsyncCommand FilterByTypeSteelCommandAsync => new MvxAsyncCommand(FilterByTypeSteelAsync);
        public IMvxAsyncCommand FilterByTypeFightingCommandAsync => new MvxAsyncCommand(FilterByTypeFightingAsync);
        public IMvxAsyncCommand FilterByTypeDragonCommandAsync => new MvxAsyncCommand(FilterByTypeDragonAsync);
        public IMvxAsyncCommand FilterByTypeWaterCommandAsync => new MvxAsyncCommand(FilterByTypeWaterAsync);
        public IMvxAsyncCommand FilterByTypeElectricCommandAsync => new MvxAsyncCommand(FilterByTypeElectricAsync);
        public IMvxAsyncCommand FilterByTypeFairyCommandAsync => new MvxAsyncCommand(FilterByTypeFairyAsync);
        public IMvxAsyncCommand FilterByTypeFireCommandAsync => new MvxAsyncCommand(FilterByTypeFireAsync);
        public IMvxAsyncCommand FilterByTypeIceCommandAsync => new MvxAsyncCommand(FilterByTypeIceAsync);
        public IMvxAsyncCommand FilterByTypeBugCommandAsync => new MvxAsyncCommand(FilterByTypeBugAsync);
        public IMvxAsyncCommand FilterByTypeNormalCommandAsync => new MvxAsyncCommand(FilterByTypeNormalAsync);
        public IMvxAsyncCommand FilterByTypeGrassCommandAsync => new MvxAsyncCommand(FilterByTypeGrassAsync);
        public IMvxAsyncCommand FilterByTypePoisonCommandAsync => new MvxAsyncCommand(FilterByTypePoisonAsync);
        public IMvxAsyncCommand FilterByTypePsychicCommandAsync => new MvxAsyncCommand(FilterByTypePsychicAsync);
        public IMvxAsyncCommand FilterByTypeRockCommandAsync => new MvxAsyncCommand(FilterByTypeRockAsync);
        public IMvxAsyncCommand FilterByTypeGroundCommandAsync => new MvxAsyncCommand(FilterByTypeGroundAsync);
        public IMvxAsyncCommand FilterByTypeGhostCommandAsync => new MvxAsyncCommand(FilterByTypeGhostAsync);
        public IMvxAsyncCommand FilterByTypeDarkCommandAsync => new MvxAsyncCommand(FilterByTypeDarkAsync);
        public IMvxAsyncCommand FilterByTypeFlyingCommandAsync => new MvxAsyncCommand(FilterByTypeFlyingAsync);
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
        #endregion

        #region Navigation Back & Filter
        private async Task NavigationBackAsync()
        {
            await _navigation.Close(this);
        }

        private async Task ModalFilterAsync()
        {
            IsVisibleModalFilter = !IsVisibleModalFilter;
        }

        private async Task ModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
            IsVisibleModalGenFilter = !IsVisibleModalGenFilter;
        }

        private async Task ModalTypeFilterAsync()
        {
            IsVisibleBackgroundModalFilter = !IsVisibleBackgroundModalFilter;
            IsVisibleModalTypeFilter = !IsVisibleModalTypeFilter;
        }
        private async Task BackModalTypeFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalTypeFilter = false;
        }

        private async Task CloseModalTypeFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalTypeFilter = false;
            IsVisibleModalFilter = false;
        }


        private async Task BackModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalGenFilter = false;
        }

        private async Task CloseModalGenFilterAsync()
        {
            IsVisibleBackgroundModalFilter = false;
            IsVisibleModalGenFilter = false;
            IsVisibleModalFilter = false;
        }
        #endregion

        #region Filter By
        private async Task FilterByNameAsync()
        {
            await LoadPokemonAsync();
        }

        #region Filter By Type
        private async Task FilterByTypeSteelAsync()
        {
            if (FiltreActiveTypeSteel)
            {
                FiltreActiveTypeSteel = false;
                ImgTypeSteelFilter = await getByteTypeIcon(Constantes.Icon_Steel_BW);
                BackgroundColorSteel = Constantes.WhiteHexa;
                TextColorSteel = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeSteel = true;
                ImgTypeSteelFilter = await getByteTypeIcon(Constantes.Icon_Steel);
                BackgroundColorSteel = await _typePokService.GetBackgroundColorType(Constantes.Steel);
                TextColorSteel = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeFightingAsync()
        {
            if (FiltreActiveTypeFighting)
            {
                FiltreActiveTypeFighting = false;
                ImgTypeFightingFilter = await getByteTypeIcon(Constantes.Icon_Fighting_BW);
                BackgroundColorFighting = Constantes.WhiteHexa;
                TextColorFighting = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeFighting = true;
                ImgTypeFightingFilter = await getByteTypeIcon(Constantes.Icon_Fighting);
                BackgroundColorFighting = await _typePokService.GetBackgroundColorType(Constantes.Fighting);
                TextColorFighting = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeDragonAsync()
        {
            if (FiltreActiveTypeDragon)
            {
                FiltreActiveTypeDragon = false;
                ImgTypeDragonFilter = await getByteTypeIcon(Constantes.Icon_Dragon_BW);
                BackgroundColorDragon = Constantes.WhiteHexa;
                TextColorDragon = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeDragon = true;
                ImgTypeDragonFilter = await getByteTypeIcon(Constantes.Icon_Dragon);
                BackgroundColorDragon = await _typePokService.GetBackgroundColorType(Constantes.Dragon);
                TextColorDragon = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeWaterAsync()
        {
            if (FiltreActiveTypeWater)
            {
                FiltreActiveTypeWater = false;
                ImgTypeWaterFilter = await getByteTypeIcon(Constantes.Icon_Water_BW);
                BackgroundColorWater = Constantes.WhiteHexa;
                TextColorWater = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeWater = true;
                ImgTypeWaterFilter = await getByteTypeIcon(Constantes.Icon_Water);
                BackgroundColorWater = await _typePokService.GetBackgroundColorType(Constantes.Water);
                TextColorWater = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeElectricAsync()
        {
            if (FiltreActiveTypeElectric)
            {
                FiltreActiveTypeElectric = false;
                ImgTypeElectricFilter = await getByteTypeIcon(Constantes.Icon_Electric_BW);
                BackgroundColorElectric = Constantes.WhiteHexa;
                TextColorElectric = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeElectric = true;
                ImgTypeElectricFilter = await getByteTypeIcon(Constantes.Icon_Electric);
                BackgroundColorElectric = await _typePokService.GetBackgroundColorType(Constantes.Electric);
                TextColorElectric = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeFairyAsync()
        {
            if (FiltreActiveTypeFairy)
            {
                FiltreActiveTypeFairy = false;
                ImgTypeFairyFilter = await getByteTypeIcon(Constantes.Icon_Fairy_BW);
                BackgroundColorFairy = Constantes.WhiteHexa;
                TextColorFairy = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeFairy = true;
                ImgTypeFairyFilter = await getByteTypeIcon(Constantes.Icon_Fairy);
                BackgroundColorFairy = await _typePokService.GetBackgroundColorType(Constantes.Fairy);
                TextColorFairy = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeFireAsync()
        {
            if (FiltreActiveTypeFire)
            {
                FiltreActiveTypeFire = false;
                ImgTypeFireFilter = await getByteTypeIcon(Constantes.Icon_Fire_BW);
                BackgroundColorFire = Constantes.WhiteHexa;
                TextColorFire = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeFire = true;
                ImgTypeFireFilter = await getByteTypeIcon(Constantes.Icon_Fire);
                BackgroundColorFire = await _typePokService.GetBackgroundColorType(Constantes.Fire);
                TextColorFire = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeIceAsync()
        {
            if (FiltreActiveTypeIce)
            {
                FiltreActiveTypeIce = false;
                ImgTypeIceFilter = await getByteTypeIcon(Constantes.Icon_Ice_BW);
                BackgroundColorIce = Constantes.WhiteHexa;
                TextColorIce = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeIce = true;
                ImgTypeIceFilter = await getByteTypeIcon(Constantes.Icon_Ice);
                BackgroundColorIce = await _typePokService.GetBackgroundColorType(Constantes.Ice);
                TextColorIce = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeBugAsync()
        {
            if (FiltreActiveTypeBug)
            {
                FiltreActiveTypeBug = false;
                ImgTypeBugFilter = await getByteTypeIcon(Constantes.Icon_Bug_BW);
                BackgroundColorBug = Constantes.WhiteHexa;
                TextColorBug = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeBug = true;
                ImgTypeBugFilter = await getByteTypeIcon(Constantes.Icon_Bug);
                BackgroundColorBug = await _typePokService.GetBackgroundColorType(Constantes.Bug);
                TextColorBug = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeNormalAsync()
        {
            if (FiltreActiveTypeNormal)
            {
                FiltreActiveTypeNormal = false;
                ImgTypeNormalFilter = await getByteTypeIcon(Constantes.Icon_Normal_BW);
                BackgroundColorNormal = Constantes.WhiteHexa;
                TextColorNormal = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeNormal = true;
                ImgTypeNormalFilter = await getByteTypeIcon(Constantes.Icon_Normal);
                BackgroundColorNormal = await _typePokService.GetBackgroundColorType(Constantes.Normal);
                TextColorNormal = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeGrassAsync()
        {
            if (FiltreActiveTypeGrass)
            {
                FiltreActiveTypeGrass = false;
                ImgTypeGrassFilter = await getByteTypeIcon(Constantes.Icon_Grass_BW);
                BackgroundColorGrass = Constantes.WhiteHexa;
                TextColorGrass = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeGrass = true;
                ImgTypeGrassFilter = await getByteTypeIcon(Constantes.Icon_Grass);
                BackgroundColorGrass = await _typePokService.GetBackgroundColorType(Constantes.Grass);
                TextColorGrass = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypePoisonAsync()
        {
            if (FiltreActiveTypePoison)
            {
                FiltreActiveTypePoison = false;
                ImgTypePoisonFilter = await getByteTypeIcon(Constantes.Icon_Poison_BW);
                BackgroundColorPoison = Constantes.WhiteHexa;
                TextColorPoison = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypePoison = true;
                ImgTypePoisonFilter = await getByteTypeIcon(Constantes.Icon_Poison);
                BackgroundColorPoison = await _typePokService.GetBackgroundColorType(Constantes.Poison);
                TextColorPoison = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypePsychicAsync()
        {
            if (FiltreActiveTypePsychic)
            {
                FiltreActiveTypePsychic = false;
                ImgTypePsychicFilter = await getByteTypeIcon(Constantes.Icon_Psychic_BW);
                BackgroundColorPsychic = Constantes.WhiteHexa;
                TextColorPsychic = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypePsychic = true;
                ImgTypePsychicFilter = await getByteTypeIcon(Constantes.Icon_Psychic);
                BackgroundColorPsychic = await _typePokService.GetBackgroundColorType(Constantes.Psychic);
                TextColorPsychic = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeRockAsync()
        {
            if (FiltreActiveTypeRock)
            {
                FiltreActiveTypeRock = false;
                ImgTypeRockFilter = await getByteTypeIcon(Constantes.Icon_Rock_BW);
                BackgroundColorRock = Constantes.WhiteHexa;
                TextColorRock = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeRock = true;
                ImgTypeRockFilter = await getByteTypeIcon(Constantes.Icon_Rock);
                BackgroundColorRock = await _typePokService.GetBackgroundColorType(Constantes.Rock);
                TextColorRock = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeGroundAsync()
        {
            if (FiltreActiveTypeGround)
            {
                FiltreActiveTypeGround = false;
                ImgTypeGroundFilter = await getByteTypeIcon(Constantes.Icon_Ground_BW);
                BackgroundColorGround = Constantes.WhiteHexa;
                TextColorGround = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeGround = true;
                ImgTypeGroundFilter = await getByteTypeIcon(Constantes.Icon_Ground);
                BackgroundColorGround = await _typePokService.GetBackgroundColorType(Constantes.Ground);
                TextColorGround = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeGhostAsync()
        {
            if (FiltreActiveTypeGhost)
            {
                FiltreActiveTypeGhost = false;
                ImgTypeGhostFilter = await getByteTypeIcon(Constantes.Icon_Ghost_BW);
                BackgroundColorGhost = Constantes.WhiteHexa;
                TextColorGhost = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeGhost = true;
                ImgTypeGhostFilter = await getByteTypeIcon(Constantes.Icon_Ghost);
                BackgroundColorGhost = await _typePokService.GetBackgroundColorType(Constantes.Ghost);
                TextColorGhost = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeDarkAsync()
        {
            if (FiltreActiveTypeDark)
            {
                FiltreActiveTypeDark = false;
                ImgTypeDarkFilter = await getByteTypeIcon(Constantes.Icon_Dark_BW);
                BackgroundColorDark = Constantes.WhiteHexa;
                TextColorDark = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeDark = true;
                ImgTypeDarkFilter = await getByteTypeIcon(Constantes.Icon_Dark);
                BackgroundColorDark = await _typePokService.GetBackgroundColorType(Constantes.Dark);
                TextColorDark = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }

        private async Task FilterByTypeFlyingAsync()
        {
            if (FiltreActiveTypeFlying)
            {
                FiltreActiveTypeFlying = false;
                ImgTypeFlyingFilter = await getByteTypeIcon(Constantes.Icon_Flying_BW);
                BackgroundColorFlying = Constantes.WhiteHexa;
                TextColorFlying = Constantes.BlackHexa;
            }
            else
            {
                FiltreActiveTypeFlying = true;
                ImgTypeFlyingFilter = await getByteTypeIcon(Constantes.Icon_Flying);
                BackgroundColorFlying = await _typePokService.GetBackgroundColorType(Constantes.Flying);
                TextColorFlying = Constantes.WhiteHexa;
            }

            await LoadPokemonAsync();
        }
        #endregion

        #region Filter By Gen
        private async Task FilterByGen1Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen2Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen3Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen4Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen5Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen6Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen7Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGen8Async()
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

            await LoadPokemonAsync();
        }

        private async Task FilterByGenArceusAsync()
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

            await LoadPokemonAsync();
        }
        #endregion
        #endregion

        #region CRUD
        private async Task CreatePokemonAsync()
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(new Pokemon());
        }

        private async Task UpdatePokemonAsync(Pokemon Pokemon)
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(Pokemon);
        }

        private async Task DeletePokemonAsync(Pokemon Pokemon)
        {
            //Peut etre mettre une boite de dialogue de confirmation avant delete (leçon sur les dialogBox)

            await _pokemonService.DeleteAsync(Pokemon).ContinueWith(
                async (result) =>
                    await LoadPokemonAsync()
                    );
        }

        private async Task DetailsPokemonAsync(Pokemon Pokemon)
        {
            await _navigation.Navigate<PokemonViewModel, Pokemon>(Pokemon);
        }
        #endregion
        #endregion

        #region PROPERTIES
        #region Collection
        public MvxNotifyTask LoadPokemonTask { get; private set; }

        private MvxObservableCollection<Pokemon> _pokemons;

        public MvxObservableCollection<Pokemon> Pokemons
        {
            get { return _pokemons; }
            set
            {
                SetProperty(ref _pokemons, value);
                RaisePropertyChanged(() => Pokemons);
            }
        }

        private MvxObservableCollection<TypePok> _typesPok;

        public MvxObservableCollection<TypePok> TypesPok
        {
            get { return _typesPok; }
            set { SetProperty(ref _typesPok, value); }
        }
        #endregion

        #region Data
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

        #region Search
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged(() => SearchText);
                    _ = FilterByNameAsync();
                }
            }
        }
        #endregion

        #region Visibility Filter
        private bool _isVisibleModalFilter;

        public bool IsVisibleModalFilter
        {
            get { return _isVisibleModalFilter; }
            set { SetProperty(ref _isVisibleModalFilter, value); }
        }


        private bool _isVisibleBackgroundModalFilter;

        public bool IsVisibleBackgroundModalFilter
        {
            get { return _isVisibleBackgroundModalFilter; }
            set { SetProperty(ref _isVisibleBackgroundModalFilter, value); }
        }

        private bool _isVisibleModalGenFilter;

        public bool IsVisibleModalGenFilter
        {
            get { return _isVisibleModalGenFilter; }
            set { SetProperty(ref _isVisibleModalGenFilter, value); }
        }

        private bool _isVisibleModalTypeFilter;

        public bool IsVisibleModalTypeFilter
        {
            get { return _isVisibleModalTypeFilter; }
            set
            { SetProperty(ref _isVisibleModalTypeFilter, value); }
        }
        #endregion

        #region Filter Type
        #region Steel
        private bool _filtreActiveTypeSteel = false;

        public bool FiltreActiveTypeSteel
        {
            get { return _filtreActiveTypeSteel; }
            set { SetProperty(ref _filtreActiveTypeSteel, value); }
        }

        private byte[] _imgTypeSteelFilter;

        public byte[] ImgTypeSteelFilter
        {
            get { return _imgTypeSteelFilter; }
            set { SetProperty(ref _imgTypeSteelFilter, value); }
        }

        private string _backgroundColorSteel = "#FFFFFF";

        public string BackgroundColorSteel
        {
            get { return _backgroundColorSteel; }
            set { SetProperty(ref _backgroundColorSteel, value); }
        }

        private string _textColorSteel = "#000000";

        public string TextColorSteel
        {
            get { return _textColorSteel; }
            set { SetProperty(ref _textColorSteel, value); }
        }
        #endregion

        #region Dragon
        private bool _filtreActiveTypeDragon;

        public bool FiltreActiveTypeDragon
        {
            get { return _filtreActiveTypeDragon; }
            set { SetProperty(ref _filtreActiveTypeDragon, value); }
        }

        private byte[] _imgTypeDragonFilter;

        public byte[] ImgTypeDragonFilter
        {
            get { return _imgTypeDragonFilter; }
            set { SetProperty(ref _imgTypeDragonFilter, value); }
        }

        private string _backgroundColorDragon = "#FFFFFF";

        public string BackgroundColorDragon
        {
            get { return _backgroundColorDragon; }
            set { SetProperty(ref _backgroundColorDragon, value); }
        }

        private string _textColorDragon = "#000000";

        public string TextColorDragon
        {
            get { return _textColorDragon; }
            set { SetProperty(ref _textColorDragon, value); }
        }
        #endregion

        #region Electric
        private bool _filtreActiveTypeElectric;

        public bool FiltreActiveTypeElectric
        {
            get { return _filtreActiveTypeElectric; }
            set { SetProperty(ref _filtreActiveTypeElectric, value); }
        }

        private byte[] _imgTypeElectricFilter;

        public byte[] ImgTypeElectricFilter
        {
            get { return _imgTypeElectricFilter; }
            set { SetProperty(ref _imgTypeElectricFilter, value); }
        }

        private string _backgroundColorElectric = "#FFFFFF";

        public string BackgroundColorElectric
        {
            get { return _backgroundColorElectric; }
            set { SetProperty(ref _backgroundColorElectric, value); }
        }

        private string _textColorElectric = "#000000";

        public string TextColorElectric
        {
            get { return _textColorElectric; }
            set { SetProperty(ref _textColorElectric, value); }
        }
        #endregion

        #region Fire
        private bool _filtreActiveTypeFire;

        public bool FiltreActiveTypeFire
        {
            get { return _filtreActiveTypeFire; }
            set { SetProperty(ref _filtreActiveTypeFire, value); }
        }

        private byte[] _imgTypeFireFilter;

        public byte[] ImgTypeFireFilter
        {
            get { return _imgTypeFireFilter; }
            set { SetProperty(ref _imgTypeFireFilter, value); }
        }

        private string _backgroundColorFire = "#FFFFFF";

        public string BackgroundColorFire
        {
            get { return _backgroundColorFire; }
            set { SetProperty(ref _backgroundColorFire, value); }
        }

        private string _textColorFire = "#000000";

        public string TextColorFire
        {
            get { return _textColorFire; }
            set { SetProperty(ref _textColorFire, value); }
        }
        #endregion

        #region Bug
        private bool _filtreActiveTypeBug;

        public bool FiltreActiveTypeBug
        {
            get { return _filtreActiveTypeBug; }
            set { SetProperty(ref _filtreActiveTypeBug, value); }
        }

        private byte[] _imgTypeBugFilter;

        public byte[] ImgTypeBugFilter
        {
            get { return _imgTypeBugFilter; }
            set { SetProperty(ref _imgTypeBugFilter, value); }
        }

        private string _backgroundColorBug = "#FFFFFF";

        public string BackgroundColorBug
        {
            get { return _backgroundColorBug; }
            set { SetProperty(ref _backgroundColorBug, value); }
        }

        private string _textColorBug = "#000000";

        public string TextColorBug
        {
            get { return _textColorBug; }
            set { SetProperty(ref _textColorBug, value); }
        }
        #endregion

        #region Grass
        private bool _filtreActiveTypeGrass;

        public bool FiltreActiveTypeGrass
        {
            get { return _filtreActiveTypeGrass; }
            set { SetProperty(ref _filtreActiveTypeGrass, value); }
        }

        private byte[] _imgTypeGrassFilter;

        public byte[] ImgTypeGrassFilter
        {
            get { return _imgTypeGrassFilter; }
            set { SetProperty(ref _imgTypeGrassFilter, value); }
        }

        private string _backgroundColorGrass = "#FFFFFF";

        public string BackgroundColorGrass
        {
            get { return _backgroundColorGrass; }
            set { SetProperty(ref _backgroundColorGrass, value); }
        }

        private string _textColorGrass = "#000000";

        public string TextColorGrass
        {
            get { return _textColorGrass; }
            set { SetProperty(ref _textColorGrass, value); }
        }
        #endregion

        #region Psychic
        private bool _filtreActiveTypePsychic;

        public bool FiltreActiveTypePsychic
        {
            get { return _filtreActiveTypePsychic; }
            set { SetProperty(ref _filtreActiveTypePsychic, value); }
        }

        private byte[] _imgTypePsychicFilter;

        public byte[] ImgTypePsychicFilter
        {
            get { return _imgTypePsychicFilter; }
            set { SetProperty(ref _imgTypePsychicFilter, value); }
        }

        private string _backgroundColorPsychic = "#FFFFFF";

        public string BackgroundColorPsychic
        {
            get { return _backgroundColorPsychic; }
            set { SetProperty(ref _backgroundColorPsychic, value); }
        }

        private string _textColorPsychic = "#000000";

        public string TextColorPsychic
        {
            get { return _textColorPsychic; }
            set { SetProperty(ref _textColorPsychic, value); }
        }
        #endregion

        #region Ground
        private bool _filtreActiveTypeGround;

        public bool FiltreActiveTypeGround
        {
            get { return _filtreActiveTypeGround; }
            set { SetProperty(ref _filtreActiveTypeGround, value); }
        }

        private byte[] _imgTypeGroundFilter;

        public byte[] ImgTypeGroundFilter
        {
            get { return _imgTypeGroundFilter; }
            set { SetProperty(ref _imgTypeGroundFilter, value); }
        }

        private string _backgroundColorGround = "#FFFFFF";

        public string BackgroundColorGround
        {
            get { return _backgroundColorGround; }
            set { SetProperty(ref _backgroundColorGround, value); }
        }

        private string _textColorGround = "#000000";

        public string TextColorGround
        {
            get { return _textColorGround; }
            set { SetProperty(ref _textColorGround, value); }
        }
        #endregion

        #region Dark
        private bool _filtreActiveTypeDark;

        public bool FiltreActiveTypeDark
        {
            get { return _filtreActiveTypeDark; }
            set { SetProperty(ref _filtreActiveTypeDark, value); }
        }

        private byte[] _imgTypeDarkFilter;

        public byte[] ImgTypeDarkFilter
        {
            get { return _imgTypeDarkFilter; }
            set { SetProperty(ref _imgTypeDarkFilter, value); }
        }

        private string _backgroundColorDark = "#FFFFFF";

        public string BackgroundColorDark
        {
            get { return _backgroundColorDark; }
            set { SetProperty(ref _backgroundColorDark, value); }
        }

        private string _textColorDark = "#000000";

        public string TextColorDark
        {
            get { return _textColorDark; }
            set { SetProperty(ref _textColorDark, value); }
        }
        #endregion

        #region Fighting
        private bool _filtreActiveTypeFighting;

        public bool FiltreActiveTypeFighting
        {
            get { return _filtreActiveTypeFighting; }
            set { SetProperty(ref _filtreActiveTypeFighting, value); }
        }

        private byte[] _imgTypeFightingFilter;

        public byte[] ImgTypeFightingFilter
        {
            get { return _imgTypeFightingFilter; }
            set { SetProperty(ref _imgTypeFightingFilter, value); }
        }

        private string _backgroundColorFighting = "#FFFFFF";

        public string BackgroundColorFighting
        {
            get { return _backgroundColorFighting; }
            set { SetProperty(ref _backgroundColorFighting, value); }
        }

        private string _textColorFighting = "#000000";

        public string TextColorFighting
        {
            get { return _textColorFighting; }
            set { SetProperty(ref _textColorFighting, value); }
        }
        #endregion

        #region Water
        private bool _filtreActiveTypeWater;

        public bool FiltreActiveTypeWater
        {
            get { return _filtreActiveTypeWater; }
            set { SetProperty(ref _filtreActiveTypeWater, value); }
        }

        private byte[] _imgTypeWaterFilter;

        public byte[] ImgTypeWaterFilter
        {
            get { return _imgTypeWaterFilter; }
            set { SetProperty(ref _imgTypeWaterFilter, value); }
        }

        private string _backgroundColorWater = "#FFFFFF";

        public string BackgroundColorWater
        {
            get { return _backgroundColorWater; }
            set { SetProperty(ref _backgroundColorWater, value); }
        }

        private string _textColorWater = "#000000";

        public string TextColorWater
        {
            get { return _textColorWater; }
            set { SetProperty(ref _textColorWater, value); }
        }
        #endregion

        #region Fairy
        private bool _filtreActiveTypeFairy;

        public bool FiltreActiveTypeFairy
        {
            get { return _filtreActiveTypeFairy; }
            set { SetProperty(ref _filtreActiveTypeFairy, value); }
        }

        private byte[] _imgTypeFairyFilter;

        public byte[] ImgTypeFairyFilter
        {
            get { return _imgTypeFairyFilter; }
            set { SetProperty(ref _imgTypeFairyFilter, value); }
        }

        private string _backgroundColorFairy = "#FFFFFF";

        public string BackgroundColorFairy
        {
            get { return _backgroundColorFairy; }
            set { SetProperty(ref _backgroundColorFairy, value); }
        }

        private string _textColorFairy = "#000000";

        public string TextColorFairy
        {
            get { return _textColorFairy; }
            set { SetProperty(ref _textColorFairy, value); }
        }
        #endregion

        #region Ice
        private bool _filtreActiveTypeIce;

        public bool FiltreActiveTypeIce
        {
            get { return _filtreActiveTypeIce; }
            set { SetProperty(ref _filtreActiveTypeIce, value); }
        }

        private byte[] _imgTypeIceFilter;

        public byte[] ImgTypeIceFilter
        {
            get { return _imgTypeIceFilter; }
            set { SetProperty(ref _imgTypeIceFilter, value); }
        }

        private string _backgroundColorIce = "#FFFFFF";

        public string BackgroundColorIce
        {
            get { return _backgroundColorIce; }
            set { SetProperty(ref _backgroundColorIce, value); }
        }

        private string _textColorIce = "#000000";

        public string TextColorIce
        {
            get { return _textColorIce; }
            set { SetProperty(ref _textColorIce, value); }
        }
        #endregion

        #region Normal
        private bool _filtreActiveTypeNormal;

        public bool FiltreActiveTypeNormal
        {
            get { return _filtreActiveTypeNormal; }
            set { SetProperty(ref _filtreActiveTypeNormal, value); }
        }

        private byte[] _imgTypeNormalFilter;

        public byte[] ImgTypeNormalFilter
        {
            get { return _imgTypeNormalFilter; }
            set { SetProperty(ref _imgTypeNormalFilter, value); }
        }

        private string _backgroundColorNormal = "#FFFFFF";

        public string BackgroundColorNormal
        {
            get { return _backgroundColorNormal; }
            set { SetProperty(ref _backgroundColorNormal, value); }
        }

        private string _textColorNormal = "#000000";

        public string TextColorNormal
        {
            get { return _textColorNormal; }
            set { SetProperty(ref _textColorNormal, value); }
        }
        #endregion

        #region Poison
        private bool _filtreActiveTypePoison;

        public bool FiltreActiveTypePoison
        {
            get { return _filtreActiveTypePoison; }
            set { SetProperty(ref _filtreActiveTypePoison, value); }
        }

        private byte[] _imgTypePoisonFilter;

        public byte[] ImgTypePoisonFilter
        {
            get { return _imgTypePoisonFilter; }
            set { SetProperty(ref _imgTypePoisonFilter, value); }
        }

        private string _backgroundColorPoison = "#FFFFFF";

        public string BackgroundColorPoison
        {
            get { return _backgroundColorPoison; }
            set { SetProperty(ref _backgroundColorPoison, value); }
        }

        private string _textColorPoison = "#000000";

        public string TextColorPoison
        {
            get { return _textColorPoison; }
            set { SetProperty(ref _textColorPoison, value); }
        }
        #endregion

        #region Rock
        private bool _filtreActiveTypeRock;

        public bool FiltreActiveTypeRock
        {
            get { return _filtreActiveTypeRock; }
            set { SetProperty(ref _filtreActiveTypeRock, value); }
        }

        private byte[] _imgTypeRockFilter;

        public byte[] ImgTypeRockFilter
        {
            get { return _imgTypeRockFilter; }
            set { SetProperty(ref _imgTypeRockFilter, value); }
        }

        private string _backgroundColorRock = "#FFFFFF";

        public string BackgroundColorRock
        {
            get { return _backgroundColorRock; }
            set { SetProperty(ref _backgroundColorRock, value); }
        }

        private string _textColorRock = "#000000";

        public string TextColorRock
        {
            get { return _textColorRock; }
            set { SetProperty(ref _textColorRock, value); }
        }
        #endregion

        #region Ghost
        private bool _filtreActiveTypeGhost;

        public bool FiltreActiveTypeGhost
        {
            get { return _filtreActiveTypeGhost; }
            set { SetProperty(ref _filtreActiveTypeGhost, value); }
        }

        private byte[] _imgTypeGhostFilter;

        public byte[] ImgTypeGhostFilter
        {
            get { return _imgTypeGhostFilter; }
            set { SetProperty(ref _imgTypeGhostFilter, value); }
        }

        private string _backgroundColorGhost = "#FFFFFF";

        public string BackgroundColorGhost
        {
            get { return _backgroundColorGhost; }
            set { SetProperty(ref _backgroundColorGhost, value); }
        }

        private string _textColorGhost = "#000000";

        public string TextColorGhost
        {
            get { return _textColorGhost; }
            set { SetProperty(ref _textColorGhost, value); }
        }
        #endregion

        #region Flying
        private bool _filtreActiveTypeFlying;

        public bool FiltreActiveTypeFlying
        {
            get { return _filtreActiveTypeFlying; }
            set { SetProperty(ref _filtreActiveTypeFlying, value); }
        }

        private byte[] _imgTypeFlyingFilter;

        public byte[] ImgTypeFlyingFilter
        {
            get { return _imgTypeFlyingFilter; }
            set { SetProperty(ref _imgTypeFlyingFilter, value); }
        }

        private string _backgroundColorFlying = "#FFFFFF";

        public string BackgroundColorFlying
        {
            get { return _backgroundColorFlying; }
            set { SetProperty(ref _backgroundColorFlying, value); }
        }

        private string _textColorFlying = "#000000";

        public string TextColorFlying
        {
            get { return _textColorFlying; }
            set { SetProperty(ref _textColorFlying, value); }
        }
        #endregion

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
