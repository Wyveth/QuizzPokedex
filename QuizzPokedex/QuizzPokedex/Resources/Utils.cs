using Android.Content.Res;
using MvvmCross.Navigation;
using QuizzPokedex.Models;
using QuizzPokedex.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static async Task RedirectQuizz(IMvxNavigationService _navigation, QuestionAnswers questionAnswers, Question question = null, QuestionType questionType = null)
        {
            if (question != null)
            {
                if (questionType.Code.Equals(Constantes.QTypPok))
                    await _navigation.Navigate<QTypPokQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTypPok))
                    await _navigation.Navigate<QTypTypPokQuizzViewModel, QuestionAnswers>(questionAnswers);
                else if (questionType.Code.Equals(Constantes.QTypTyp))
                    await _navigation.Navigate<QTypTypQuizzViewModel, QuestionAnswers>(questionAnswers);
            }
            else
                await _navigation.Navigate<ResultQuizzViewModel, QuestionAnswers>(questionAnswers);
        }
    }
}
