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

        public static int Vaporize200Asteroids(string[] asteroidMap)
        {
            if (asteroidMap is null || asteroidMap.Length == 0)
            {
                throw new ArgumentException("map null or empty", nameof(asteroidMap));
            }

            var asteroids = new Dictionary<Point, Asteroid>();
            var mapXMax = asteroidMap[0].Length - 1;
            var mapYMax = asteroidMap.Length - 1;
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
                                    var angle = (int)(((Math.Atan2((double)y2 - y1, (double)x2 - x1) * (180 / Math.PI) + 360) % 360) * 100); // multiplied by 100 so the precision is greater and I can still use array with int index
                                    asteroid.AsteroidInView[angle] = true;  
                                    if (!(asteroid.AsteroidsInView.TryGetValue(angle, out var asteroidsInLine)))
                                    {
                                        asteroidsInLine = new List<Asteroid>();
                                        asteroid.AsteroidsInView.Add(angle, asteroidsInLine);
                                    }
                                    asteroidsInLine.Add(new Asteroid(x2, y2, x1, y1));
                                    
                                }
                            }
                        }
                    }
                }
            }

            var maxAsteroidsInView = int.MinValue;
            Asteroid bestAsteroid = null;
            foreach (var asteroid in asteroids)
            {
                var asteroidsSeen = asteroid.Value.AsteroidInView.Count(value => value);
                if (asteroidsSeen > maxAsteroidsInView)
                {
                    maxAsteroidsInView = asteroidsSeen;
                    bestAsteroid = asteroid.Value;
                }
            }

            Asteroid target = null;
            if (bestAsteroid != null)
            {
                var asteroidsVaporized = 0;
                var angle = 36000 - (90 * 100);  // zero is at X acsel. Se rewind 90 degrees (times 100 because the index)
                while (asteroidsVaporized < 200)
                {
                    if (bestAsteroid.AsteroidsInView.TryGetValue(angle, out var asteroidsInSight))
                    {
                        var minDistance = asteroidsInSight.Min(a => a.Vaporized ? double.MaxValue : a.Distance);
                        target = asteroidsInSight.Find(a => a.Distance == minDistance);
                        target.Vaporize();
                        asteroidsVaporized += 1;
                    }
                    angle += 1;
                    if (angle % 36000 == 0)
                    {
                        angle = 0;
                    }
                }
            }
            

            return target == null ? throw new Exception("something went wrong") : target.X * 100 + target.Y;
        }



        public class Asteroid : Point
        {
            public Asteroid(int x, int y) : base(x,y)
            {

            }

            public Asteroid(int x, int y, int x0, int y0) : base(x, y)
            {
                Distance = Math.Sqrt(Math.Pow(Math.Abs(X - x0), 2) + Math.Pow(Math.Abs(Y - y0), 2));
            }

            public double Distance { get; }

            public bool[] AsteroidInView { get; set; } = new bool[36000];

            public Dictionary<int, List<Asteroid>> AsteroidsInView { get; set; } = new Dictionary<int, List<Asteroid>>();  // key is degrees*100

            public Point Location => new Point(X, Y);

            public bool Vaporized { get; private set; }

            public void Vaporize()
            {
                Vaporized = true;
            }
        }

        public class AsteroidV2 : Point
        {
            public AsteroidV2(int x, int y) : base(x, y)
            {

            }

            public Dictionary<int, List<AsteroidV2>> AsteroidInView { get; set; } = new Dictionary<int, List<AsteroidV2>>();  // key is degrees*100

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
