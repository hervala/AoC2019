using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AdventOfCode2019
{
    public class Day08
    {
        public static List<int[,]> DecodeImageData(string data, int imageWidth, int imageHeight)
        {
            var imageArray = data
                .ToCharArray()
                .Select(c => int.TryParse(c.ToString(), out var intValue) ? intValue : -1)
                .Where(i => i >= 0)
                .ToArray();
            var index = 0;
            var dataLenght = imageArray.Length;
            var imageLayers = new List<int[,]>();

            if (dataLenght % (imageHeight*imageWidth) != 0)
            {
                throw new Exception("image is corrupted!");
            }

            while (index < dataLenght)
            {
                // new layer
                var layer = new int[imageWidth, imageHeight];
                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        layer[x, y] = imageArray[index];
                        index += 1;
                    }
                }
                imageLayers.Add(layer);
            }
            return imageLayers;
        }

        public static int Checksum(List<int[,]> imageData)
        {
            var layerDigitCounts = new List<int[]>();
            foreach (var layer in imageData)
            {
                var digitCount = new int[10];
                for (int i = 0; i < layer.GetLength(0); i++)
                {
                    for (int j = 0; j < layer.GetLength(1); j++)
                    {
                        digitCount[layer[i, j]] += 1;
                    }
                }
                layerDigitCounts.Add(digitCount);
            }

            var layerWithMinimumZeros = -1;
            var minZeros = int.MaxValue;
            for (int i = 0; i < layerDigitCounts.Count; i++)
            {
                if (layerDigitCounts[i][0] < minZeros)
                {
                    minZeros = layerDigitCounts[i][0];
                    layerWithMinimumZeros = i;
                }
            }

            if (layerWithMinimumZeros > -1)
            {
                return layerDigitCounts[layerWithMinimumZeros][1] * layerDigitCounts[layerWithMinimumZeros][2];
            }
            else
            {
                throw new Exception("something went wrong");
            }
        }

        public static int[,] MergeLayers(List<int[,]> imageData)
        {
            var image = new int[imageData[0].GetLength(0), imageData[0].GetLength(1)];
            for (int i = 0; i < imageData[0].GetLength(0); i++)
            {
                for (int j = 0; j < imageData[0].GetLength(1); j++)
                {
                    image[i, j] = Transparent;
                }
            }

            foreach (var layer in imageData)
            {
                for (int i = 0; i < layer.GetLength(0); i++)
                {
                    for (int j = 0; j < layer.GetLength(1); j++)
                    {
                        if (image[i,j] == Transparent)
                        {
                            image[i, j] = layer[i, j];
                        }
                    }
                }
            }

            return image;
        }


        public const int Black = 0;

        public const int White = 1;

        public const int Transparent = 2;


    }
}
