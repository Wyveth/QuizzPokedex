using Android.Content.Res;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using MvvmCross.Navigation;
using QuizzPokedex.Models;
using QuizzPokedex.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace QuizzPokedex.Resources
{
    public static class Utils
    {
        public static async Task<byte[]> GetByteAssetImage(string fileName)
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

        public static async Task<List<ITransformation>> GetTransformationImage(QuestionType questionType)
        {
            List<ITransformation> transformationList = new List<ITransformation>();

            if (questionType.IsBlurred)
                transformationList.Add(new BlurredTransformation());

            if (questionType.IsGrayscale)
                transformationList.Add(new GrayscaleTransformation());

            if (questionType.IsHide)
                transformationList.Add(new TintTransformation("#000000"));

            return await Task.FromResult(transformationList);
        }

        public static async Task<byte[]> GetBytesDifficulty(Difficulty difficulty)
        {
            byte[] difficultyLogo = null;
            if (difficulty.Libelle.Equals(Constantes.EasyTQ))
                difficultyLogo = await Utils.GetByteAssetImage(Constantes.Easy_Color);
            else if (difficulty.Libelle.Equals(Constantes.NormalTQ))
                difficultyLogo = await Utils.GetByteAssetImage(Constantes.Normal_Color);
            else if (difficulty.Libelle.Equals(Constantes.HardTQ))
                difficultyLogo = await Utils.GetByteAssetImage(Constantes.Hard_Color);

            return await Task.FromResult(difficultyLogo);
        }

        public static async Task<int> GetTransformationImageDelay(QuestionType questionType)
        {
            int i = 15;

            if (questionType.IsBlurred)
                i = 600;

            return await Task.FromResult(i);
        }

        public static string GetValueStat(Pokemon pokemon, string typeStat)
        {
            string libelle = string.Empty;
            if (typeStat.Equals(Constantes.Pv))
                libelle = pokemon.StatPv.ToString();
            else if (typeStat.Equals(Constantes.Attaque))
                libelle = pokemon.StatAttaque.ToString();
            else if (typeStat.Equals(Constantes.Defense))
                libelle = pokemon.StatDefense.ToString();
            else if (typeStat.Equals(Constantes.AttaqueSpe))
                libelle = pokemon.StatAttaqueSpe.ToString();
            else if (typeStat.Equals(Constantes.DefenseSpe))
                libelle = pokemon.StatDefenseSpe.ToString();
            else if (typeStat.Equals(Constantes.Vitesse))
                libelle = pokemon.StatVitesse.ToString();

            return libelle;
        }

        public static async Task RedirectQuizz(IMvxNavigationService _navigation, QuestionAnswers questionAnswers, Question question = null, QuestionType questionType = null)
        {
            if (question != null)
            {
                if (questionType.Code.Equals(Constantes.QTypPok) 
                    || questionType.Code.Equals(Constantes.QTypPokBlurred)
                    || questionType.Code.Equals(Constantes.QTypPokBlack))
                    await _navigation.Navigate<QTypPokQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypPokDesc))
                    await _navigation.Navigate<QTypPokDescQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypPokDescReverse))
                    await _navigation.Navigate<QTypPokDescReverseQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTalent) || questionType.Code.Equals(Constantes.QTypTalentReverse))
                    await _navigation.Navigate<QTypTalentQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTypPok))
                    await _navigation.Navigate<QTypTypPokQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTypPokVarious))
                    await _navigation.Navigate<QTypTypPokVariousQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypWeakPokVarious))
                    await _navigation.Navigate<QTypWeakPokVariousQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTalentPokVarious))
                    await _navigation.Navigate<QTypTalentPokVariousQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTyp))
                    await _navigation.Navigate<QTypTypQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypPokStat))
                    await _navigation.Navigate<QTypPokStatQuizzViewModel, QuestionAnswers>(questionAnswers);
            }
            else
                await _navigation.Navigate<ResultQuizzViewModel, QuestionAnswers>(questionAnswers);
        }
    }
}
