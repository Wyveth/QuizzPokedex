using Android.Content.Res;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QuizzPokedex.Resources
{
    public static class Utils
    {
        public static async Task<byte[]> getByteAssetImage(string fileName)
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
    }
}
