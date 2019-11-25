using System;
using System.Collections.Generic;

namespace FiveFriends2
{
    class Program
    {

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

        private static bool IsDisjoint(List<int> parentPathIndices, int childSelectionIndex, List<int> selections)
        {
            for (int i = 0; i < parentPathIndices.Count; i++)
            {
                int pathIndex = parentPathIndices[i];
                foreach (int index in allSelections[selections[pathIndex]])
                    foreach (int testIndex in allSelections[selections[childSelectionIndex]])
                        if (index == testIndex)
                            return false;
            }
            return true;
        }

        private static int BuildDisjointSelections(List<int> pathIndices, List<int> selections)
        {
            int count = 0;
            if (pathIndices.Count > 5)
            {
                Console.WriteLine("Unexpected level greater than 5");
                return 0;
            }
            else if (pathIndices.Count == 5)
                count = 1;
            for (int i=pathIndices[pathIndices.Count-1]+1; i<selections.Count; i++)
            {
                if (IsDisjoint(pathIndices, i, selections))
                {
                    List<int> childPathIndices = new List<int>();
                    foreach (int pathIndex in pathIndices)
                        childPathIndices.Add(pathIndex);
                    childPathIndices.Add(i);
                    count += BuildDisjointSelections(childPathIndices, selections);
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

            // For each product, build all combinations of disjoint selections from its selection list.  Count
            // the number of paths that reach a length of 5 (for the 5 players of this game).  Record any product
            // where the count is greater than zero.
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int product in uniqueProducts.Keys)
            {
                int count = 0;
                List<int> selections = uniqueProducts[product];
                if (selections.Count >= 5)
                {
                    for (int selectionIndex=0; selectionIndex<selections.Count; selectionIndex++)
                        count += BuildDisjointSelections(new List<int> { selectionIndex }, selections);
                    if (count > 0)
                        counts.Add(product, count);
                }
            }
            Console.WriteLine("Number of qualified products = " + counts.Count);
            foreach (int product in counts.Keys)
                Console.WriteLine("Product: " + product + " has " + counts[product] + " ways to get there");
        }
    }
}
