using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Day09
{
    public class IntCodeComputerStatefull
    {
        private readonly string identifier;
        private readonly int[] program;
        private int instructionPointer = 0;
        private int[] inputs;
        private int output;
        private int relativeBase;

        public IntCodeComputerStatefull(string identifier, IEnumerable<int> instructions, IEnumerable<int> initialInput)
        {
            if (instructions is null)
            {
                throw new ArgumentNullException(nameof(instructions));
            }

            if (!(instructions.Count() > 0))
            {
                throw new ArgumentException("empty input array", nameof(instructions));
            }

            this.identifier = identifier;
            this.program = instructions.ToArray();
            this.inputs = initialInput.ToArray();
        }

        public (int output, ProcessState state) RunProgram() => RunProgram(new int[] { });

        public (int output, ProcessState state) RunProgram(IEnumerable<int> newInputs)
        {
            if (newInputs.Any())
            {
                inputs = inputs.Concat(newInputs).ToArray();
            }

            int firstParameterValue() => program[instructionPointer] / 100 % 10 == 0 ? program[program[instructionPointer + 1]] : program[instructionPointer] / 100 % 10 == 1 ? program[instructionPointer + 1] : program[relativeBase + program[instructionPointer + 1]]; 
            int secondParameterValue() => program[instructionPointer] / 1000 % 10 == 0 ? program[program[instructionPointer + 2]] : program[instructionPointer] / 1000 % 10 == 1 ? program[instructionPointer + 2] : program[relativeBase + program[instructionPointer + 2]];
            int firstParameterPointer() => program[instructionPointer + 1];
            int thirdParameterPointer() => program[instructionPointer + 3];

            while (instructionPointer < program.Length && program[instructionPointer] != 99)
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
                switch (program[instructionPointer] % 100)
                {
                    case 1:
                        // sum
                        program[thirdParameterPointer()] = firstParameterValue() + secondParameterValue();
                        instructionPointer += 4;
                        break;
                    case 2:
                        // multiply
                        program[thirdParameterPointer()] = firstParameterValue() * secondParameterValue();
                        instructionPointer += 4;
                        break;
                    case 3:
                        // store input
                        try
                        {
                            var input = inputs[0];
                            inputs = inputs[1..];
                            program[firstParameterPointer()] = input;
                        }
                        catch (Exception)
                        {
                            throw new Exception($"{identifier}: Program needs input but none left.");
                        }

                        instructionPointer += 2;
                        break;
                    case 4:
                        // get output
                        output = firstParameterValue();
                        instructionPointer += 2;
                        return (output, ProcessState.Paused);  // return output and pause execution
                    case 5:
                        // jump-if-true
                        instructionPointer = firstParameterValue() != 0 ? secondParameterValue() : instructionPointer + 3;
                        break;
                    case 6:
                        // jump-if-false
                        instructionPointer = firstParameterValue() == 0 ? secondParameterValue() : instructionPointer + 3;
                        break;
                    case 7:
                        // less than
                        program[thirdParameterPointer()] = firstParameterValue() < secondParameterValue() ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    case 8:
                        // equals
                        program[thirdParameterPointer()] = firstParameterValue() == secondParameterValue() ? 1 : 0;
                        instructionPointer += 4;
                        break;
                    case 9:
                        // set relative base
                        relativeBase = firstParameterValue();
                        instructionPointer += 2;
                        break;
                    default:
                        throw new Exception($"invalid opcode {program[instructionPointer]}");
                }
            }

            return (output, ProcessState.Ended);
        }

        public enum ProcessState
        {
            Paused,
            Ended,
        }

    }
}
