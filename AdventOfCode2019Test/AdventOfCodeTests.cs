using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using AdventOfCode2019;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019Test
{
    [Collection("http client collection")]
    public class Day1Tests
    {

        private readonly HttpClientFixture fixture;

        public Day1Tests(HttpClientFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Day01_Part1_Test()
        {
            int sum = 0;
            var result = await fixture.Client.GetAsync("/2019/day/1/input");
            result.EnsureSuccessStatusCode();
            var inputs = await result.Content.ReadAsStringAsync();

            foreach (var input in inputs.Split("\n"))
            {
                try
                {
                    sum += Day01.Puzzle01(int.Parse(input.Trim()));
                }
                catch (Exception)
                {
                }
            }

            Assert.Equal(3299598, sum);
        }

        [Fact]
        public async Task Day01_Part2_Test()
        {
            int sum = 0;
            var result = await fixture.Client.GetAsync("/2019/day/1/input");
            result.EnsureSuccessStatusCode();
            var inputs = await result.Content.ReadAsStringAsync();

            foreach (var input in inputs.Split("\n"))
            {
                try
                {
                    var moduleResult = Day01.Puzzle01(int.Parse(input.Trim()));
                    while (moduleResult > 0)
                    {
                        sum += moduleResult;
                        moduleResult = Day01.Puzzle01(moduleResult);
                    }
                }
                catch (Exception)
                {
                }
            }

            Assert.Equal(4946546, sum);
        }

        [Fact]
        public async Task Day02_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/2/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => int.Parse(v)).ToArray();
            program[1] = 12;
            program[2] = 2;
            var output = Day02.ProcessIntcode(program);
            Assert.Equal(7210630, output[0]);
        }

        [Fact]
        public async Task Day02_Part2_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/2/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => int.Parse(v)).ToArray();
            for (int i = 0; i < program.Count(); i++)
            {
                for (int j = 0; j < program.Count(); j++)
                {
                    program[1] = i;  // noun
                    program[2] = j;  // verb
                    var output = Day02.ProcessIntcode(program);
                    if (output[0] == 19690720)
                    {
                        Assert.Equal(3892, 100 * i + j);
                    }
                }
            }
        }

        [Fact]
        public async Task Day03_Part1_Test()
        {

            var input = "R1005,D32,R656,U228,L629,U59,L558,D366,L659,D504,R683,U230,R689,U489,R237,U986,L803,U288,R192,D473,L490,U934,L749,D631,L333,U848,L383,D363,L641,D499,R926,D945,L520,U311,R75,D414,L97,D338,L754,U171,R601,D215,R490,U164,R158,U499,L801,U27,L671,D552,R406,U168,R12,D321,L97,U27,R833,U503,R950,U432,L688,U977,R331,D736,R231,U301,L579,U17,R984,U399,L224,U100,L266,U184,R46,D989,L851,D739,R45,D231,R893,D372,L260,U26,L697,U423,L716,D573,L269,U867,R722,U193,R889,D322,L743,U371,L986,D835,R534,U170,R946,U271,L514,D521,L781,U390,L750,D134,L767,U599,L508,U683,L426,U433,L405,U10,L359,D527,R369,D365,L405,D812,L979,D122,L782,D460,R583,U765,R502,D2,L109,D69,L560,U76,R130,D794,R197,D113,L602,D123,L190,U246,L407,D957,L35,U41,L884,D591,R38,D911,L269,D204,R332,U632,L826,D202,L984,U153,L187,U472,R272,U232,L786,U932,L618,U104,R632,D469,L868,D451,R261,U647,L211,D781,R609,D549,L628,U963,L917,D716,L218,U71,L148,U638,R34,U133,R617,U312,L215,D41,L673,U643,R379,U486,L273,D539,L294,D598,L838,D60,L158,U817,R207,U825,L601,D786,R225,D89,L417,U481,L416,U133,R261,U405,R109,U962,R104,D676,R966,U138,L343,U14,L82,U564,R73,D361,R678,D868,L273,D879,R629,U164,R228,U949,R504,D254,L662,D726,R126,D437,R569,D23,R246,U840,R457,D429,R296,U110,L984,D106,L44,U264,R801,D350,R932,D334,L252,U714,L514,U261,R632,D926,R944,U924,R199,D181,L737,U408,R636,U57,L380,D949,R557,U28,L432,D83,R829,D865,L902,D351,R71,U704,R477,D501,L882,D75,R325,D53,L990,U460,R165,D82,R577,D788,R375,U264,L178,D193,R830,D343,L394\nL1003,U125,L229,U421,R863,D640,L239,U580,R342,U341,R989,U732,R51,U140,L179,U60,R483,D575,R49,U220,L284,U336,L905,U540,L392,U581,L570,U446,L817,U694,R923,U779,R624,D387,R495,D124,R862,D173,R425,D301,L550,D605,R963,U503,R571,U953,L878,D198,L256,D77,R409,D752,R921,D196,R977,U86,L842,U155,R987,D39,L224,U433,L829,D99,R558,U736,R645,D335,L52,D998,L613,D239,R470,U79,R839,D71,L753,U127,R135,D429,R729,U71,L151,U875,R668,D220,L501,D822,R306,D557,R461,U942,R59,U14,R353,D546,R409,D261,R204,U873,L847,U936,R611,U487,R474,U406,R818,U838,L301,D684,R861,D738,L265,D214,R272,D702,L145,U872,R345,D623,R200,D186,R407,U988,L608,U533,L185,D287,L549,U498,L630,U295,L425,U517,L263,D27,R697,U177,L615,U960,L553,U974,L856,U716,R126,D819,L329,D233,L212,U232,L164,D712,R316,D682,L641,U676,L535,U783,R39,U953,R39,U511,R837,U325,R391,U401,L642,U435,R626,U801,R876,D849,R448,D8,R74,U238,L186,D558,L648,D258,R262,U7,L510,U178,L183,U415,L631,D162,L521,D910,R462,U789,R885,D822,R908,D879,R614,D119,L570,U831,R993,U603,L118,U764,L414,U39,R14,U189,L415,D744,R897,U714,R326,U348,R822,U98,L357,D478,L464,D851,L545,D241,L672,U197,R156,D916,L246,U578,R4,U195,R82,D402,R327,D429,R119,U661,L184,D122,R891,D499,L808,U519,L36,U323,L259,U479,L647,D354,R891,D320,R653,U772,L158,U608,R149,U564,L164,D998,L485,U107,L145,U834,R846,D462,L391,D661,R841,U742,L597,D937,L92,U877,L350,D130,R684,U914,R400,D910,L739,U789,L188,U256,R10,U258,L965,U942,R234,D106,R852,U108,R732,U339,L955,U271,L340,U23,R373,D100,R137,U648,L130";
            var wireString = input.Split("\n");
            var wire1 = wireString[0].Split(",");
            var wire2 = wireString[1].Split(",");
            var closestDistance = Day03.CalculateClosestCrossing(new[] { wire1, wire2 });

            Assert.Equal(375, closestDistance);
        }

        [Fact]
        public async Task Day03_Part2_Test()
        {

            var input = "R1005,D32,R656,U228,L629,U59,L558,D366,L659,D504,R683,U230,R689,U489,R237,U986,L803,U288,R192,D473,L490,U934,L749,D631,L333,U848,L383,D363,L641,D499,R926,D945,L520,U311,R75,D414,L97,D338,L754,U171,R601,D215,R490,U164,R158,U499,L801,U27,L671,D552,R406,U168,R12,D321,L97,U27,R833,U503,R950,U432,L688,U977,R331,D736,R231,U301,L579,U17,R984,U399,L224,U100,L266,U184,R46,D989,L851,D739,R45,D231,R893,D372,L260,U26,L697,U423,L716,D573,L269,U867,R722,U193,R889,D322,L743,U371,L986,D835,R534,U170,R946,U271,L514,D521,L781,U390,L750,D134,L767,U599,L508,U683,L426,U433,L405,U10,L359,D527,R369,D365,L405,D812,L979,D122,L782,D460,R583,U765,R502,D2,L109,D69,L560,U76,R130,D794,R197,D113,L602,D123,L190,U246,L407,D957,L35,U41,L884,D591,R38,D911,L269,D204,R332,U632,L826,D202,L984,U153,L187,U472,R272,U232,L786,U932,L618,U104,R632,D469,L868,D451,R261,U647,L211,D781,R609,D549,L628,U963,L917,D716,L218,U71,L148,U638,R34,U133,R617,U312,L215,D41,L673,U643,R379,U486,L273,D539,L294,D598,L838,D60,L158,U817,R207,U825,L601,D786,R225,D89,L417,U481,L416,U133,R261,U405,R109,U962,R104,D676,R966,U138,L343,U14,L82,U564,R73,D361,R678,D868,L273,D879,R629,U164,R228,U949,R504,D254,L662,D726,R126,D437,R569,D23,R246,U840,R457,D429,R296,U110,L984,D106,L44,U264,R801,D350,R932,D334,L252,U714,L514,U261,R632,D926,R944,U924,R199,D181,L737,U408,R636,U57,L380,D949,R557,U28,L432,D83,R829,D865,L902,D351,R71,U704,R477,D501,L882,D75,R325,D53,L990,U460,R165,D82,R577,D788,R375,U264,L178,D193,R830,D343,L394\nL1003,U125,L229,U421,R863,D640,L239,U580,R342,U341,R989,U732,R51,U140,L179,U60,R483,D575,R49,U220,L284,U336,L905,U540,L392,U581,L570,U446,L817,U694,R923,U779,R624,D387,R495,D124,R862,D173,R425,D301,L550,D605,R963,U503,R571,U953,L878,D198,L256,D77,R409,D752,R921,D196,R977,U86,L842,U155,R987,D39,L224,U433,L829,D99,R558,U736,R645,D335,L52,D998,L613,D239,R470,U79,R839,D71,L753,U127,R135,D429,R729,U71,L151,U875,R668,D220,L501,D822,R306,D557,R461,U942,R59,U14,R353,D546,R409,D261,R204,U873,L847,U936,R611,U487,R474,U406,R818,U838,L301,D684,R861,D738,L265,D214,R272,D702,L145,U872,R345,D623,R200,D186,R407,U988,L608,U533,L185,D287,L549,U498,L630,U295,L425,U517,L263,D27,R697,U177,L615,U960,L553,U974,L856,U716,R126,D819,L329,D233,L212,U232,L164,D712,R316,D682,L641,U676,L535,U783,R39,U953,R39,U511,R837,U325,R391,U401,L642,U435,R626,U801,R876,D849,R448,D8,R74,U238,L186,D558,L648,D258,R262,U7,L510,U178,L183,U415,L631,D162,L521,D910,R462,U789,R885,D822,R908,D879,R614,D119,L570,U831,R993,U603,L118,U764,L414,U39,R14,U189,L415,D744,R897,U714,R326,U348,R822,U98,L357,D478,L464,D851,L545,D241,L672,U197,R156,D916,L246,U578,R4,U195,R82,D402,R327,D429,R119,U661,L184,D122,R891,D499,L808,U519,L36,U323,L259,U479,L647,D354,R891,D320,R653,U772,L158,U608,R149,U564,L164,D998,L485,U107,L145,U834,R846,D462,L391,D661,R841,U742,L597,D937,L92,U877,L350,D130,R684,U914,R400,D910,L739,U789,L188,U256,R10,U258,L965,U942,R234,D106,R852,U108,R732,U339,L955,U271,L340,U23,R373,D100,R137,U648,L130";
            var wireString = input.Split("\n");
            var wire1 = wireString[0].Split(",");
            var wire2 = wireString[1].Split(",");
            var shortestLenght = Day03Part2.CalculateNearestCrossing(new[] { wire1, wire2 });

            Assert.Equal(14746, shortestLenght);
        }

        [Fact]
        public async Task Day04_Part1_Test()
        {
            var maxValidPasswords = Day04.PossiblePasswordCount(359282, 820401);

            Assert.Equal(511, maxValidPasswords);
        }

        [Fact]
        public async Task Day04_Part2_Test()
        {
            var maxValidPasswords = Day04.PossiblePasswordCountExtendedRules(359282, 820401);

            Assert.Equal(316, maxValidPasswords);
        }

        [Fact]
        public async Task Day05_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/5/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => int.Parse(v)).ToArray();
            var programAfter = Day05.ProcessIntcode(program, 1, out var output);
            Assert.Equal(7692125, output);
        }

        [Fact]
        public async Task Day05_Part2_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/5/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => int.Parse(v)).ToArray();
            var programAfter = Day05.ProcessIntcode(program, 5, out var output);
            Assert.Equal(14340395, output);
        }

        [Fact]
        public async Task Day06_Part1_Test()
        {
            //var input = "COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L";
            var result = await fixture.Client.GetAsync("/2019/day/6/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var checkSum = Day06.CalculateOrbitCountCheckSum(input, out _);
            Assert.Equal(142915, checkSum);
        }

        [Fact]
        public async Task Day06_Part2_Test()
        {
            //var input = "COM)B\nB)C\nC)D\nD)E\nE)F\nB)G\nG)H\nD)I\nE)J\nJ)K\nK)L";
            var result = await fixture.Client.GetAsync("/2019/day/6/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var checkSum = Day06.CalculateOrbitCountCheckSum(input, out var orbits);
            var orbitTransfers = Day06_Part2.OrbitalTransfers(orbits, "YOU", "SAN");
            Assert.Equal(283, orbitTransfers);
        }


        [Fact]
        public async Task Day07_Part1_Test()
        {
            var useRealInput = true;
            var input = string.Empty;
            if (useRealInput)
            {
                var result = await fixture.Client.GetAsync("/2019/day/7/input");
                result.EnsureSuccessStatusCode();
                input = await result.Content.ReadAsStringAsync();
            }
            else
            {
                input = "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0";
                input = "3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0";
            }
            var inputArray = input.Split(",");
            var instructions = inputArray.Select(v => int.Parse(v)).ToArray();

            var ampA = new AdventOfCode2019.Day07.IntCodeComputer("Amp A");
            var ampB = new AdventOfCode2019.Day07.IntCodeComputer("Amp B");
            var ampC = new AdventOfCode2019.Day07.IntCodeComputer("Amp C");
            var ampD = new AdventOfCode2019.Day07.IntCodeComputer("Amp D");
            var ampE = new AdventOfCode2019.Day07.IntCodeComputer("Amp E");

            var maxOutput = int.MinValue;

            for (int input1 = 0; input1 <= 4; input1++)
            {
                var output1 = ampA.ProcessIntcode(instructions, new[] { input1, 0 });

                for (int input2 = 0; input2 <= 4; input2++)
                {
                    int output2 = 0;
                    if (input2 != input1)
                    {
                        output2 = ampB.ProcessIntcode(instructions, new[] { input2, output1 });
                    }
                    else
                    {
                        continue;
                    }

                    for (int input3 = 0; input3 <= 4; input3++)
                    {
                        int output3 = 0;
                        if (input3 != input2 && input3 != input1)
                        {
                            output3 = ampC.ProcessIntcode(instructions, new[] { input3, output2 });
                        }
                        else
                        {
                            continue;
                        }

                        for (int input4 = 0; input4 <= 4; input4++)
                        {
                            int output4 = 0;
                            if (input4 != input3 && input4 != input2 && input4 != input1)
                            {
                                output4 = ampD.ProcessIntcode(instructions, new[] { input4, output3 });
                            }
                            else
                            {
                                continue;
                            }

                            for (int input5 = 0; input5 <= 4; input5++)
                            {
                                var output5 = 0;
                                if (input5 != input4 && input5 != input3 && input5 != input2 && input5 != input1)
                                {
                                    output5 = ampE.ProcessIntcode(instructions, new[] { input5, output4 });
                                }
                                else
                                {
                                    continue;
                                }
                                if (output5 > maxOutput)
                                {
                                    maxOutput = output5;
                                }
                            }
                        }
                    }
                }
            }

            Assert.Equal(437860, maxOutput);
        }


        [Fact]
        public async Task Day07_Part2_Test()
        {
            var useRealInput = true;
            var input = string.Empty;
            if (useRealInput)
            {
                var result = await fixture.Client.GetAsync("/2019/day/7/input");
                result.EnsureSuccessStatusCode();
                input = await result.Content.ReadAsStringAsync();
            }
            else
            {
                input = "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5";
            }
            var inputArray = input.Split(",");
            var instructions = inputArray.Select(v => int.Parse(v)).ToArray();
            var maxOutput = int.MinValue;

            for (int input1 = 5; input1 <= 9; input1++)
            {
                for (int input2 = 5; input2 <= 9; input2++)
                {
                    if (input2 == input1)
                    {
                        continue;
                    }

                    for (int input3 = 5; input3 <= 9; input3++)
                    {
                        if (input3 == input2 || input3 == input1)
                        {
                            continue;
                        }

                        for (int input4 = 5; input4 <= 9; input4++)
                        {
                            if (input4 == input3 || input4 == input2 || input4 == input1)
                            {
                                continue;
                            }

                            for (int input5 = 5; input5 <= 9; input5++)
                            {
                                if (input5 == input4 || input5 == input3 || input5 == input2 || input5 == input1)
                                {
                                    continue;
                                }
                                else
                                {
                                    var ampA = new AdventOfCode2019.Day07.IntCodeComputerStatefull("Amp A", instructions, new[] { input1, 0 });
                                    var (output1, state1) = ampA.RunProgram();
                                    var ampB = new AdventOfCode2019.Day07.IntCodeComputerStatefull("Amp B", instructions, new[] { input2 });
                                    var (output2, state2) = ampB.RunProgram(new[] { output1 });
                                    var ampC = new AdventOfCode2019.Day07.IntCodeComputerStatefull("Amp C", instructions, new[] { input3 });
                                    var (output3, state3) = ampC.RunProgram(new[] { output2 });
                                    var ampD = new AdventOfCode2019.Day07.IntCodeComputerStatefull("Amp D", instructions, new[] { input4 });
                                    var (output4, state4) = ampD.RunProgram(new[] { output3 });
                                    var ampE = new AdventOfCode2019.Day07.IntCodeComputerStatefull("Amp E", instructions, new[] { input5 });
                                    var (output5, state5) = ampE.RunProgram(new[] { output4 });

                                    while (state5 == AdventOfCode2019.Day07.ProcessState.Paused)
                                    {
                                        (output1, state1) = ampA.RunProgram(new[] { output5 });
                                        (output2, state2) = ampB.RunProgram(new[] { output1 });
                                        (output3, state3) = ampC.RunProgram(new[] { output2 });
                                        (output4, state4) = ampD.RunProgram(new[] { output3 });
                                        (output5, state5) = ampE.RunProgram(new[] { output4 });
                                    }

                                    if (output5 > maxOutput)
                                    {
                                        maxOutput = output5;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Assert.Equal(49810599, maxOutput);
        }

        [Fact]
        public async Task Day08_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/8/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var image = Day08.DecodeImageData(input, imageWidth: 25, imageHeight: 6);
            var checksum = Day08.Checksum(image);
            Assert.Equal(2064, checksum);
        }

        [Fact]
        public async Task Day08_Part2_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/8/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var imageLayers = Day08.DecodeImageData(input, imageWidth: 25, imageHeight: 6);
            var image = Day08.MergeLayers(imageLayers);

            for (int j = 0; j < image.GetLength(1); j++)
            {
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    if (image[i, j] == Day08.Black)
                    {
                        Debug.Write("@");
                    }
                    else
                    {
                        Debug.Write(".");
                    }

                }
                Debug.Write(Environment.NewLine);
            }

            // output:
            //.@@.@@..@@.@@.@....@@..@@
            //.@.@@.@@.@.@@.@@@@.@.@@.@
            //..@@@.@@.@.@@.@@@.@@.@@.@
            //.@.@@....@.@@.@@.@@@....@
            //.@.@@.@@.@.@@.@.@@@@.@@.@
            //.@@.@.@@.@@..@@....@.@@.@

            Assert.Equal("KAUZA", "KAUZA");

            // lets make bmp
            Day08.DrawImage(image, invertColors: true).Save("biosPassword.gif", System.Drawing.Imaging.ImageFormat.Gif);
        }

        [Fact]
        public async Task Day09_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/9/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => long.Parse(v)).ToArray();
            var intCodeProcesser = new AdventOfCode2019.Day09.IntCodeComputerStatefull(program, new[] { 1L });
            var state = AdventOfCode2019.Day09.IntCodeComputerStatefull.ProcessState.Paused;
            var output = 0L;
            while (state != AdventOfCode2019.Day09.IntCodeComputerStatefull.ProcessState.Ended)
            {
                (output, state) = intCodeProcesser.RunProgram();
                Debug.Write($"{output},");
            }

            Assert.Equal(2714716640, output);
        }

        [Fact]
        public async Task Day09_Part2_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/9/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => long.Parse(v)).ToArray();
            var intCodeProcesser = new AdventOfCode2019.Day09.IntCodeComputerStatefull(program, new[] { 2L });
            var state = AdventOfCode2019.Day09.IntCodeComputerStatefull.ProcessState.Paused;
            var output = 0L;
            while (state != AdventOfCode2019.Day09.IntCodeComputerStatefull.ProcessState.Ended)
            {
                (output, state) = intCodeProcesser.RunProgram();
                Debug.Write($"{output},");
            }

            Assert.Equal(58879, output);
        }

        [Fact]
        public async Task Day10_Part1_Test()
        {
            var asteroidMap = ".#..#\n.....\n#####\n....#\n...##";
            asteroidMap = @"......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####";
            asteroidMap = @"#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.";
            asteroidMap = @".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##";

            // Here is the real input. Those before are examples from the puzzle.
            var result = await fixture.Client.GetAsync("/2019/day/10/input");
            result.EnsureSuccessStatusCode();
            asteroidMap = await result.Content.ReadAsStringAsync();

            var mapArray = asteroidMap.Split("\n").SkipLast(1).ToArray();
            var maxAsteroidsInView = Day10.CalculateMaxAsteroidsInView(mapArray);

            Assert.Equal(269, maxAsteroidsInView);
        }

        [Fact]
        public async Task Day10_Part2_Test()
        {
            var asteroidMap = @".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##
";

            // Here is the real input. Those before are examples from the puzzle. (use \r\b split for hard coded test inputs)
            var result = await fixture.Client.GetAsync("/2019/day/10/input");
            result.EnsureSuccessStatusCode();
            asteroidMap = await result.Content.ReadAsStringAsync();

            var mapArray = asteroidMap.Split("\n").SkipLast(1).ToArray();
            var maxAsteroidsInView = Day10.Vaporize200Asteroids(mapArray);

            Assert.Equal(612, maxAsteroidsInView);
        }

        [Fact]
        public async Task Day11_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/11/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => long.Parse(v)).ToArray();

            var painter = new AdventOfCode2019.Day11.Painter(AdventOfCode2019.Day11.Painter.TileColor.Black);
            painter.Paint(program);

            Assert.Equal(2392, painter.TilesPlainted);
        }

        [Fact]
        public async Task Day11_Part2_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/11/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var inputArray = input.Split(",");
            var program = inputArray.Select(v => long.Parse(v)).ToArray();

            var painter = new AdventOfCode2019.Day11.Painter(AdventOfCode2019.Day11.Painter.TileColor.White);
            painter.Paint(program);

            painter.GetImage().Save("license.gif", System.Drawing.Imaging.ImageFormat.Gif);

            Assert.Equal("EGBHLEUE", "EGBHLEUE");
        }

        [Fact]
        public async Task Day12_Part1_Test()
        {
            var result = await fixture.Client.GetAsync("/2019/day/12/input");
            result.EnsureSuccessStatusCode();
            var input = await result.Content.ReadAsStringAsync();
            var totalEnergy = Day12.CalculateMoonSystemsTotalEnergy(input, steps: 1000);
            Assert.Equal(7202, totalEnergy);
        }

        // Day 13 is in console project

        [Fact]
        public async Task Day14_Part1_Test()
        {
            //var result = await fixture.Client.GetAsync("/2019/day/14/input");
            //result.EnsureSuccessStatusCode();
            //var input = await result.Content.ReadAsStringAsync();
            var input = Day14Input;
            var day14 = new Day14();
            var neededOre = day14.CalculateNeededOre(input, neededOutput: (material: "FUEL", amount: 1));
            Assert.Equal(273638, neededOre);
        }

        [Fact]
        public async Task Day14_Part2_Test()
        {
            //var result = await fixture.Client.GetAsync("/2019/day/14/input");
            //result.EnsureSuccessStatusCode();
            //var input = await result.Content.ReadAsStringAsync();
            var input = Day14Input; 
            var totalCollectedOre = 1000000000000;
            var fuelAmount = 0L;
            var fuelLeft = 0L;
            var fuelRight = 4300000L;
            var neededOre = 0L;
            
            // using binary search to find the fuel amount
            while (true)
            {
                if (fuelAmount == fuelLeft + (fuelRight - fuelLeft) / 2)
                {
                    break;
                }
                fuelAmount = fuelLeft + (fuelRight - fuelLeft) / 2;
                neededOre = new Day14().CalculateNeededOre(input, neededOutput: (material: "FUEL", amount: fuelAmount));

                if (totalCollectedOre - neededOre > 0)
                {
                    fuelLeft = fuelAmount;
                }
                else
                {
                    fuelRight = fuelAmount;
                    
                }
            }

            Assert.Equal(4200533, fuelAmount);
        }

        private string Day14Input => @"3 CFGBR, 9 PFMFC, 2 FQFPN => 2 PKPWN
9 XQHK => 3 KDXDC
9 MPQFZ, 6 SGHLD => 6 DXPTR
6 QDBK, 2 SGHLD => 3 WDJKB
1 RXJS, 9 XQHK => 4 GTXPN
2 XJHR, 3 WNFC => 4 NRLM
3 RHWD => 7 NDQK
3 PZXG, 3 TNCBS, 1 GNSW => 7 CFGBR
1 VRMZK => 2 TVDH
3 JQFP => 8 VRMZK
124 ORE => 6 NKRXN
2 KRDMT, 11 MPQFZ => 2 WDWNX
3 ZPCP => 9 WLMB
2 MPQFZ => 1 DQRQW
13 KHXVX => 8 RHWD
5 ZPVWS => 8 JQFP
1 NDQK, 1 JZQN, 1 GNSW => 6 XHRQW
4 KRDMT => 7 HCVLB
3 NRLM => 9 WHWK
172 ORE => 5 ZPCP
104 ORE => 1 TJHD
1 LFPG => 6 TNCBS
3 XJHR => 9 TVBNZ
3 JQFP => 4 DSJK
3 ZPVWS => 3 SGHLD
15 NRLM, 5 KDXDC, 1 DQRQW, 5 WDWNX, 12 RXJS, 3 GTXPN => 5 QTSK
1 WDWNX => 1 GNSW
1 QDBK => 1 LBTRH
3 FQFPN, 13 WDWNX => 4 RXJS
1 QDBK => 7 MPQFZ
6 LBTRH, 6 TVDH => 6 JDKMB
4 KWXF, 8 XJHR => 9 JZQN
8 MPQFZ, 8 VRMZK => 7 WNFC
16 QGZSZ, 9 XHRQW, 17 MRBFL, 10 WHWK, 36 JDKMB, 82 LFNZ, 11 TDRWG, 7 QTSK, 7 MNWVT, 6 CDNHC, 3 NDQK, 4 TNCBS => 1 FUEL
1 DQRQW, 1 MRBFL, 1 GTXPN, 1 CFGBR, 2 HCVLB, 1 DGXBN, 3 GZQSX => 8 QGZSZ
13 SGHLD, 11 XQHK, 17 PKPWN, 1 RXJS, 1 FQFPN, 11 JZQN => 1 CDNHC
21 NKRXN, 9 TJHD, 2 ZXJCJ => 5 KHXVX
2 WLMB => 8 XJKTS
2 WDJKB => 6 KRDMT
2 MGXB, 1 KWXF => 8 LFNZ
1 TVBNZ, 5 VRMZK => 8 CSDWQ
7 LFPG => 8 TDRWG
1 RHWD, 8 XJKTS => 2 QDBK
182 ORE => 4 ZXJCJ
3 ZXJCJ => 8 ZPVWS
1 WNFC, 2 CSDWQ, 2 NRLM => 6 GZQSX
4 TVDH, 2 DGXBN => 6 MRBFL
3 DSJK => 4 FQFPN
9 NDQK, 7 WLMB => 2 KWXF
4 CSDWQ => 2 XQHK
1 NKRXN => 5 PZXG
2 LFPG => 2 DGXBN
7 MGXB => 7 XJHR
2 WLMB => 7 LFPG
8 DXPTR, 7 WNFC, 5 MPQFZ => 9 PFMFC
5 PFMFC, 4 NRLM => 9 MNWVT
7 ZPVWS, 14 ZPCP, 11 TJHD => 2 MGXB";

        [Fact]
        public async Task Day15_Part1_Test()
        {
            // not done.. no time at the moment.. will look into it later. 
            // thinks should do some maze solver and path findin stuff.
            AdventOfCode2019.Day15.TheDay.LetsDoThis();
        }

        [Fact]
        public async Task Day16_Part1_Test()
        {
            var output = Day16.CalculateFftTheIdiotWay();
            Assert.Equal("29795507", output);
        }

        [Fact]
        public async Task Day17_Part1_Test()
        {
            var calibration = Day17.CalibrateCamera();
            Assert.Equal(7280, calibration);
        }

        [Fact]
        public async Task Day22_Part1_Test()
        {
            var posOf2019 = Day22.ShuffleAndDeal();
            Assert.Equal(2306, posOf2019);
        }

        [Fact]
        public async Task Day23_Part1_Test()
        {
            Day23.Cat6Router();
        }

        [Fact]
        public async Task Day23_Part2_Test()
        {
            Day23.Cat6RouterWithNat();
        }

    }
    public class HttpClientFixture : IDisposable
    {
        private readonly HttpClientHandler handler;

        public HttpClientFixture()
        {
            var baseAddress = new Uri(@"https://adventofcode.com");
            var cookieContainer = new CookieContainer();
            handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            Client = new HttpClient(handler) { BaseAddress = baseAddress };
            cookieContainer.Add(baseAddress, new Cookie("session", Environment.GetEnvironmentVariable("AoCSession", EnvironmentVariableTarget.User)));
        }

        public void Dispose()
        {
            handler.Dispose();
            Client.Dispose();
        }

        public HttpClient Client { get; private set; }
    }

    [CollectionDefinition("http client collection")]
    public class DatabaseCollection : ICollectionFixture<HttpClientFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}

