using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Day09
{
    public class IntCodeComputerStatefull
    {
        private readonly string identifier;
        private long[] program;
        private int instructionPointer = 0;
        private long[] inputs;
        private long output;
        private int relativeBase;

        public IntCodeComputerStatefull(IEnumerable<long> instructions) : this(instructions, new long[] { })
        {
        }

        public IntCodeComputerStatefull(IEnumerable<long> instructions, IEnumerable<long> initialInput) : this("id001", instructions, initialInput)
        {
        }

        public IntCodeComputerStatefull(string identifier, IEnumerable<long> instructions, IEnumerable<long> initialInput)
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

        public (long output, ProcessState state) RunProgram() => RunProgram(new long[] { });

        public (long output, ProcessState state) RunProgram(IEnumerable<long> newInputs)
        {
            if (newInputs.Any())
            {
                inputs = inputs.Concat(newInputs).ToArray();
            }

            var value = 0L;
            var pointer = 0;

            while (instructionPointer < program.Length && program[instructionPointer] != 99)
            {
                /*
                ABCDE
                 1202

                DE - two - digit opcode,    02 == opcode 2
                 C - mode of 1st parameter,  2 == relative mode
                 B - mode of 2nd parameter,  1 == immediate mode
                 A - mode of 3rd parameter,  0 == position mode,
                                                  omitted due to being a leading zero
                */
                switch (program[instructionPointer] % 100)
                {
                    case 1:
                        // sum
                        value = ParameterValue(Param.First) + ParameterValue(Param.Second);
                        pointer = ParameterPointer(Param.Third);
                        program[pointer] = value;
                        instructionPointer += 4;
                        break;
                    case 2:
                        // multiply
                        value = ParameterValue(Param.First) * ParameterValue(Param.Second);
                        pointer = ParameterPointer(Param.Third);
                        program[pointer] = value;
                        instructionPointer += 4;
                        break;
                    case 3:
                        // store input
                        try
                        {
                            var input = inputs[0];
                            inputs = inputs[1..];
                            pointer = ParameterPointer(Param.First);
                            program[pointer] = input;
                        }
                        catch (Exception)
                        {
                            throw new Exception($"{identifier}: Program needs input but none left.");
                        }

                        instructionPointer += 2;
                        break;
                    case 4:
                        // get output
                        output = ParameterValue(Param.First);
                        instructionPointer += 2;
                        return (output, ProcessState.Paused);  // return output and pause execution
                    case 5:
                        // jump-if-true
                        instructionPointer = ParameterValue(Param.First) != 0 ? (int)ParameterValue(Param.Second) : instructionPointer + 3;
                        break;
                    case 6:
                        // jump-if-false
                        instructionPointer = ParameterValue(Param.First) == 0 ? (int)ParameterValue(Param.Second) : instructionPointer + 3;
                        break;
                    case 7:
                        // less than
                        value = ParameterValue(Param.First) < ParameterValue(Param.Second) ? 1 : 0;
                        pointer = ParameterPointer(Param.Third);
                        program[pointer] = value;
                        instructionPointer += 4;
                        break;
                    case 8:
                        // equals
                        value = ParameterValue(Param.First) == ParameterValue(Param.Second) ? 1 : 0;
                        pointer = ParameterPointer(Param.Third);
                        program[pointer] = value;
                        instructionPointer += 4;
                        break;
                    case 9:
                        // set relative base
                        relativeBase = relativeBase + (int)ParameterValue(Param.First);
                        instructionPointer += 2;
                        break;
                    default:
                        throw new Exception($"invalid opcode {program[instructionPointer]}");
                }
            }

            return (output, ProcessState.Ended);
        }

        private int ParameterPointer(Param param)
        {
            var mode = GetParamMode(param);
            var paramPointer = mode switch
            {
                Mode.Position => (int)program[instructionPointer + (int)param],
                Mode.Relative => relativeBase + (int)program[instructionPointer + (int)param],
                _ => throw new Exception("unknown parameter mode."),
            };
            ResizeProgramArray(paramPointer);
            return paramPointer;
        }

        private long ParameterValue(Param param)
        {
            var mode = GetParamMode(param);
            var paramPointer= mode switch
            {
                Mode.Immediate => instructionPointer + (int)param,
                Mode.Position => (int)program[instructionPointer + (int)param],
                Mode.Relative => relativeBase + (int)program[instructionPointer + (int)param],
                _ => throw new Exception("unknown parameter mode."),
            };
            ResizeProgramArray(paramPointer);

            return program[paramPointer];
        }

        private void ResizeProgramArray(int paramPointer)
        {
            if (paramPointer >= program.Length)
            {
                Array.Resize(ref program, paramPointer + 1);
            }
        }

        private Mode GetParamMode(Param param)
        {
            return (Mode)(program[instructionPointer] / (int)Math.Pow(10,(int)param+1) % 10);
        }

        public enum ProcessState
        {
            Paused,
            Ended,
        }

        public enum Param
        {
            First = 1,
            Second,
            Third,
        }

        public enum Mode
        {
            Position,
            Immediate,
            Relative,
        }
    }
}
