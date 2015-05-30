using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver.DvrpAlgorithm
{
    internal class DvrpDivider
    {
        public DvrpPartialProblem[] Divide(DvrpProblem dvrpProblem, int numberOfParts)
        {
            // we generate all the partitions
            // set {{0 0 0 0 0}} represents {{0 1 2 3 4}}
            // set {{0 1 2 3 4}} represents {{0}{1}{2}{3}{4}}
            // set {{0 1 2 1 2}} represents {{0}{1 3}{2 4}}
            // part - an actual partition 
            // max[i] = Max{max[i-1], part[i-1]}
            // max[0] = -1
            // example:
            // 0 0 0
            // 0 0 1
            // 0 1 0
            // 0 1 1
            // 0 1 2

            if (numberOfParts < 1)
                throw new ArgumentOutOfRangeException("numberOfParts");
            var n = dvrpProblem.Requests.Length;
            var part = new int[n];

            var max = new int[n];
            var maxNumberofSets = TriangularMethodBellNumber(n);
            var numberOfSetsForThread = maxNumberofSets/(ulong) numberOfParts;
            var result = new DvrpPartialProblem[numberOfParts];
            var solver = new DvrpSolver(dvrpProblem);
            var approximateResult = solver.SolveApproximately();

            for (var i = n - 2; i > -1; --i)
                part[i] = 0;
            part[n - 1] = max[0] = -1;
            var partLast = (int[]) part.Clone();
            ulong j = 0;
            var k = 0;
            do
            {
                // checking if max[i] == Max{max[i-1], part[i-1]}
                for (var i = 1; i < n; ++i)
                {
                    if (max[i - 1] < part[i - 1])
                        max[i] = part[i - 1];
                    else
                        max[i] = max[i - 1];
                }
                // we check if the last elements of the set are in their actual maximum
                // for example
                // 01200345 p = 7
                // 01200340 p = 6
                // 01200300 p = 5
                // 01200000 p = 4
                // and now we can increment element for after following loop
                // 01201000
                var p = n - 1;
                while (part[p] == max[p] + 1)
                {
                    part[p] = 0;
                    p = p - 1;
                }

                #region optimalization

                // now it is (n^2) insted of (B*n)
                if (p <= n - 2 && p > 1 && CheckZeros(part, p, n))
                {
                    var tmp = CalculateCombinations(max[p], p, n);
                    if (tmp > numberOfSetsForThread - j - 1)
                    {
                        part[p] = part[p] + 1;
                        ++j;
                    }
                    else
                    {
                        part[p] = max[p] + 1;
                        for (var i = p + 1; i < n; i++)
                        {
                            part[i] = part[i - 1] + 1;
                        }
                        j += tmp;
                    }
                }
                else if (p == n - 1 && part[p] == 0)
                {
                    var tmp = (int) Math.Min(numberOfSetsForThread - j, (double) max[p] + 1);

                    part[p] = tmp;
                    j += (ulong) part[p];
                }
                else
                {
                    part[p] = part[p] + 1;
                    ++j;
                }

                #endregion

                if (j == numberOfSetsForThread)
                {
                    result[k] = new DvrpPartialProblem(partLast, approximateResult, j, part);
                    partLast = (int[]) part.Clone();
                    ++k;
                    j = 0;
                    if (k == numberOfParts - 1)
                    {
                        break;
                    }
                }

                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            } while (part[n - 1] != n - 1);
            for (var i = 0; i < n; i++)
            {
                part[i] = i;
            }
            result[k] = new DvrpPartialProblem(partLast, approximateResult,
                maxNumberofSets - (ulong) k*numberOfSetsForThread, part);
            return result;
        }

        private static ulong TriangularMethodBellNumber(int n)
        {
            var triangle = new Dictionary<long, List<ulong>> {{1, new List<ulong>(new ulong[] {1})}};
            for (var i = 2; i <= n; i++)
            {
                triangle.Add(i, new List<ulong>());
                triangle[i].Add(triangle[i - 1].Last());
                for (var k = 1; k < i; k++)
                {
                    var lastVal = triangle[i][k - 1] + triangle[i - 1][k - 1];
                    triangle[i].Add(lastVal);
                }
                triangle.Remove(i - 2);
            }
            return triangle[n].Last();
        }

        private static ulong CalculateCombinations(int max, int p, int n)
        {
            if (p == n - 2)
            {
                var tmp = (ulong) max + 2;
                tmp = tmp*(ulong) max + tmp + 1;
                return tmp;
            }
            return CalculateCombinations(max, p + 1, n)*(ulong) (max + 1) + CalculateCombinations(max + 1, p + 1, n);
        }

        private static bool CheckZeros(int[] part, int p, int n)
        {
            for (; p < n; p++)
            {
                if (part[p] != 0)
                    return false;
            }
            return true;
        }
    }
}