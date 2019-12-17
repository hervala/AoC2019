using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2019
{
    public class Day16
    {
        public static string CalculateFftTheIdiotWay()
        {
            var input = @"59731816011884092945351508129673371014862103878684944826017645844741545300230138932831133873839512146713127268759974246245502075014905070039532876129205215417851534077861438833829150700128859789264910166202535524896960863759734991379392200570075995540154404564759515739872348617947354357737896622983395480822393561314056840468397927687908512181180566958267371679145705350771757054349846320639601111983284494477902984330803048219450650034662420834263425046219982608792077128250835515865313986075722145069152768623913680721193045475863879571787112159970381407518157406924221437152946039000886837781446203456224983154446561285113664381711600293030463013";
            //input = "12345678"; // phases 4
            //input = "80871224585914546619083218645595";  // phases 100
            var inputLen = input.Length;
            var pattern = new int[] { 0, 1, 0, -1 };
            var patternIndex = 0;
            var patternRepeat = 0;
            var patternMatrix = new int[inputLen, inputLen];
            for (int i = 0; i < inputLen; i++)
            {
                patternIndex = 0;
                patternRepeat = i + 1;
                for (int j = 0; j < inputLen; j++)
                {
                    if (j == 0)
                    {
                        patternRepeat -= 1;  // remove first
                    }
                    if (patternRepeat == 0)
                    {
                        patternIndex += 1;
                        patternIndex = patternIndex % 4;
                        patternRepeat = i + 1;
                    }
                    patternMatrix[i, j] = pattern[patternIndex];
                    patternRepeat -= 1;
                }
            }

            var signalMatrix = new int[inputLen, inputLen];
            for (int j = 0; j < inputLen; j++)
            {
                for (int i = 0; i < inputLen; i++)
                {
                    signalMatrix[i, j] = int.Parse(input.Substring(i, 1));
                }
            }

            var product = Multiply(patternMatrix, signalMatrix);

            for (int phase = 1; phase < 100; phase++)
            {
                for (int i = 0; i < inputLen; i++)
                {
                    for (int j = 0; j < inputLen; j++)
                    {
                        signalMatrix[j, i] = Math.Abs(product[j, i]) % 10;
                    }
                }
                product = Multiply(patternMatrix, signalMatrix);
                if (phase % 10 == 0)
                    Debug.WriteLine(DateTime.Now.ToString());
            }

            var output = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                output += Math.Abs(product[i, 0]) % 10;
            }

            return output;

        }

        private static int[,] Multiply(int[,] matrixA, int[,] matrixB)
        {
            if (matrixA.GetLength(1) == matrixB.GetLength(0))
            { 
                var product = new int[matrixA.GetLength(0), matrixB.GetLength(1)];
                for (int i = 0; i < product.GetLength(0); i++)
                {
                    for (int j = 0; j < product.GetLength(1); j++)
                    {
                        //product[i, j] = 0;
                        for (int k = 0; k < matrixA.GetLength(1); k++)
                        {
                            product[i, j] = product[i, j] + (matrixA[i, k] * matrixB[k, j]);
                        }
                    }
                }
                return product;
            }
            else
            {
                throw new Exception("Illegal dimensions for multiplying matrixes");
            }
        }

        private static int[] Multiply1(int[,] matrixA, int[,] matrixB)
        {
            if (matrixA.GetLength(1) == matrixB.GetLength(0))
            {
                var product = new int[matrixA.GetLength(0)];
                for (int i = 0; i < product.GetLength(0); i++)
                {
                    for (int j = 0; j < product.GetLength(0); j++)
                    {
                        //product[i, j] = 0;
                        for (int k = 0; k < matrixA.GetLength(1); k++)
                        {
                            product[i] = product[i] + (matrixA[i, k] * matrixB[k, j]);
                        }
                    }
                }
                return product;
            }
            else
            {
                throw new Exception("Illegal dimensions for multiplying matrixes");
            }
        }

    }

    public static class F
    {
        public static string Dump(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
