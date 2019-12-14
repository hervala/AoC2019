using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019
{
    public class Day13
    {
        public static int DrawPlayarea(IEnumerable<long> instructions)
        {
            var blockTileCount = 0;
            var inputBuffer = new BufferBlock<long>();
            var outputBuffer = new BufferBlock<long>();
            var batchBlock = new BatchBlock<long>(3);
            outputBuffer.LinkTo(batchBlock, new DataflowLinkOptions { PropagateCompletion = true });
            var vm = new IntCodeVm(instructions, inputBuffer, outputBuffer);
            var runningProgram = Task.Run(() => vm.RunProgram());

            var playareaDrawer = Task.Run(async () =>
            {
                var endOperation = false;
                var outputCounter = -1;
                var xPos = 0;
                var yPos = 0;
                var tileMarkers = " W#_O";
                
                while (!endOperation)
                {
                    try
                    {
                        outputCounter += 1;
                        var outputValue = await batchBlock.ReceiveAsync();
                        xPos = (int)outputValue[0];
                        yPos = (int)outputValue[1];
                        Console.SetCursorPosition(xPos, yPos + 1);   // +1 leave 1. row free
                        var tileId = (int)outputValue[2];
                        if (tileId == 2)
                        {
                            blockTileCount += 1;
                        }
                        Console.Write(tileMarkers[tileId].ToString());
                    }
                    catch (InvalidOperationException ex) when (ex.Source == "System.Threading.Tasks.Dataflow")
                    {
                        endOperation = true;
                    }

                }
            });

            runningProgram.Wait();
            outputBuffer.Complete();
            playareaDrawer.Wait();

            return blockTileCount;  // should be 335 
        }

        public static long AutoPlayBreakout(IEnumerable<long> instructions)
        {
            var score = 0L;
            var inputBuffer = new BufferBlock<long>();
            var outputBuffer = new BufferBlock<long>();
            var batchBlock = new BatchBlock<long>(3);
            outputBuffer.LinkTo(batchBlock, new DataflowLinkOptions { PropagateCompletion = true });
            var vm = new IntCodeVm(instructions, inputBuffer, outputBuffer);
            var runningProgram = Task.Run(() => vm.RunProgram());

            //0 is an empty tile.No game object appears in this tile.
            //1 is a wall tile.Walls are indestructible barriers.
            //2 is a block tile.Blocks can be broken by the ball.
            //3 is a horizontal paddle tile. The paddle is indestructible.
            //4 is a ball tile.The ball moves diagonally and bounces off objects.

            var playareaDrawer = Task.Run(async () =>
            {
                var endOperation = false;
                var outputCounter = -1;
                var xPos = 0;
                var yPos = 0;
                var tileMarkers = " W#_O";
                var ballXPos = -1;
                var paddleXPos = -1;
                while (!endOperation)
                {
                    try
                    {
                        outputCounter += 1;
                        var outputValue = await batchBlock.ReceiveAsync();
                        xPos = (int)outputValue[0];
                        yPos = (int)outputValue[1];
                        if (xPos >= 0)
                        {
                            // draw play area
                            var tileId = (int)outputValue[2];
                            if (tileId == 4)
                            {
                                ballXPos = xPos;
                                //Task.Delay(1).Wait();
                                // move joystick only after paddle has been drawn
                                var joystickInput = ballXPos < paddleXPos ? -1 : ballXPos > paddleXPos ? 1 : 0; // -1 left, 0 neutral, 1 right
                                paddleXPos += joystickInput;
                                inputBuffer.Post(joystickInput);
                            }
                            else if (tileId == 3)
                            {
                                paddleXPos = xPos;
                            }
                            //if ((tileId == 4 || tileId == 3) && (ballXPos > -1 && paddleXPos > -1))
                            //{
                                
                            //}

                            Console.SetCursorPosition(xPos, yPos + 1);   // +1 leave 1. row free
                            Console.Write(tileMarkers[tileId].ToString());
                        }
                        else
                        {
                            // draw score count
                            Console.SetCursorPosition(0, 0);   // +1 leave 1. row free
                            score = outputValue[2];
                            Console.Write($"Score: {score}");
                        }
                    }
                    catch (InvalidOperationException ex) when (ex.Source == "System.Threading.Tasks.Dataflow")  // Dataflow has completed and outputBuffer is empty (Program has ended it's not producing any more output)
                    {
                        endOperation = true;
                    }

                }
            });

            runningProgram.Wait();
            outputBuffer.Complete();
            playareaDrawer.Wait();

            return score;  // should be 335 
        }

    }

    public class IntCodeVm
    {
        private readonly string identifier;
        private readonly BufferBlock<long> input;
        private readonly BufferBlock<long> output;
        private long[] program;
        private int instructionPointer = 0;
        private int relativeBase;

        public IntCodeVm(IEnumerable<long> instructions, BufferBlock<long> input, BufferBlock<long> output) : this("id001", instructions, input, output)
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
                            program[pointer] = await input.ReceiveAsync(TimeSpan.FromSeconds(500), ctoken);
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
