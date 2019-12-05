using System; 
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day03
    {
        // this multidimensional array solution is shit. Takes for ever.
        public static int CalculateClosestCrossing(string[][] wires)
        {
            var startPos = (x: 15000, y: 15000);
            var curPos = startPos;
            try
            {
                var wireCnt = wires.Count();
                var panel = new bool[40000, 40000, wireCnt];
                for (int i = 0; i < wireCnt; i++)
                {
                    startPos = (x: 20000, y: 20000);
                    curPos = startPos;
                    panel[curPos.x, curPos.y, i] = true;
                    foreach (var instruction in wires[i])
                    {
                        var movement = int.Parse(instruction.Substring(1));
                        var direction = instruction.Substring(0, 1);
                        for (int j = 1; j <= movement; j++)
                        {
                            switch (direction)
                            {
                                case "L":
                                    panel[curPos.x - j, curPos.y, i] = true;
                                    break;
                                case "R":
                                    panel[curPos.x + j, curPos.y, i] = true;
                                    break;
                                case "U":
                                    panel[curPos.x, curPos.y + j, i] = true;
                                    break;
                                case "D":
                                    panel[curPos.x, curPos.y - j, i] = true;
                                    break;
                                default:
                                    break;
                            }
                        }

                        switch (direction)
                        {
                            case "L":
                                curPos = (x: curPos.x - movement, y: curPos.y);
                                break;
                            case "R":
                                curPos = (x: curPos.x + movement, y: curPos.y);
                                break;
                            case "U":
                                curPos = (x: curPos.x, y: curPos.y + movement);
                                break;
                            case "D":
                                curPos = (x: curPos.x, y: curPos.y - movement);
                                break;
                            default:
                                break;
                        }

                    }
                }

                var minDistance = int.MaxValue;
                for (int x = 0; x < 40000; x++)
                {
                    for (int y = 0; y < 40000; y++)
                    {
                        var bothInSamePosition = true;
                        for (int i = 0; i < wireCnt; i++)
                        {
                            bothInSamePosition &= panel[x, y, i];
                        }
                        if (bothInSamePosition)
                        {
                            var distance = CalculateManhattanDistance(startPos.x, x, startPos.y, y);
                            if (distance < minDistance && distance != 0)
                            {
                                minDistance = distance;
                            }

                        }
                    }
                }

                return minDistance;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int CalculateManhattanDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
    }

    public class Day03Part2
    {
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

        public class Bundle
        {
            public LinkedListNode<Point> Wire1 { get; set; }
            public LinkedListNode<Point> Wire2 { get; set; }

            public LinkedListNode<Point> this[int index]   
            {
                get
                {
                    if (index == 0)
                    {
                        return Wire1;
                    }
                    else
                    {
                        return Wire2;
                    }
                }
                set
                {
                    if (index == 0)
                    {
                        Wire1 = value;
                    }
                    else
                    {
                        Wire2 = value;
                    }
                }
            }
        }

        public static int CalculateNearestCrossing(string[][] wires)
        {

            var wirePath = new[] { new LinkedList<Point>(), new LinkedList<Point>() };

            var bundles = new Dictionary<Point, Bundle>();

            var minLenght = int.MaxValue;

            var startPos = new Point();  // 0,0
            try
            {
                var wireCnt = wires.Count();
                for (int i = 0; i < wireCnt; i++)
                {
                    Point curPos = startPos;
                    wirePath[i].AddFirst(curPos);
                    foreach (var instruction in wires[i])
                    {
                        var movement = int.Parse(instruction.Substring(1));
                        var direction = instruction.Substring(0, 1);
                        for (int j = 1; j <= movement; j++)
                        {
                            Point newPoint = null;
                            switch (direction)
                            {
                                case "L":
                                    newPoint = curPos.MoveLeft();
                                    break;
                                case "R":
                                    newPoint = curPos.MoveRight();
                                    break;
                                case "U":
                                    newPoint = curPos.MoveUp();
                                    break;
                                case "D":
                                    newPoint = curPos.MoveDown();
                                    break;
                                default:
                                    break;
                            }
                            var node = wirePath[i].AddFirst(newPoint);
                            if (bundles.TryGetValue(newPoint, out var wirePoints))
                            {
                                if (i == 1 && wirePoints[0] != null)
                                {
                                    var lenght = CountWireLenghtSum(wirePoints[0], node);
                                    if (lenght < minLenght)
                                    {
                                        minLenght = lenght;
                                    }
                                }
                                if (wirePoints[i] is null)
                                {
                                    wirePoints[i] = node;
                                }
                            }
                            else
                            {
                                var bundle = new Bundle();
                                bundle[i] = node;
                                bundles.Add(newPoint, bundle);
                            }
                            curPos = newPoint;
                        }

                    }
                }

                return minLenght;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static int CountWireLenghtSum(LinkedListNode<Point> wire1, LinkedListNode<Point> wire2)
        {
            LinkedListNode<Point> node = wire1.Next;
            int cnt = 0;
            while (node != null)
            {
                cnt += 1;
                node = node.Next;
            }

            node = wire2.Next;
            while (node != null)
            {
                cnt += 1;
                node = node.Next;
            }
            return cnt;
        }

    }
}
