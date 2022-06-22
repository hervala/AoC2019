﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019
{
    public class Day23
    {
        public static void Cat6Router()
        {
            var computers = new List<IntCodeVm>();
            var inputBuffers = new List<BufferBlock<long>>();
            var outputBuffers = new List<BatchBlock<long>>();
            var programTasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                var input = new BufferBlock<long>();
                input.Post(i);
                inputBuffers.Add(input);
                var output = new BatchBlock<long>(batchSize: 3);
                outputBuffers.Add(output);
                var vm = new IntCodeVm(i.ToString(), programStr, input, output);
                computers.Add(vm);
                programTasks.Add(Task.Run(() => vm.RunProgram()));
            }

            var routerTask = Task.Run(() =>
            {
                while(true)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (outputBuffers[i].TryReceive(out var message))
                        {
                            var destAddr = (int)message[0];
                            if (destAddr == 255)
                            {
                                var theAnswer = message[2];
                            }
                            var input = inputBuffers[(int)message[0]];
                            input.Post(message[1]);
                            input.Post(message[2]);
                        }
                        else
                        {
                            inputBuffers[i].Post(-1);
                        }


                    }
                }
            });

            Task.WaitAll(programTasks.ToArray());
            routerTask.Wait();

        }

        public static void Cat6RouterWithNat()
        {
            var computers = new List<IntCodeVm>();
            var inputBuffers = new List<BufferBlock<long>>();
            var outputBuffers = new List<BatchBlock<long>>();
            var programTasks = new List<Task>();
            for (int i = 0; i < 50; i++)
            {
                var input = new BufferBlock<long>();
                input.Post(i);
                inputBuffers.Add(input);
                var output = new BatchBlock<long>(batchSize: 3);
                outputBuffers.Add(output);
                var vm = new IntCodeVm(i.ToString(), programStr, input, output);
                computers.Add(vm);
                programTasks.Add(Task.Run(() => vm.RunProgram()));
            }


            var routerTask = Task.Run(() =>
            {
                (long X, long Y) messageBlock = (0, 0);
                var natHasValue = false;
                var lastY = 0L;
                while (true)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (outputBuffers[i].TryReceive(out var message))
                        {
                            var destAddr = (int)message[0];
                            if (destAddr == 255)
                            {
                                messageBlock.X = message[1];
                                messageBlock.Y = message[2];
                                natHasValue = true;
                            }
                            else
                            {
                                var inputBuffer = inputBuffers[(int)message[0]];
                                inputBuffer.Post(message[1]);
                                inputBuffer.Post(message[2]);
                            }
                            
                        }
                        else
                        {
                            if (natHasValue && inputBuffers.All(i => i.Count == 0))
                            {
                                if (lastY == messageBlock.Y)
                                {
                                    var halt = true;
                                }
                                inputBuffers[0].Post(messageBlock.X);
                                inputBuffers[0].Post(messageBlock.Y);
                                lastY = messageBlock.Y;
                                natHasValue = false;
                            }
                            else
                            {
                                inputBuffers[i].Post(-1);
                            }
                        }


                    }
                }
            });

            Task.WaitAll(programTasks.ToArray());
            routerTask.Wait();

        }


        //var inputBuffer = new BufferBlock<long>();
        //var outputBuffer = new BufferBlock<long>();
        //var vm = new IntCodeVm(programStr, inputBuffer, outputBuffer);
        //var runningProgram = Task.Run(() => vm.RunProgram());

        private static string programStr = @"3,62,1001,62,11,10,109,2247,105,1,0,1196,1418,670,1843,2084,1979,1684,2119,1031,936,608,1878,1746,1272,2150,1942,1101,967,1649,1517,1777,1381,1715,2216,1000,1130,903,1620,2055,1911,707,1062,1348,1237,2012,1163,1313,800,769,1587,571,1455,738,1548,1486,833,2181,1808,639,864,0,0,0,0,0,0,0,0,0,0,0,0,3,64,1008,64,-1,62,1006,62,88,1006,61,170,1105,1,73,3,65,20102,1,64,1,20101,0,66,2,21102,1,105,0,1106,0,436,1201,1,-1,64,1007,64,0,62,1005,62,73,7,64,67,62,1006,62,73,1002,64,2,132,1,132,68,132,1001,0,0,62,1001,132,1,140,8,0,65,63,2,63,62,62,1005,62,73,1002,64,2,161,1,161,68,161,1102,1,1,0,1001,161,1,169,1002,65,1,0,1101,1,0,61,1101,0,0,63,7,63,67,62,1006,62,203,1002,63,2,194,1,68,194,194,1006,0,73,1001,63,1,63,1105,1,178,21102,210,1,0,105,1,69,1201,1,0,70,1101,0,0,63,7,63,71,62,1006,62,250,1002,63,2,234,1,72,234,234,4,0,101,1,234,240,4,0,4,70,1001,63,1,63,1106,0,218,1106,0,73,109,4,21101,0,0,-3,21101,0,0,-2,20207,-2,67,-1,1206,-1,293,1202,-2,2,283,101,1,283,283,1,68,283,283,22001,0,-3,-3,21201,-2,1,-2,1106,0,263,21202,-3,1,-3,109,-4,2106,0,0,109,4,21101,1,0,-3,21101,0,0,-2,20207,-2,67,-1,1206,-1,342,1202,-2,2,332,101,1,332,332,1,68,332,332,22002,0,-3,-3,21201,-2,1,-2,1106,0,312,21201,-3,0,-3,109,-4,2105,1,0,109,1,101,1,68,359,20102,1,0,1,101,3,68,366,21001,0,0,2,21101,376,0,0,1106,0,436,21202,1,1,0,109,-1,2105,1,0,1,2,4,8,16,32,64,128,256,512,1024,2048,4096,8192,16384,32768,65536,131072,262144,524288,1048576,2097152,4194304,8388608,16777216,33554432,67108864,134217728,268435456,536870912,1073741824,2147483648,4294967296,8589934592,17179869184,34359738368,68719476736,137438953472,274877906944,549755813888,1099511627776,2199023255552,4398046511104,8796093022208,17592186044416,35184372088832,70368744177664,140737488355328,281474976710656,562949953421312,1125899906842624,109,8,21202,-6,10,-5,22207,-7,-5,-5,1205,-5,521,21102,0,1,-4,21102,1,0,-3,21101,0,51,-2,21201,-2,-1,-2,1201,-2,385,471,20102,1,0,-1,21202,-3,2,-3,22207,-7,-1,-5,1205,-5,496,21201,-3,1,-3,22102,-1,-1,-5,22201,-7,-5,-7,22207,-3,-6,-5,1205,-5,515,22102,-1,-6,-5,22201,-3,-5,-3,22201,-1,-4,-4,1205,-2,461,1106,0,547,21101,0,-1,-4,21202,-6,-1,-6,21207,-7,0,-5,1205,-5,547,22201,-7,-6,-7,21201,-4,1,-4,1106,0,529,22102,1,-4,-7,109,-8,2105,1,0,109,1,101,1,68,563,21002,0,1,0,109,-1,2105,1,0,1102,57287,1,66,1101,4,0,67,1102,598,1,68,1102,1,302,69,1101,1,0,71,1102,1,606,72,1105,1,73,0,0,0,0,0,0,0,0,13,224855,1102,1,21059,66,1101,0,1,67,1102,635,1,68,1102,556,1,69,1101,1,0,71,1102,1,637,72,1106,0,73,1,7,31,33213,1101,69767,0,66,1102,1,1,67,1102,1,666,68,1102,1,556,69,1102,1,1,71,1102,668,1,72,1105,1,73,1,23117,11,82279,1101,0,69779,66,1101,0,4,67,1101,697,0,68,1101,0,253,69,1101,0,1,71,1101,0,705,72,1106,0,73,0,0,0,0,0,0,0,0,5,113194,1102,1,277,66,1102,1,1,67,1102,1,734,68,1102,556,1,69,1101,0,1,71,1101,736,0,72,1105,1,73,1,10634745,36,3183,1101,39199,0,66,1101,1,0,67,1101,0,765,68,1101,0,556,69,1102,1,1,71,1102,767,1,72,1105,1,73,1,-17027471,36,1061,1102,45979,1,66,1102,1,1,67,1102,796,1,68,1102,1,556,69,1101,0,1,71,1102,798,1,72,1105,1,73,1,-3143,33,26561,1102,1,51383,66,1102,1,1,67,1101,0,827,68,1102,556,1,69,1101,0,2,71,1101,829,0,72,1106,0,73,1,3,15,12553,15,50212,1102,68771,1,66,1102,1,1,67,1101,0,860,68,1101,0,556,69,1102,1,1,71,1102,862,1,72,1106,0,73,1,769,1,31873,1101,0,66959,66,1102,1,5,67,1102,1,891,68,1102,1,302,69,1102,1,1,71,1101,0,901,72,1105,1,73,0,0,0,0,0,0,0,0,0,0,21,89108,1101,0,41257,66,1102,1,1,67,1101,930,0,68,1102,556,1,69,1101,2,0,71,1102,932,1,72,1106,0,73,1,10,40,229148,13,269826,1101,0,61099,66,1101,0,1,67,1102,1,963,68,1102,556,1,69,1102,1,1,71,1101,0,965,72,1105,1,73,1,341,49,200877,1101,0,28859,66,1101,1,0,67,1102,994,1,68,1101,556,0,69,1102,2,1,71,1101,0,996,72,1105,1,73,1,31,49,133918,15,37659,1102,1,40787,66,1102,1,1,67,1101,1027,0,68,1101,556,0,69,1102,1,1,71,1102,1029,1,72,1106,0,73,1,-186,15,25106,1101,20879,0,66,1102,1,1,67,1101,1058,0,68,1102,556,1,69,1101,1,0,71,1101,0,1060,72,1106,0,73,1,125,40,171861,1102,1,11071,66,1101,5,0,67,1102,1089,1,68,1101,0,302,69,1102,1,1,71,1101,0,1099,72,1106,0,73,0,0,0,0,0,0,0,0,0,0,2,209337,1102,1,19861,66,1102,1,1,67,1102,1,1128,68,1102,556,1,69,1102,1,0,71,1101,1130,0,72,1106,0,73,1,1343,1101,14177,0,66,1101,0,2,67,1101,0,1157,68,1102,1,302,69,1102,1,1,71,1102,1,1161,72,1105,1,73,0,0,0,0,21,22277,1102,101891,1,66,1102,1,2,67,1102,1,1190,68,1101,0,302,69,1102,1,1,71,1101,0,1194,72,1106,0,73,0,0,0,0,2,69779,1101,93889,0,66,1102,1,1,67,1102,1,1223,68,1102,556,1,69,1102,1,6,71,1101,1225,0,72,1106,0,73,1,24427,25,14177,46,70667,46,141334,3,37579,3,75158,3,112737,1101,26561,0,66,1101,3,0,67,1102,1264,1,68,1102,1,302,69,1101,1,0,71,1101,0,1270,72,1105,1,73,0,0,0,0,0,0,2,139558,1102,44971,1,66,1101,6,0,67,1101,0,1299,68,1101,302,0,69,1101,1,0,71,1101,0,1311,72,1105,1,73,0,0,0,0,0,0,0,0,0,0,0,0,32,18314,1102,1,1061,66,1102,1,3,67,1102,1340,1,68,1101,0,253,69,1101,0,1,71,1102,1,1346,72,1106,0,73,0,0,0,0,0,0,35,101891,1101,0,9157,66,1101,0,2,67,1101,1375,0,68,1101,351,0,69,1102,1,1,71,1101,1379,0,72,1106,0,73,0,0,0,0,255,93889,1102,1,22277,66,1102,1,4,67,1102,1408,1,68,1102,253,1,69,1102,1,1,71,1102,1,1416,72,1105,1,73,0,0,0,0,0,0,0,0,32,9157,1101,0,31873,66,1102,4,1,67,1101,1445,0,68,1101,0,302,69,1102,1,1,71,1101,1453,0,72,1105,1,73,0,0,0,0,0,0,0,0,2,279116,1101,48989,0,66,1102,1,1,67,1102,1482,1,68,1101,0,556,69,1101,0,1,71,1102,1,1484,72,1106,0,73,1,25,1,63746,1101,0,2389,66,1101,1,0,67,1102,1,1513,68,1102,1,556,69,1102,1,1,71,1101,0,1515,72,1105,1,73,1,263,4,102759,1101,0,41851,66,1101,0,1,67,1102,1544,1,68,1102,1,556,69,1102,1,1,71,1102,1546,1,72,1106,0,73,1,12,49,66959,1101,0,84979,66,1102,1,1,67,1101,0,1575,68,1101,556,0,69,1101,0,5,71,1102,1577,1,72,1105,1,73,1,2,35,203782,5,56597,39,28753,13,44971,13,89942,1102,1,28753,66,1102,2,1,67,1101,0,1614,68,1101,0,302,69,1101,1,0,71,1102,1618,1,72,1106,0,73,0,0,0,0,25,28354,1102,102451,1,66,1101,1,0,67,1102,1647,1,68,1102,556,1,69,1102,1,0,71,1101,1649,0,72,1106,0,73,1,1582,1101,0,48371,66,1101,3,0,67,1101,1676,0,68,1101,302,0,69,1102,1,1,71,1101,0,1682,72,1106,0,73,0,0,0,0,0,0,49,267836,1102,1,53887,66,1101,0,1,67,1102,1,1711,68,1101,556,0,69,1102,1,1,71,1101,1713,0,72,1106,0,73,1,160,13,179884,1102,35969,1,66,1102,1,1,67,1102,1742,1,68,1101,0,556,69,1101,0,1,71,1102,1744,1,72,1105,1,73,1,54,4,68506,1102,6869,1,66,1101,1,0,67,1102,1,1773,68,1101,0,556,69,1102,1,1,71,1102,1,1775,72,1105,1,73,1,49,31,44284,1102,1,3677,66,1101,0,1,67,1102,1,1804,68,1101,0,556,69,1101,1,0,71,1102,1806,1,72,1106,0,73,1,37,31,22142,1102,58763,1,66,1101,0,1,67,1101,0,1835,68,1101,556,0,69,1101,0,3,71,1101,1837,0,72,1106,0,73,1,5,40,57287,40,114574,13,134913,1102,1,37579,66,1101,0,3,67,1101,0,1870,68,1101,302,0,69,1102,1,1,71,1102,1876,1,72,1106,0,73,0,0,0,0,0,0,21,44554,1101,82279,0,66,1101,2,0,67,1101,1905,0,68,1101,302,0,69,1102,1,1,71,1102,1,1909,72,1106,0,73,0,0,0,0,33,53122,1101,0,17489,66,1101,1,0,67,1101,1938,0,68,1102,1,556,69,1101,0,1,71,1101,1940,0,72,1105,1,73,1,5482,31,11071,1102,1,12553,66,1101,4,0,67,1101,1969,0,68,1102,302,1,69,1102,1,1,71,1101,0,1977,72,1106,0,73,0,0,0,0,0,0,0,0,46,212001,1101,56597,0,66,1101,0,2,67,1101,2006,0,68,1102,1,302,69,1101,0,1,71,1102,1,2010,72,1105,1,73,0,0,0,0,39,57506,1102,1,37363,66,1101,0,1,67,1101,2039,0,68,1101,556,0,69,1101,7,0,71,1101,2041,0,72,1106,0,73,1,1,18,48371,49,334795,31,55355,11,164558,33,79683,4,34253,1,127492,1102,32083,1,66,1102,1,1,67,1102,1,2082,68,1101,556,0,69,1102,0,1,71,1102,1,2084,72,1106,0,73,1,1110,1101,34253,0,66,1101,0,3,67,1102,1,2111,68,1101,302,0,69,1101,1,0,71,1102,1,2117,72,1106,0,73,0,0,0,0,0,0,1,95619,1102,1,15959,66,1102,1,1,67,1101,2146,0,68,1102,556,1,69,1101,1,0,71,1101,2148,0,72,1105,1,73,1,5903,18,96742,1101,11399,0,66,1101,1,0,67,1102,2177,1,68,1101,556,0,69,1101,0,1,71,1102,2179,1,72,1106,0,73,1,18,18,145113,1102,1,70667,66,1101,3,0,67,1102,1,2208,68,1102,302,1,69,1102,1,1,71,1101,0,2214,72,1105,1,73,0,0,0,0,0,0,21,66831,1101,0,26261,66,1101,0,1,67,1102,1,2243,68,1102,556,1,69,1102,1,1,71,1101,0,2245,72,1105,1,73,1,71126137,36,2122";


    }
}