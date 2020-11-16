using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019
{
    public class IntCodeVm
    {
        private readonly string identifier;
        private readonly BufferBlock<long> input;
        private readonly BufferBlock<long> output;
        private long[] program;
        private int instructionPointer = 0;
        private int relativeBase;

        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public IntCodeVm(IEnumerable<long> instructions, BufferBlock<long> input, BufferBlock<long> output) : this("id001", instructions, input, output)
        {
        }

        public IntCodeVm(string instructions, BufferBlock<long> input, BufferBlock<long> output) : this("id001", instructions.Split(",").Select(v => long.Parse(v)).ToArray(), input, output)
        {
        }

        public IntCodeVm(string identifier, IEnumerable<long> instructions, BufferBlock<long> input, BufferBlock<long> output)
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
            this.input = input;
            this.output = output;
            this.program = instructions.ToArray();
        }

        public void Poke(int pointer, long value)
        {
            program[pointer] = value;
        }

        public async Task Reset(string programStr)
        {
            this.program = programStr.Split(",").Select(v => long.Parse(v)).ToArray();
            instructionPointer = 0;
        }

        public Task<bool> AwaitingInputAsync()
        {
            tcs = new TaskCompletionSource<bool>();
            return tcs.Task;
        }

        public async Task RunProgram() => await RunProgram(new CancellationTokenSource().Token);

        public async Task RunProgram(CancellationToken ctoken)
        {
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
                            pointer = ParameterPointer(Param.First);
                            if (!input.TryReceive(out var inputValue))
                            {
                                tcs.SetResult(true);
                                inputValue = await input.ReceiveAsync(TimeSpan.FromSeconds(500), ctoken);
                            }
                            program[pointer] = inputValue;
                        }
                        catch (Exception)
                        {
                            throw new Exception($"{identifier}: Program needs input but none left.");
                        }

                        instructionPointer += 2;
                        break;
                    case 4:
                        // get output
                        output.Post(ParameterValue(Param.First));
                        instructionPointer += 2;
                        break;
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
            tcs.SetResult(false);
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
            var paramPointer = mode switch
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
            return (Mode)(program[instructionPointer] / (int)Math.Pow(10, (int)param + 1) % 10);
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
