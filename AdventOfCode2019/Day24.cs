using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2019
{
    public class Day24
    {
        public static void GameOfBug()
        {
            var bugHabitat = new bool[5][];

            var rows = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < rows.Length; i++)
            {
                bugHabitat[i] = new bool[]
                {
                    rows[i].Substring(0, 1) == "#",
                    rows[i].Substring(1, 1) == "#",
                    rows[i].Substring(2, 1) == "#",
                    rows[i].Substring(3, 1) == "#",
                    rows[i].Substring(4, 1) == "#",
                };
            }

            do
            {
                bool[][] newHabitat =
                {
                    new bool[] { false, false, false, false, false },
                    new bool[] { false, false, false, false, false },
                    new bool[] { false, false, false, false, false },
                    new bool[] { false, false, false, false, false },
                    new bool[] { false, false, false, false, false },
                };
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        newHabitat[y][x] = IsItAlive(bugHabitat, x, y);
                    }
                }
            } while (true);

        }

        private static bool IsItAlive(bool[][] bugHabitat, int x, int y)
        {
            var adjacentBugs = 0;

            // up
            if (y-1 >= 0)
            {
                adjacentBugs += bugHabitat[y - 1][x] ? 1 : 0;
            }

            // down 
            if (y+1 < bugHabitat.Length)
            {
                adjacentBugs += bugHabitat[y + 1][x] ? 1 : 0;
            }

            // left
            if (x-1 >= 0)
            {
                adjacentBugs += bugHabitat[y][x - 1] ? 1 : 0;
            }

            // right
            if (x+1 < bugHabitat[0].Length)
            {
                adjacentBugs += bugHabitat[y][x + 1] ? 1 : 0;
            }

            if (bugHabitat[y][x])
            {
                // A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
                return adjacentBugs == 1;
            }
            else
            {
                // An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
                return adjacentBugs == 1 || adjacentBugs == 2;
            }
        }

        private static string input = @"#..##
##...
.#.#.
#####
####.";

    }
}
