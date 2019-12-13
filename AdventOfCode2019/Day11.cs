using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2019.Day11
{
    public class Painter
    {
        private Point location;
        private int directionAngle;
        private TileColor colorUnderCamera;
        private readonly Dictionary<Point, TileColor> painting = new Dictionary<Point, TileColor>();

        public Painter() : this(TileColor.Black)
        {
        }

        public Painter(TileColor startingColor)
        {
            location = new Point(0, 0);
            directionAngle = 360 - 90; // up
            colorUnderCamera = startingColor;
            painting.Add(location, colorUnderCamera);
        }

        public void Paint(IEnumerable<long> instruction)
        {
            var inputBuffer = new BufferBlock<long>();
            var outputBuffer = new BufferBlock<long>();

            inputBuffer.Post((long)colorUnderCamera);  // first is black
            var vm = new IntCodeVm(instruction, inputBuffer, outputBuffer);

            var tokenSource = new CancellationTokenSource();
            var ctoken = tokenSource.Token;

            var runningProgram = Task.Run(() => vm.RunProgram(ctoken), ctoken);

            var paintJob = Task.Run(async () =>
            {
                var endOperation = false;
                while (!endOperation)
                {
                    try
                    {
                        var outputColor = await outputBuffer.ReceiveAsync(ctoken);
                        // paint
                        painting[location] = (TileColor)outputColor;

                        var outputTurn = await outputBuffer.ReceiveAsync(ctoken);
                        // turn an move
                        directionAngle = (directionAngle + (90 * (outputTurn == 0 ? -1 : 1) + 360)) % 360; // (directionAngle + (90 * outputTurn == 0 ? -1 : 1) + 360) % 360;
                        location = directionAngle switch
                        {
                            0 => location.MoveRight(),
                            90 => location.MoveDown(),
                            180 => location.MoveLeft(),
                            270 => location.MoveUp(),
                            _ => throw new Exception("Unknown direction angle"),
                        };
                        if (!painting.TryGetValue(location, out colorUnderCamera))
                        {
                            colorUnderCamera = TileColor.Black;
                            painting.Add(location, colorUnderCamera);
                        }
                        inputBuffer.Post((long)colorUnderCamera);
                    }
                    catch (OperationCanceledException)
                    {
                        endOperation = true;
                    }
                    
                }
            }, ctoken);

            runningProgram.Wait();

            try
            {
                tokenSource.Cancel();
                paintJob.Wait();
            }
            catch (OperationCanceledException)
            {
            }

        }

        public int TilesPlainted => painting.Count();

        public enum TileColor
        {
            Black,
            White,
        }

        public Bitmap GetImage()
        {
            // find upper left corner
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            foreach (var tile in painting.Keys)
            {
                if (minY > tile.Y)
                {
                    minY = tile.Y;
                }
                
                if (maxY < tile.Y)
                {
                    maxY = tile.Y;
                }

                if (minX > tile.X)
                {
                    minX = tile.X;
                }

                if (maxX < tile.X)
                {
                    maxX = tile.X;
                }
            }

            if (minY == 0) minY = 1;
            if (minX == 0) minX = 1;
            if (maxY == 0) maxY = 1;
            if (maxX == 0) maxX = 1;

            var bitmap = new Bitmap(Math.Abs(maxX) + Math.Abs(minX) + 1, Math.Abs(minY) + Math.Abs(maxY) + 1);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);
            foreach (var tile in painting)
            {
                bitmap.SetPixel(tile.Key.X + Math.Abs(minX), tile.Key.Y + Math.Abs(minY), color: tile.Value == TileColor.Black ? Color.Black : Color.White);
            }
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }
    }

    [DebuggerDisplay("X = {X}, Y = {Y}")]
    public class Point
    {
        public Point() : this(0, 0)
        {
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !(obj is Point objPoint))
            {
                return false;
            }
            else
            {
                return (X == objPoint.X) && (Y == objPoint.Y);
            }
        }

        public override int GetHashCode()
        {
            return (X << 2) ^ Y;
        }

        public Point MoveLeft() => new Point(this.X - 1, this.Y);

        public Point MoveRight() => new Point(this.X + 1, this.Y);

        public Point MoveUp() => new Point(this.X, this.Y + 1);

        public Point MoveDown() => new Point(this.X, this.Y - 1);

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
