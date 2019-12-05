using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day02
    {
        public static int[] ProcessIntcode(IEnumerable<int> memory)
        {
            if (!(memory.Count() > 0))
            {
                throw new ArgumentException("empty input array", nameof(memory));
            }
            var output = memory.ToArray();
            var instructionPointer = 0;
            while (output[instructionPointer] != 99 && instructionPointer < output.Length)
            {
                switch (output[instructionPointer])
                {
                    case 1:
                        output[output[instructionPointer + 3]] = output[output[instructionPointer + 1]] + output[output[instructionPointer + 2]];
                        instructionPointer += 4;
                        break;
                    case 2:
                        output[output[instructionPointer + 3]] = output[output[instructionPointer + 1]] * output[output[instructionPointer + 2]];
                        instructionPointer += 4;
                        break;
                    default:
                        throw new Exception($"invalid opcode {output[instructionPointer]}");
                }
            }
            return output;
        }
    }
}
