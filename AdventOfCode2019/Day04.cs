using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day04
    {
        public static int PossiblePasswordCount(int rangeMin, int rangeMax)
        {
            var possiblePasswordCount = 0;
            for (int i = rangeMin; i <= rangeMax; i++)
            {
                var increasing = false;
                var twoSame = false;
                var numberArray = i.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
                var numLenght = numberArray.Count();
                int j;
                for (j = 0; j < numLenght; j++)
                {
                    if (j + 1 < numLenght)
                    {
                        if (numberArray[j] > numberArray[j + 1])
                        {
                            break;
                        }
                    }
                }

                if (j == numLenght)
                {
                    increasing = true;
                }

                for (j = 0; j < numLenght; j++)
                {
                    if (j + 1 < numLenght)
                    {
                        if (numberArray[j] == numberArray[j + 1])
                        {
                            twoSame = true;
                            break;
                        }
                    }
                }

                if (increasing && twoSame)
                {
                    possiblePasswordCount += 1;
                }
            }
            return possiblePasswordCount;
        }

        public static int PossiblePasswordCountExtendedRules(int rangeMin, int rangeMax)
        {
            var possiblePasswordCount = 0;
            for (int i = rangeMin; i <= rangeMax; i++)
            {
                if (i==444455)
                {
                    var aa = 1;
                }
                var increasing = false;
                var twoSame = false;
                var numberArray = i.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
                var numLenght = numberArray.Count();
                int j;
                for (j = 0; j < numLenght; j++)
                {
                    if (j + 1 < numLenght)
                    {
                        if (numberArray[j] > numberArray[j + 1])
                        {
                            break;
                        }
                    }
                }

                if (j == numLenght)
                {
                    increasing = true;
                }

                var sameCnt = 0;
                var sameTotalCnt = 0;
                for (j = 0; j < numLenght; j++)
                {
                    if (j + 1 < numLenght)
                    {
                        if (numberArray[j] == numberArray[j + 1])
                        {
                            sameCnt += 1;
                        }
                        else
                        {
                            if (sameCnt == 1)
                            {
                                sameTotalCnt += 1;
                            }
                            sameCnt = 0;
                        }
                    }
                }

                if (increasing && (sameCnt == 1 || sameTotalCnt > 0))
                {
                    possiblePasswordCount += 1;
                }
            }
            return possiblePasswordCount;
        }

    }
}
