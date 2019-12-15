using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AdventOfCode2019
{
    public class Day14
    {
        private Dictionary<string,long> surplusMaterials = new Dictionary<string, long>();

        public long CalculateNeededOre(string input, (string material, long amount) neededOutput)
        {
            var reactions = new Dictionary<string, Reaction>();
            var reactionFormulas = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var reactionFormula in reactionFormulas)
            {
                if (!string.IsNullOrWhiteSpace(reactionFormula))
                {
                    var chemicals = reactionFormula.Split("=>").Select(s => s.Trim());

                    var outputMaterial = chemicals.Skip(1).Take(1).Select(o =>
                    {
                        var outputArr = o.Split(" ");
                        return (material: outputArr[1], amount: long.Parse(outputArr[0]));
                    }).First();

                    var inputMaterials = chemicals.First().Split(",").Select(s => s.Trim()).Select(o =>
                    {
                        var outputArr = o.Split(" ");
                        return (material: outputArr[1], amount: long.Parse(outputArr[0]));
                    });

                    var reaction = new Reaction(outputMaterial, inputMaterials);
                    reactions.Add(reaction.Output.material, reaction);
                }
            }

            var NeededMaterials = new List<(string material, long amount)>();
            NeededMaterials.Add(neededOutput);

            var loopCount = 0;
            while (!(NeededMaterials.Count == 1 && NeededMaterials.Where(n => n.material == "ORE").Any()))
            {
                var needed = NeededMaterials.Where(n => n.material != "ORE").Take(1).First();
                loopCount += 1;
                // Find formula for needed material
                if (!reactions.TryGetValue(needed.material, out var reaction))
                {
                    throw new Exception($"No formula for needed material {needed.material}");
                }

                // Go throu what reaction needs to produce the needed material
                foreach (var reactionInputMaterial in reaction.Input)
                { 
                    var neededInput = (material: reactionInputMaterial.Key, amount: (long)(Math.Ceiling((double)needed.amount / reaction.Output.amount) * reactionInputMaterial.Value));

                    // first check if reaction input material can be found in surplus
                    if (surplusMaterials.TryGetValue(neededInput.material, out var surplusAmount))
                    {
                        var leftoverSurplus = surplusAmount;
                        if (neededInput.amount >= leftoverSurplus)
                        {
                            neededInput.amount -= leftoverSurplus;
                            leftoverSurplus = 0;
                        }
                        else
                        {
                            leftoverSurplus -= neededInput.amount;
                            neededInput.amount = 0;
                        }

                        // and update what is left of surplus
                        if (leftoverSurplus == 0)
                        {
                            surplusMaterials.Remove(neededInput.material);
                        }
                        else
                        {
                            surplusMaterials[neededInput.material] = leftoverSurplus;
                        }
                    }

                    // if surplus wasn't enough. Add to needed materials
                    if (neededInput.amount > 0)
                    {
                        var index = NeededMaterials.FindIndex(0, (n) => n.material == neededInput.material);
                        if (index >= 0)
                        {
                            NeededMaterials[index] = (neededInput.material, neededInput.amount + NeededMaterials[index].amount);
                        }
                        else
                        {
                            NeededMaterials.Add((neededInput.material, neededInput.amount));
                        }
                    }

                    
                }

                // calculate added surplus (if any)
                var produced = (long)(Math.Ceiling((double)needed.amount / reaction.Output.amount) * reaction.Output.amount);
                if (produced > needed.amount)
                {
                    if (surplusMaterials.ContainsKey(reaction.Output.material))
                    {
                        surplusMaterials[reaction.Output.material] += (produced - needed.amount);
                    }
                    else
                    {
                        surplusMaterials.Add(reaction.Output.material, produced - needed.amount);
                    }
                }

                // remove the produced needed material from the NeededMaterials
                var indexNeeded = NeededMaterials.FindIndex(0, (n) => n.material == needed.material);
                if (indexNeeded >= 0)
                {
                    NeededMaterials.RemoveAt(indexNeeded);
                }
                else
                {
                    throw new Exception("something went wrong");
                }

            }

            return NeededMaterials[0].amount;  // only one left and it is ORE
        }

        public class Reaction
        {
            public (string material, long amount) Output { get; set; }

            public Dictionary<string, long> Input { get; set; }

            public Reaction((string material, long amount) output, IEnumerable<(string material, long amount)> input)
            {
                Output = output;
                Input = input.ToDictionary(i => i.material, i => i.amount);
            }
        }

    }
}
