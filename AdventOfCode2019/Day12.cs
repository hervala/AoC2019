using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AdventOfCode2019
{
    public class Day12
    {
        public static int CalculateMoonSystemsTotalEnergy(string input, int steps)
        {
            var moonSystem = new MoonSystem();
            foreach (var line in input.Split("\n").SkipLast(1))
            {
                moonSystem.Moons.Add(new Moon(line));
            }

            for (int i = 0; i < steps; i++)
            {
                moonSystem.AdvanceTime();
            }

            return moonSystem.TotalEnergy;
        }
    }

    internal class Point3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    internal class Velocity : Point3D
    {

    }

    [DebuggerDisplay("pos=<x={Position.X}, y={Position.Y}, z={Position.Z}>, vel=<x={Velocity.X}, y={Velocity.Y}, z={Velocity.Z}>")]
    internal class Moon
    {
        public Moon(string initPosition)  // <x=17, y=-9, z=4>
        {
            string expression = @"<x=([-\d]*),\sy=([-\d]*),\sz=([-\d]*)>";

            Regex r = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(initPosition);
            if (m.Success)
            {
                Position = new Point3D
                {
                    X = int.Parse(m.Groups[1].ToString()),
                    Y = int.Parse(m.Groups[2].ToString()),
                    Z = int.Parse(m.Groups[3].ToString()),
                };

                Velocity = new Velocity();
            }
            else
            {
                throw new ArgumentException($"Couldn't parse {initPosition}");
            }
        }

        public Point3D Position { get; set; }
        public Velocity Velocity { get; set; }

        public static void AdjustVelocities(Moon moon1, Moon moon2)
        {
            moon1.Velocity.X += moon1.Position.X < moon2.Position.X ? 1 : moon1.Position.X == moon2.Position.X ? 0 : -1;
            moon2.Velocity.X += moon1.Position.X > moon2.Position.X ? 1 : moon1.Position.X == moon2.Position.X ? 0 : -1;
            moon1.Velocity.Y += moon1.Position.Y < moon2.Position.Y ? 1 : moon1.Position.Y == moon2.Position.Y ? 0 : -1;
            moon2.Velocity.Y += moon1.Position.Y > moon2.Position.Y ? 1 : moon1.Position.Y == moon2.Position.Y ? 0 : -1;
            moon1.Velocity.Z += moon1.Position.Z < moon2.Position.Z ? 1 : moon1.Position.Z == moon2.Position.Z ? 0 : -1;
            moon2.Velocity.Z += moon1.Position.Z > moon2.Position.Z ? 1 : moon1.Position.Z == moon2.Position.Z ? 0 : -1;
        }

        public void ApplyGravity()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Position.Z += Velocity.Z;
        }


        public int PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);

        public int KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

        public int TotalEnergy => PotentialEnergy * KineticEnergy;

    }

    internal class MoonSystem
    {
        public List<Moon> Moons { get; set; } = new List<Moon>();

        internal void AdvanceTime()
        {
            ApplyGravity();
            ApplyVelocity();
        }

        private void ApplyVelocity()
        {
            foreach (var moon in Moons)
            {
                moon.ApplyGravity();
            }
        }

        private void ApplyGravity()
        {
            var pairs = Moons.SelectMany((first, i) => Moons.Skip(i + 1).Select(second => (first, second)));
            foreach (var pair in pairs)
            {
                Moon.AdjustVelocities(pair.first, pair.second);
            }
        }

        public int TotalEnergy => Moons.Sum(moon => moon.TotalEnergy);
    }

}
