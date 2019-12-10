using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace AdventOfCode2019
{
    public class Day10
    {
        public static int CalculateMaxAsteroidsInView(string[] asteroidMap)
        {
            if (asteroidMap is null || asteroidMap.Length == 0)
            {
                throw new ArgumentException("map null or empty", nameof(asteroidMap));
            }

            var asteroids = new Dictionary<Point, Asteroid>();
            var mapXMax = asteroidMap[0].Length - 1;
            var mapYMax = asteroidMap.Length -1 ;
            for (int y1 = 0; y1 <= mapYMax; y1++)
            {
                for (int x1 = 0; x1 <= mapXMax; x1++)
                {
                    if (asteroidMap[y1].Substring(x1, 1) == "#")
                    {
                        var asteroid = new Asteroid(x1, y1);
                        asteroids.Add(asteroid.Location, asteroid);
                        for (int y2 = 0; y2 <= mapYMax; y2++)
                        {
                            for (int x2 = 0; x2 <= mapXMax; x2++)
                            {
                                if (asteroidMap[y2].Substring(x2, 1) == "#" && !(x2 == x1 && y2 == y1))
                                {
                                    //Debug.WriteLine($"[x1,y1:{x1},{y1} - x2,y2:{x2},{y2}]: {(int)((Math.Atan2((double)y2 - y1, (double)x2 - x1) * (180 / Math.PI) + 360) % 360)} astetta x-tasosta");
                                    asteroid.AsteroidInView[(int)(((Math.Atan2((double)y2 - y1, (double)x2 - x1) * (180 / Math.PI) + 360) % 360)*100)] = true;  // multiplied by 100 so the precision is greater and I can still use array with int index
                                }
                            }
                        }
                    }
                }
            }

            var maxAsteroidsInView = int.MinValue;
            Point maxCntCoors;
            foreach (var asteroid in asteroids)
            {
                var asteroidsSeen = asteroid.Value.AsteroidInView.Count(value => value);
                if (asteroidsSeen > maxAsteroidsInView)
                {
                    maxAsteroidsInView = asteroidsSeen;
                    maxCntCoors = asteroid.Value.Location;
                }
            }

            return maxAsteroidsInView;
        }

 

        public class Asteroid : Point
        {
            public Asteroid(int x, int y) : base(x,y)
            {

            }

            public bool[] AsteroidInView { get; set; } = new bool[36000];

            public Point Location => new Point(X, Y);
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

        }


    }

}
