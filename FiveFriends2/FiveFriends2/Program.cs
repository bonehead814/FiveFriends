using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveFriends2
{
    class Program
    {
        // Tree node class containing one player's selection.  Trees will be built of all selections which have the same product.
        // Child nodes of a parent are represent selections that are disjoint from the selections represented by parent and all
        // other nodes on the path to the tree root.  If any branch of the tree reaches a depth of 5, then we know that there
        // are five disjoint selections that have the same product.
        private class SelectionNode
        {
            public int SelectionIndex { get; set; }
            public List<int> PathIndices { get; set; }
            public List<SelectionNode> DisjointChildren { get; set; } = new List<SelectionNode>();
        }

        private static List<int> primes = new List<int>();
        private static List<int> nonPrimes = new List<int>();

        // List of all nonPrimes with at least two distinct prime factors
        private static List<int> candidates = new List<int>();

        // List of all possible selections.  Each selection is an array of 5 distinct candidates.
        private static List<int[]> allSelections = new List<int[]>();


        private static bool IsPrime(int n)
        {
            int numDivisors = 0;
            for (int i = 1; i <= n; i++)
                if ((n % i) == 0)
                    numDivisors++;
            return (numDivisors == 2) ? true : false;
        }

        private static bool HasAtLeast2PrimeFactors(int n)
        {
            int numDivisors = 0;
            for (int i = 0; i < primes.Count; i++)
                if ((n % primes[i]) == 0)
                    numDivisors++;
            return (numDivisors >= 2) ? true : false;
        }

        private static bool IsDisjoint(SelectionNode parent, int childSelectionIndex, List<int> selections)
        {
            for (int i = 0; i < parent.PathIndices.Count; i++)
            {
                int pathIndex = parent.PathIndices[i];
                foreach (int index in allSelections[selections[pathIndex]])
                    foreach (int testIndex in allSelections[selections[childSelectionIndex]])
                        if (index == testIndex)
                            return false;
            }
            return true;
        }

        private static int AddDisjointChildren(SelectionNode parent, List<int> selections)
        {
            int count = 0;
            if (parent.PathIndices.Count > 5)
            {
                Console.WriteLine("Unexpected level greater than 5");
                return 0;
            }
            else if (parent.PathIndices.Count == 5)
                count = 1;
            for (int i=parent.PathIndices[parent.PathIndices.Count-1]+1; i<selections.Count; i++)
            {
                if (IsDisjoint(parent, i, selections))
                {
                    SelectionNode child = new SelectionNode { SelectionIndex = i, PathIndices = new List<int>() };
                    foreach (int pathIndex in parent.PathIndices)
                        child.PathIndices.Add(pathIndex);
                    child.PathIndices.Add(i);
                    count += AddDisjointChildren(child, selections);
                    parent.DisjointChildren.Add(child);
                }
            }
            return count;
        }

        static void Main(string[] args)
        {
            // Separate primes
            for (int i = 1; i <= 70; i++)
                if (IsPrime(i))
                    primes.Add(i);
                else
                    nonPrimes.Add(i);

            // Find candidates (non-primes with at least 2 distinct prime factors)
            for (int i = 0; i < nonPrimes.Count; i++)
                if (HasAtLeast2PrimeFactors(nonPrimes[i]))
                    candidates.Add(nonPrimes[i]);

            // Find the product for all valid selections (combinations of 5 candidates). Store the selections
            // indexed by their product in the uniqueProducts dictionary.
            Dictionary<int, List<int>> uniqueProducts = new Dictionary<int, List<int>>();
            for (int i = 0; i < candidates.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    for (int k = 0; k < j; k++)
                    {
                        for (int l = 0; l < k; l++)
                        {
                            for (int m = 0; m < l; m++)
                            {
                                int product = candidates[i] * candidates[j] * candidates[k] * candidates[l] * candidates[m];
                                int[] indexArray = new int[] { i, j, k, l, m };
                                if (uniqueProducts.ContainsKey(product))
                                    uniqueProducts[product].Add(allSelections.Count);
                                else
                                    uniqueProducts.Add(product, new List<int> { allSelections.Count }  );
                                allSelections.Add(indexArray);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Unique products count = " + uniqueProducts.Count);

            // Build selection trees for for all the selections with the same product.  The recursive fuction AddDisjointChildren
            // will return a count of all branches that reached a depth of 5.
            Dictionary<int, List<SelectionNode>> selectionTrees = new Dictionary<int, List<SelectionNode>>();
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int product in uniqueProducts.Keys)
            {
                int count = 0;
                List<int> selections = uniqueProducts[product];
                if (selections.Count >= 5)
                {
                    List<SelectionNode> nodeList = new List<SelectionNode>();
                    for (int selectionIndex=0; selectionIndex<selections.Count; selectionIndex++)
                    {
                        SelectionNode node = new SelectionNode { SelectionIndex = selectionIndex, PathIndices = new List<int> { selectionIndex } };
                        count += AddDisjointChildren(node, selections);
                        nodeList.Add(node);
                    }
                    // Only record a selection tree if at least one branch reached a depth of 5.
                    if (count > 0)
                    {
                        selectionTrees.Add(product, nodeList);
                        counts.Add(product, count);
                    }
                }
            }
            Console.WriteLine("Number of qualified products = " + selectionTrees.Count);
            foreach (int product in selectionTrees.Keys)
                Console.WriteLine("Product: " + product + " has " + counts[product] + " ways to get there");
        }
    }
}
