using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class PairwiseStrategy : ICombiningStrategy
	{
		internal class FleaRand
		{
			private uint _b;

			private uint _c;

			private uint _d;

			private uint _z;

			private uint[] _m;

			private uint[] _r;

			private uint _q;

			public FleaRand(uint seed)
			{
				_b = seed;
				_c = seed;
				_d = seed;
				_z = seed;
				_m = new uint[256];
				_r = new uint[256];
				for (int i = 0; i < _m.Length; i++)
				{
					_m[i] = seed;
				}
				for (int i = 0; i < 10; i++)
				{
					Batch();
				}
				_q = 0u;
			}

			public uint Next()
			{
				if (_q == 0)
				{
					Batch();
					_q = (uint)(_r.Length - 1);
				}
				else
				{
					_q--;
				}
				return _r[_q];
			}

			private void Batch()
			{
				uint num = _b;
				uint num2 = _c + ++_z;
				uint num3 = _d;
				for (int i = 0; i < _r.Length; i++)
				{
					uint num4 = _m[(long)num % (long)_m.Length];
					_m[(long)num % (long)_m.Length] = num3;
					num3 = (num2 << 19) + (num2 >> 13) + num;
					num2 = num ^ _m[i];
					num = num4 + num3;
					_r[i] = num2;
				}
				_b = num;
				_c = num2;
				_d = num3;
			}
		}

		internal class FeatureInfo
		{
			public readonly int Dimension;

			public readonly int Feature;

			public FeatureInfo(int dimension, int feature)
			{
				Dimension = dimension;
				Feature = feature;
			}
		}

		internal class FeatureTuple
		{
			private readonly FeatureInfo[] _features;

			public int Length
			{
				get
				{
					return _features.Length;
				}
			}

			public FeatureInfo this[int index]
			{
				get
				{
					return _features[index];
				}
			}

			public FeatureTuple(FeatureInfo feature1)
			{
				_features = new FeatureInfo[1] { feature1 };
			}

			public FeatureTuple(FeatureInfo feature1, FeatureInfo feature2)
			{
				_features = new FeatureInfo[2] { feature1, feature2 };
			}
		}

		internal class TestCaseInfo
		{
			public readonly int[] Features;

			public TestCaseInfo(int length)
			{
				Features = new int[length];
			}

			public bool IsTupleCovered(FeatureTuple tuple)
			{
				for (int i = 0; i < tuple.Length; i++)
				{
					if (Features[tuple[i].Dimension] != tuple[i].Feature)
					{
						return false;
					}
				}
				return true;
			}
		}

		internal class PairwiseTestCaseGenerator
		{
			private FleaRand _prng;

			private int[] _dimensions;

			private List<FeatureTuple>[][] _uncoveredTuples;

			public IEnumerable GetTestCases(int[] dimensions)
			{
				_prng = new FleaRand(15485863u);
				_dimensions = dimensions;
				CreateAllTuples();
				List<TestCaseInfo> list = new List<TestCaseInfo>();
				while (true)
				{
					bool flag = true;
					FeatureTuple nextTuple = GetNextTuple();
					if (nextTuple == null)
					{
						break;
					}
					TestCaseInfo testCaseInfo = CreateTestCase(nextTuple);
					RemoveTuplesCoveredByTest(testCaseInfo);
					list.Add(testCaseInfo);
				}
				SelfTest(list);
				return list;
			}

			private int GetNextRandomNumber()
			{
				return (int)(_prng.Next() >> 1);
			}

			private void CreateAllTuples()
			{
				_uncoveredTuples = new List<FeatureTuple>[_dimensions.Length][];
				for (int i = 0; i < _dimensions.Length; i++)
				{
					_uncoveredTuples[i] = new List<FeatureTuple>[_dimensions[i]];
					for (int j = 0; j < _dimensions[i]; j++)
					{
						_uncoveredTuples[i][j] = CreateTuples(i, j);
					}
				}
			}

			private List<FeatureTuple> CreateTuples(int dimension, int feature)
			{
				List<FeatureTuple> list = new List<FeatureTuple>();
				list.Add(new FeatureTuple(new FeatureInfo(dimension, feature)));
				for (int i = 0; i < _dimensions.Length; i++)
				{
					if (i != dimension)
					{
						for (int j = 0; j < _dimensions[i]; j++)
						{
							list.Add(new FeatureTuple(new FeatureInfo(dimension, feature), new FeatureInfo(i, j)));
						}
					}
				}
				return list;
			}

			private FeatureTuple GetNextTuple()
			{
				for (int i = 0; i < _uncoveredTuples.Length; i++)
				{
					for (int j = 0; j < _uncoveredTuples[i].Length; j++)
					{
						List<FeatureTuple> list = _uncoveredTuples[i][j];
						if (list.Count > 0)
						{
							FeatureTuple result = list[0];
							list.RemoveAt(0);
							return result;
						}
					}
				}
				return null;
			}

			private TestCaseInfo CreateTestCase(FeatureTuple tuple)
			{
				TestCaseInfo result = null;
				int num = -1;
				for (int i = 0; i < 7; i++)
				{
					TestCaseInfo testCaseInfo = CreateRandomTestCase(tuple);
					int num2 = MaximizeCoverage(testCaseInfo, tuple);
					if (num2 > num)
					{
						result = testCaseInfo;
						num = num2;
					}
				}
				return result;
			}

			private TestCaseInfo CreateRandomTestCase(FeatureTuple tuple)
			{
				TestCaseInfo testCaseInfo = new TestCaseInfo(_dimensions.Length);
				for (int i = 0; i < _dimensions.Length; i++)
				{
					testCaseInfo.Features[i] = GetNextRandomNumber() % _dimensions[i];
				}
				for (int j = 0; j < tuple.Length; j++)
				{
					testCaseInfo.Features[tuple[j].Dimension] = tuple[j].Feature;
				}
				return testCaseInfo;
			}

			private int MaximizeCoverage(TestCaseInfo testCase, FeatureTuple tuple)
			{
				int num = 1;
				int[] mutableDimensions = GetMutableDimensions(tuple);
				bool flag2;
				do
				{
					bool flag = true;
					flag2 = false;
					ScrambleDimensions(mutableDimensions);
					foreach (int num2 in mutableDimensions)
					{
						int num3 = CountTuplesCoveredByTest(testCase, num2, testCase.Features[num2]);
						int num4 = MaximizeCoverageForDimension(testCase, num2, num3);
						num += num4;
						if (num4 > num3)
						{
							flag2 = true;
						}
					}
				}
				while (flag2);
				return num;
			}

			private int[] GetMutableDimensions(FeatureTuple tuple)
			{
				List<int> list = new List<int>();
				bool[] array = new bool[_dimensions.Length];
				for (int i = 0; i < tuple.Length; i++)
				{
					array[tuple[i].Dimension] = true;
				}
				for (int j = 0; j < _dimensions.Length; j++)
				{
					if (!array[j])
					{
						list.Add(j);
					}
				}
				return list.ToArray();
			}

			private void ScrambleDimensions(int[] dimensions)
			{
				for (int i = 0; i < dimensions.Length; i++)
				{
					int num = GetNextRandomNumber() % dimensions.Length;
					int num2 = dimensions[i];
					dimensions[i] = dimensions[num];
					dimensions[num] = num2;
				}
			}

			private int MaximizeCoverageForDimension(TestCaseInfo testCase, int dimension, int bestCoverage)
			{
				List<int> list = new List<int>(_dimensions[dimension]);
				for (int i = 0; i < _dimensions[dimension]; i++)
				{
					testCase.Features[dimension] = i;
					int num = CountTuplesCoveredByTest(testCase, dimension, i);
					if (num >= bestCoverage)
					{
						if (num > bestCoverage)
						{
							bestCoverage = num;
							list.Clear();
						}
						list.Add(i);
					}
				}
				testCase.Features[dimension] = list[GetNextRandomNumber() % list.Count];
				return bestCoverage;
			}

			private int CountTuplesCoveredByTest(TestCaseInfo testCase, int dimension, int feature)
			{
				int num = 0;
				List<FeatureTuple> list = _uncoveredTuples[dimension][feature];
				for (int i = 0; i < list.Count; i++)
				{
					if (testCase.IsTupleCovered(list[i]))
					{
						num++;
					}
				}
				return num;
			}

			private void RemoveTuplesCoveredByTest(TestCaseInfo testCase)
			{
				for (int i = 0; i < _uncoveredTuples.Length; i++)
				{
					for (int j = 0; j < _uncoveredTuples[i].Length; j++)
					{
						List<FeatureTuple> list = _uncoveredTuples[i][j];
						for (int num = list.Count - 1; num >= 0; num--)
						{
							if (testCase.IsTupleCovered(list[num]))
							{
								list.RemoveAt(num);
							}
						}
					}
				}
			}

			private void SelfTest(List<TestCaseInfo> testCases)
			{
				for (int i = 0; i < _dimensions.Length - 1; i++)
				{
					for (int j = i + 1; j < _dimensions.Length; j++)
					{
						for (int k = 0; k < _dimensions[i]; k++)
						{
							for (int l = 0; l < _dimensions[j]; l++)
							{
								FeatureTuple featureTuple = new FeatureTuple(new FeatureInfo(i, k), new FeatureInfo(j, l));
								if (!IsTupleCovered(testCases, featureTuple))
								{
									throw new InvalidOperationException(string.Format("PairwiseStrategy : Not all pairs are covered : {0}", featureTuple.ToString()));
								}
							}
						}
					}
				}
			}

			private bool IsTupleCovered(List<TestCaseInfo> testCases, FeatureTuple tuple)
			{
				foreach (TestCaseInfo testCase in testCases)
				{
					if (testCase.IsTupleCovered(tuple))
					{
						return true;
					}
				}
				return false;
			}
		}

		public IEnumerable<ITestCaseData> GetTestCases(IEnumerable[] sources)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			List<object>[] array = CreateValueSet(sources);
			int[] dimensions = CreateDimensions(array);
			IEnumerable testCases = new PairwiseTestCaseGenerator().GetTestCases(dimensions);
			foreach (TestCaseInfo item2 in testCases)
			{
				object[] array2 = new object[item2.Features.Length];
				for (int i = 0; i < item2.Features.Length; i++)
				{
					array2[i] = array[i][item2.Features[i]];
				}
				TestCaseParameters item = new TestCaseParameters(array2);
				list.Add(item);
			}
			return list;
		}

		private List<object>[] CreateValueSet(IEnumerable[] sources)
		{
			List<object>[] array = new List<object>[sources.Length];
			for (int i = 0; i < array.Length; i++)
			{
				List<object> list = new List<object>();
				foreach (object item in sources[i])
				{
					list.Add(item);
				}
				array[i] = list;
			}
			return array;
		}

		private int[] CreateDimensions(List<object>[] valueSet)
		{
			int[] array = new int[valueSet.Length];
			for (int i = 0; i < valueSet.Length; i++)
			{
				array[i] = valueSet[i].Count;
			}
			return array;
		}
	}
}
