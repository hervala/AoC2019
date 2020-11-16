using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day22
    {
        private const int deckSize = 10007;
        public static int ShuffleAndDeal()
        {
            var deck = Enumerable.Range(0, deckSize).ToArray();

            foreach (var shuffle in input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (shuffle.Contains("deal with increment"))
                {
                    var increment = int.Parse(shuffle.Substring(shuffle.LastIndexOf(' ')));
                    var newDeck = new int[deckSize];
                    var newPosition = 0;
                    for (int i = 0; i < deckSize; i++)
                    {
                        newDeck[newPosition] = deck[i];
                        newPosition = (newPosition + increment) % deckSize;
                    }
                    deck = newDeck.ToArray();
                }
                else if (shuffle.Contains("deal into new stack"))
                {
                    var stack = new Stack<int>();
                    for (int i = 0; i < deckSize; i++)
                    {
                        stack.Push(deck[i]);
                    }
                    deck = stack.ToArray();
                }
                else if (shuffle.Contains("cut"))
                {
                    var cutAmount = int.Parse(shuffle.Substring(shuffle.LastIndexOf(' ')));
                    if (cutAmount >= 0)
                    {
                        var cutLeft = deck[..cutAmount];
                        var cutRight = deck[cutAmount..];
                        deck = cutRight.Concat(cutLeft).ToArray();
                    }
                    else
                    {
                        cutAmount = Math.Abs(cutAmount);
                        var cutLeft = deck[..^cutAmount];
                        var cutRight = deck[^cutAmount..];
                        deck = cutRight.Concat(cutLeft).ToArray();
                    }
                }

            }

            var posOf2019 = 0;
            for (int i = 0; i < deckSize; i++)
            {
                if (deck[i] == 2019)
                {
                    posOf2019 = i;
                    break;
                }
            }

            return posOf2019;

        }

        private static string testInput = @"deal into new stack
cut -2
deal with increment 7
cut 8
cut -4
deal with increment 7
cut 3
deal with increment 9
deal with increment 3
cut -1";

        private static string input = @"deal with increment 21
deal into new stack
deal with increment 52
deal into new stack
deal with increment 19
deal into new stack
deal with increment 65
cut -8368
deal into new stack
cut -3820
deal with increment 47
cut -2965
deal with increment 38
deal into new stack
cut 9165
deal with increment 62
cut 3224
deal with increment 72
cut 4120
deal with increment 40
cut -9475
deal with increment 52
cut 4437
deal into new stack
cut 512
deal with increment 33
cut -9510
deal into new stack
cut -6874
deal with increment 56
cut -47
deal with increment 7
deal into new stack
deal with increment 13
cut 4530
deal with increment 67
deal into new stack
cut 8584
deal with increment 26
cut 4809
deal with increment 72
cut -8003
deal with increment 24
cut 1384
deal into new stack
deal with increment 13
deal into new stack
cut -1171
deal with increment 72
cut 6614
deal with increment 61
cut 1412
deal with increment 16
cut -4808
deal into new stack
deal with increment 51
cut -8507
deal with increment 60
cut 1202
deal with increment 2
cut -4030
deal with increment 4
cut -9935
deal with increment 57
cut -6717
deal with increment 5
deal into new stack
cut 3912
deal with increment 64
cut 5152
deal into new stack
deal with increment 68
deal into new stack
cut 49
deal with increment 31
cut 4942
deal with increment 44
cut -4444
deal with increment 47
cut -5533
deal with increment 51
cut -3045
deal with increment 67
cut -1711
deal with increment 46
cut -6247
deal into new stack
cut 969
deal with increment 55
cut 7549
deal with increment 62
cut -9083
deal with increment 54
deal into new stack
cut -3337
deal with increment 62
cut 1777
deal with increment 39
cut 3207
deal with increment 13";

    }
}
