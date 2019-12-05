using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day05
    {
        public static int[] ProcessIntcode(IEnumerable<int> memory, int input, out int output)
        {
            output = -1;
            if (!(memory.Count() > 0))
            {
                throw new ArgumentException("empty input array", nameof(memory));
            }
            var program = memory.ToArray();
            var instructionPointer = 0;
            while (program[instructionPointer] != 99 && instructionPointer < program.Length)
            {
                /*
                ABCDE
                 1002

                DE - two - digit opcode,      02 == opcode 2
                 C - mode of 1st parameter,  0 == position mode
                 B - mode of 2nd parameter,  1 == immediate mode
                 A - mode of 3rd parameter,  0 == position mode,
                                                  omitted due to being a leading zero
                */
               // int firstParameterValue = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]);
               // int secondParameterValue = (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]);
               // int thirdParameterPointer = program[instructionPointer + 3];
                switch (program[instructionPointer] % 100)  
                {
                    case 1:
                        // sum
                        program[program[instructionPointer + 3]] = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) + (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]);
                        instructionPointer += 4;
                        break;
                    case 2:
                        // multiply
                        program[program[instructionPointer + 3]] = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) * (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]);
                        instructionPointer += 4;
                        break;
                    case 3:
                        // store input
                        program[program[instructionPointer + 1]] = input;
                        instructionPointer += 2; 
                        break;
                    case 4:
                        // get output
                        output = program[program[instructionPointer] / 100 % 10 == 0 ? program[instructionPointer + 1] : instructionPointer + 1];
                        Debug.WriteLine(output);
                        instructionPointer += 2;
                        break;
                    case 5:
                        // jump-if-true
                        instructionPointer = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) != 0 ? (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]) : instructionPointer + 3;
                        break;
                    case 6:
                        // jump-if-false
                        instructionPointer = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) == 0 ? (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]) : instructionPointer + 3;
                        break;
                    case 7:
                        // less than
                        program[program[instructionPointer + 3]] = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) < (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]) ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    case 8:
                        // equals
                        program[program[instructionPointer + 3]] = (program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer + 1]) == (program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer + 2]) ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    default:
                        throw new Exception($"invalid opcode {program[instructionPointer]}");
                }
            }
            return program;
        }
    }

}
