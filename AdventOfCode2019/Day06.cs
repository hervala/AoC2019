using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public static class Day06
    {
        public static int CalculateOrbitCountCheckSum(string inputData, out Tree<string> orbits)
        {
            var orbitTree = new Tree<string>();
            foreach (var row in inputData.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var values = row.Split(')');
                var parentValue = values[0];
                var childValue = values[1];

                if (!orbitTree.TryGetValue(parentValue, out var parent))
                {
                    if(!orbitTree.TryGetValue(childValue, out var child))
                    {
                        child = new Node<string>(childValue);
                    }
                    orbitTree.AddNode(parentValue, null, new[] { child });
                }
                else
                {
                    if (!orbitTree.TryGetValue(childValue, out var child))
                    {
                        child = new Node<string>(childValue);
                    }
                    parent.AddChildren(new[] { child });
                    orbitTree.AddNode(child);
                }
            }

            var levelSum = 0;
            foreach (var item in orbitTree)
            {
                levelSum += item.Value.Level;
            }

            orbits = orbitTree;
            return levelSum;
        }


    }

    public static class Day06_Part2
    {
        public static int OrbitalTransfers(Tree<string> orbits, string from, string to)
        {
            var fromOrbit = orbits[from].Parent;
            var toOrbit = orbits[to].Parent;

            var traveller = fromOrbit;

            var orbitsTravelled = 0;
            while(traveller != null)
            {
                if (traveller.Value == toOrbit.Value)
                {
                    break;
                }

                var orbitsAway = traveller.TryFindFromChilder(toOrbit);
                orbitsTravelled += orbitsAway;
                if (orbitsAway != 0)
                {
                    break;
                }
                traveller = traveller.Parent;
                orbitsTravelled += 1;
            }

            return orbitsTravelled;
        }

        private static int TryFindFromChilder(this Node<string> node, Node<string> searchFor)
        {
            foreach (var child in node.Children)
            {
                if (child.Value == searchFor.Value)
                {
                    return 1;
                }
                var orbs = child.TryFindFromChilder(searchFor);
                if (orbs != 0)
                {
                    return orbs + 1;
                }
            }
            return 0;
        }
    }

    [DebuggerDisplay("value = {Value}")]
    public class Node<T>
    {
        public Node(T value) : this(value, null)
        {
        }

        public Node(T value, Node<T> parent)
        {
            Value = value;
            Parent = parent;
            Children = new List<Node<T>>();
        }

        public T Value { get; set; }

        public Node<T> Parent { get; set; }

        public List<Node<T>> Children { get; }

        public void AddChildren(IEnumerable<Node<T>> children)
        {
            foreach (var item in children)
            {
                item.Parent = this;
                Children.Add(item);
            }
        }

        public int Level
        {
            get
            {
                var level = 0;
                var node = Parent;
                while (node != null)
                {
                    level += 1;
                    node = node.Parent;
                }
                return level;
            }
        }
    }

    public class Tree<T> : Dictionary<T, Node<T>>
    {
        public Node<T> AddNode(Node<T> node)
        {
            TryAdd(node.Value, node);
            return node;
        }

        public Node<T> AddNode(T value, Node<T> parent, IEnumerable<Node<T>> children)
        {
            var node = new Node<T>(value, parent);
            node.AddChildren(children);
            TryAdd(value, node);
            foreach (var item in children)
            { 
                AddNode(item);
            }
            return node;
        }
    }


}
