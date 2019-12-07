using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace AdventOfCode2019.Day07
{
    public class IntCodeComputer
    {
        private readonly string identifier;

        public IntCodeComputer(string identifier)
        {
            this.identifier = identifier;
        }
        public int ProcessIntcode(IEnumerable<int> instructions, int[] inputs)
        {
            int output = 0;
            if (!(instructions.Count() > 0))
            {
                throw new ArgumentException("empty input array", nameof(instructions));
            }
            var program = instructions.ToArray();
            var instructionPointer = 0;
            var remainingInputs = inputs.ToArray();
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
                        try
                        {
                            var input = remainingInputs[0];
                            remainingInputs = remainingInputs[1..];
                            program[program[instructionPointer + 1]] = input;
                        }
                        catch (Exception)
                        {
                            throw new Exception($"{identifier}: Program needs input but none left.");
                        }

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
            return output;
        }
    }

    public enum ProcessState
    {
        Paused,
        Ended,
    }

    public class IntCodeComputerStatefull
    {
        private readonly string identifier;
        private readonly int[] program;
        private int instructionPointer = 0;
        private int[] inputs;
        private int output;

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
                        try
                        {
                            var input = inputs[0];
                            inputs = inputs[1..];
                            program[program[instructionPointer + 1]] = input;
                        }
                        catch (Exception)
                        {
                            throw new Exception($"{identifier}: Program needs input but none left.");
                        }

                        instructionPointer += 2;
                        break;
                    case 4:
                        // get output
                        output = program[program[instructionPointer] / 100 % 10 == 0 ? program[instructionPointer + 1] : instructionPointer + 1];
                        instructionPointer += 2;
                        return (output, ProcessState.Paused);  // return output and pause execution
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

            return (output, ProcessState.Ended);
        }
    }

}
