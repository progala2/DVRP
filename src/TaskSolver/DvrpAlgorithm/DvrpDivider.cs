using System;
using System.Collections.Generic;
using System.Linq;

namespace Dvrp.Ucc.TaskSolver.DvrpAlgorithm
{
	/// <summary>
	/// Class dividing a DVRP problem instance.
	/// </summary>
	public static class DvrpDivider
	{
		/// <summary>
		/// Divide the problem into partial problems.
		/// </summary>
		/// <param name="dvrpProblem">The problem instance to divide.</param>
		/// <param name="numberOfParts">How many parts to divide into.</param>
		/// <returns>Output partial problems.</returns>
		public static DvrpPartialProblem[] Divide(DvrpProblem dvrpProblem, int numberOfParts)
		{
			// we generate all the partitions
			// set {{0 0 0 0 0}} represents {{0 1 2 3 4}}
			// set {{0 1 2 3 4}} represents {{0}{1}{2}{3}{4}}
			// set {{0 1 2 1 2}} represents {{0}{1 3}{2 4}}
			// part - current partition 
			// max[i] = Max{max[i-1], part[i-1]}
			// max[0] = -1
			// example:
			// 0 0 0
			// 0 0 1
			// 0 1 0
			// 0 1 1
			// 0 1 2

			if (numberOfParts < 1)
				throw new ArgumentOutOfRangeException(nameof(numberOfParts));
			var length = dvrpProblem.Requests.Length;
			var part = new int[length];

			var max = new int[length];
			var maxNumberOfSets = TriangularMethodBellNumber(length);
			var numberOfSetsForThread = maxNumberOfSets / (ulong)numberOfParts;
			var result = new DvrpPartialProblem[numberOfParts];
			var solver = new DvrpSolver(dvrpProblem);
			var approximateResult = solver.SolveApproximately();

			for (var i = length - 2; i > -1; --i)
				part[i] = 0;
			part[length - 1] = max[0] = -1;
			var partLast = (int[])part.Clone();
			ulong currentNumberOfSets = 0;
			var curPartsNr = 0;
			do
			{
				// checking if max[i] == Max{max[i-1], part[i-1]}
				for (var i = 1; i < length; ++i)
				{
					if (max[i - 1] < part[i - 1])
						max[i] = part[i - 1];
					else
						max[i] = max[i - 1];
				}
				// we check if the last elements of the set are in their current maximum
				// for example
				// 01200345 pos = 7
				// 01200340 pos = 6
				// 01200300 pos = 5
				// 01200000 pos = 4
				// and now we can increment element for after following loop
				// 01201000
				var p = length - 1;
				while (part[p] == max[p] + 1)
				{
					part[p] = 0;
					--p;
				}

				#region optimalization

				// now it is O(len^2) instead of O(B*len)
				if (p <= length - 2 && p > 1 && CheckZeros(part, p, length))
				{
					var tmp = CalculateCombinations(max[p], p, length);
					if (tmp > numberOfSetsForThread - currentNumberOfSets - 1)
					{
						part[p] += 1;
						++currentNumberOfSets;
					}
					else
					{
						part[p] = max[p] + 1;
						for (var i = p + 1; i < length; i++)
						{
							part[i] = part[i - 1] + 1;
						}
						currentNumberOfSets += tmp;
					}
				}
				else if (p == length - 1 && part[p] == 0)
				{
					var tmp = (int)Math.Min(numberOfSetsForThread - currentNumberOfSets, (double)max[p] + 1);

					part[p] = tmp;
					currentNumberOfSets += (ulong)part[p];
				}
				else
				{
					part[p] += 1;
					++currentNumberOfSets;
				}

				#endregion

				if (currentNumberOfSets == numberOfSetsForThread)
				{
					result[curPartsNr] = new DvrpPartialProblem(partLast, approximateResult, currentNumberOfSets, part);
					partLast = (int[])part.Clone();
					++curPartsNr;
					currentNumberOfSets = 0;
					if (curPartsNr == numberOfParts - 1)
					{
						break;
					}
				}

			} while (part[length - 1] != length - 1);
			for (var i = 0; i < length; i++)
			{
				part[i] = i;
			}
			result[curPartsNr] = new DvrpPartialProblem(partLast, approximateResult, maxNumberOfSets - (ulong)curPartsNr * numberOfSetsForThread, part);
			return result;
		}

		/// <summary>
		/// Compute a Bell number with the triangular method.
		/// </summary>
		/// <param name="x">Which Bell number to generate.</param>
		/// <returns>Calculated Bell number.</returns>
		public static ulong TriangularMethodBellNumber(int x)
		{
			// var triangle = new Dictionary<long, List<ulong>>(x) {{1, new List<ulong>(new ulong[] {1})}};
			var triangleMinusOne = new List<ulong>(new ulong[] { 1 });
			var triangleZero = new List<ulong>(triangleMinusOne);
			for (var i = 2; i <= x; i++)
			{
				triangleZero = new List<ulong> { triangleMinusOne.Last() };
				for (var k = 1; k < i; k++)
				{
					var lastVal = triangleZero[k - 1] + triangleMinusOne[k - 1];
					triangleZero.Add(lastVal);
				}

				triangleMinusOne = triangleZero;
			}
			return triangleZero.Last();
		}

		/// <summary>
		/// Calculate the number of sets combinations to skip. 
		/// The numbers from <paramref name="pos"/> position to the end of a set should be equal to 0.
		/// </summary>
		/// <param name="max">The current maximum number in a set before <paramref name="pos"/> position.</param>
		/// <param name="pos">The position from which to start skipping combinations.</param>
		/// <param name="len">Length of a set.</param>
		/// <returns>The number of sets combinations to skip.</returns>
		private static ulong CalculateCombinations(int max, int pos, int len)
		{
			if (pos == len - 2)
			{
				var tmp = (ulong)max + 2;
				tmp = tmp * (ulong)max + tmp + 1;
				return tmp;
			}
			return CalculateCombinations(max, pos + 1, len) * (ulong)(max + 1) + CalculateCombinations(max + 1, pos + 1, len);
		}

		/// <summary>
		/// Check if the <paramref name="part"/> has zeros from <paramref name="pos"/> position to the end.
		/// </summary>
		/// <param name="part">The set.</param>
		/// <param name="pos">The position from which to start checking.</param>
		/// <param name="len">Length of the set.</param>
		/// <returns>True, if the <paramref name="part"/> has zeros from <paramref name="pos"/> the end. False otherwise.</returns>
		private static bool CheckZeros(int[] part, int pos, int len)
		{
			for (; pos < len; pos++)
			{
				if (part[pos] != 0)
					return false;
			}
			return true;
		}
	}
}