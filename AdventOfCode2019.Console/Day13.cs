using System;
using System.Collections.Generic;
using System.Text;
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
}
