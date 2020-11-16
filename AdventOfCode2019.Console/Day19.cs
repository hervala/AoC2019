using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019
{
    class Day19
    {
        public static void CheckTractorBeam()
        {
            StringBuilder sb = new StringBuilder();
            var count = 0;
            var inputBuffer = new BufferBlock<long>();
            var outputBuffer = new BufferBlock<long>();
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    var vm = new IntCodeVm(programStr, inputBuffer, outputBuffer);

                    var programTask = Task.Run(() => vm.RunProgram());

                    inputBuffer.Post(x);
                    inputBuffer.Post(y);
                    var output = outputBuffer.ReceiveAsync().Result;
                    count += output == 1 ? 1 : 0;
                    sb.Append(output);
                }
                sb.AppendLine();
            }

            byte[] buffer = Encoding.ASCII.GetBytes(sb.ToString());
            Console.SetCursorPosition(0, 0);
            using (var stream = Console.OpenStandardOutput(sb.Length))
            {
                stream.Write(buffer);
            }

        }

        public static void CheckSantaFits()
        {
            var y = 15;
            var x = 0;

            while (true)
            {
                if (!IsWithinBeam(x, y))
                {
                    x++;
                }
                else
                {
                    while(IsWithinBeam(x + 99, y))
                    {
                        if (IsWithinBeam(x, y + 99))
                        {
                            break;
                        }
                        x++;
                    }

                    if (IsWithinBeam(x, y + 99) && IsWithinBeam(x + 99, y))
                    {
                        break;
                    }
                    y++;
                }
            }

            var result = x * 10000 + y; // 9480761

        }

        public static bool IsWithinBeam(int x, int y)
        {
            var inputBuffer = new BufferBlock<long>();
            var outputBuffer = new BufferBlock<long>();
            var vm = new IntCodeVm(programStr, inputBuffer, outputBuffer);
            var programTask = Task.Run(() => vm.RunProgram());
            inputBuffer.Post(x);
            inputBuffer.Post(y);
            return outputBuffer.ReceiveAsync().Result == 1 ? true : false;
        }

        private static string programStr = @"109,424,203,1,21101,0,11,0,1105,1,282,21101,18,0,0,1105,1,259,1202,1,1,221,203,1,21102,1,31,0,1106,0,282,21102,38,1,0,1106,0,259,21002,23,1,2,22102,1,1,3,21102,1,1,1,21102,1,57,0,1105,1,303,2102,1,1,222,20101,0,221,3,21001,221,0,2,21102,259,1,1,21102,1,80,0,1106,0,225,21102,62,1,2,21101,91,0,0,1105,1,303,2101,0,1,223,21001,222,0,4,21101,0,259,3,21101,0,225,2,21101,0,225,1,21101,0,118,0,1105,1,225,20102,1,222,3,21101,94,0,2,21102,133,1,0,1105,1,303,21202,1,-1,1,22001,223,1,1,21101,0,148,0,1105,1,259,1202,1,1,223,20101,0,221,4,21001,222,0,3,21102,17,1,2,1001,132,-2,224,1002,224,2,224,1001,224,3,224,1002,132,-1,132,1,224,132,224,21001,224,1,1,21101,195,0,0,105,1,109,20207,1,223,2,20101,0,23,1,21102,-1,1,3,21101,214,0,0,1106,0,303,22101,1,1,1,204,1,99,0,0,0,0,109,5,2101,0,-4,249,22102,1,-3,1,22101,0,-2,2,21201,-1,0,3,21102,1,250,0,1106,0,225,22101,0,1,-4,109,-5,2105,1,0,109,3,22107,0,-2,-1,21202,-1,2,-1,21201,-1,-1,-1,22202,-1,-2,-2,109,-3,2106,0,0,109,3,21207,-2,0,-1,1206,-1,294,104,0,99,22101,0,-2,-2,109,-3,2106,0,0,109,5,22207,-3,-4,-1,1206,-1,346,22201,-4,-3,-4,21202,-3,-1,-1,22201,-4,-1,2,21202,2,-1,-1,22201,-4,-1,1,21201,-2,0,3,21101,343,0,0,1106,0,303,1105,1,415,22207,-2,-3,-1,1206,-1,387,22201,-3,-2,-3,21202,-2,-1,-1,22201,-3,-1,3,21202,3,-1,-1,22201,-3,-1,2,22102,1,-4,1,21102,384,1,0,1105,1,303,1105,1,415,21202,-4,-1,-4,22201,-4,-3,-4,22202,-3,-2,-2,22202,-2,-4,-4,22202,-3,-2,-3,21202,-4,-1,-2,22201,-3,-2,1,21201,1,0,-4,109,-5,2105,1,0";

    }
}
