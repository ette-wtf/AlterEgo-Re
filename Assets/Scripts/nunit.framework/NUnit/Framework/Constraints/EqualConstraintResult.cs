using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EqualConstraintResult : ConstraintResult
	{
		private object expectedValue;

		private Tolerance tolerance;

		private bool caseInsensitive;

		private bool clipStrings;

		private IList<NUnitEqualityComparer.FailurePoint> failurePoints;

		private static readonly string StringsDiffer_1 = "String lengths are both {0}. Strings differ at index {1}.";

		private static readonly string StringsDiffer_2 = "Expected string length {0} but was {1}. Strings differ at index {2}.";

		private static readonly string StreamsDiffer_1 = "Stream lengths are both {0}. Streams differ at offset {1}.";

		private static readonly string StreamsDiffer_2 = "Expected Stream length {0} but was {1}.";

		private static readonly string CollectionType_1 = "Expected and actual are both {0}";

		private static readonly string CollectionType_2 = "Expected is {0}, actual is {1}";

		private static readonly string ValuesDiffer_1 = "Values differ at index {0}";

		private static readonly string ValuesDiffer_2 = "Values differ at expected index {0}, actual index {1}";

		public EqualConstraintResult(EqualConstraint constraint, object actual, bool hasSucceeded)
			: base(constraint, actual, hasSucceeded)
		{
			expectedValue = constraint.Arguments[0];
			tolerance = constraint.Tolerance;
			caseInsensitive = constraint.CaseInsensitive;
			clipStrings = constraint.ClipStrings;
			failurePoints = constraint.FailurePoints;
		}

		public override void WriteMessageTo(MessageWriter writer)
		{
			DisplayDifferences(writer, expectedValue, base.ActualValue, 0);
		}

		private void DisplayDifferences(MessageWriter writer, object expected, object actual, int depth)
		{
			if (expected is string && actual is string)
			{
				DisplayStringDifferences(writer, (string)expected, (string)actual);
			}
			else if (expected is ICollection && actual is ICollection)
			{
				DisplayCollectionDifferences(writer, (ICollection)expected, (ICollection)actual, depth);
			}
			else if (expected is IEnumerable && actual is IEnumerable)
			{
				DisplayEnumerableDifferences(writer, (IEnumerable)expected, (IEnumerable)actual, depth);
			}
			else if (expected is Stream && actual is Stream)
			{
				DisplayStreamDifferences(writer, (Stream)expected, (Stream)actual, depth);
			}
			else if (tolerance != null)
			{
				writer.DisplayDifferences(expected, actual, tolerance);
			}
			else
			{
				writer.DisplayDifferences(expected, actual);
			}
		}

		private void DisplayStringDifferences(MessageWriter writer, string expected, string actual)
		{
			int num = MsgUtils.FindMismatchPosition(expected, actual, 0, caseInsensitive);
			if (expected.Length == actual.Length)
			{
				writer.WriteMessageLine(StringsDiffer_1, expected.Length, num);
			}
			else
			{
				writer.WriteMessageLine(StringsDiffer_2, expected.Length, actual.Length, num);
			}
			writer.DisplayStringDifferences(expected, actual, num, caseInsensitive, clipStrings);
		}

		private void DisplayStreamDifferences(MessageWriter writer, Stream expected, Stream actual, int depth)
		{
			if (expected.Length == actual.Length)
			{
				long position = failurePoints[depth].Position;
				writer.WriteMessageLine(StreamsDiffer_1, expected.Length, position);
			}
			else
			{
				writer.WriteMessageLine(StreamsDiffer_2, expected.Length, actual.Length);
			}
		}

		private void DisplayCollectionDifferences(MessageWriter writer, ICollection expected, ICollection actual, int depth)
		{
			DisplayTypesAndSizes(writer, expected, actual, depth);
			if (failurePoints.Count > depth)
			{
				NUnitEqualityComparer.FailurePoint failurePoint = failurePoints[depth];
				DisplayFailurePoint(writer, expected, actual, failurePoint, depth);
				if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
				{
					DisplayDifferences(writer, failurePoint.ExpectedValue, failurePoint.ActualValue, ++depth);
				}
				else if (failurePoint.ActualHasData)
				{
					writer.Write("  Extra:    ");
					writer.WriteCollectionElements(actual, failurePoint.Position, 3);
				}
				else
				{
					writer.Write("  Missing:  ");
					writer.WriteCollectionElements(expected, failurePoint.Position, 3);
				}
			}
		}

		private void DisplayTypesAndSizes(MessageWriter writer, IEnumerable expected, IEnumerable actual, int indent)
		{
			string text = MsgUtils.GetTypeRepresentation(expected);
			if (expected is ICollection && !(expected is Array))
			{
				text += string.Format(" with {0} elements", ((ICollection)expected).Count);
			}
			string text2 = MsgUtils.GetTypeRepresentation(actual);
			if (actual is ICollection && !(actual is Array))
			{
				text2 += string.Format(" with {0} elements", ((ICollection)actual).Count);
			}
			if (text == text2)
			{
				writer.WriteMessageLine(indent, CollectionType_1, text);
			}
			else
			{
				writer.WriteMessageLine(indent, CollectionType_2, text, text2);
			}
		}

		private void DisplayFailurePoint(MessageWriter writer, IEnumerable expected, IEnumerable actual, NUnitEqualityComparer.FailurePoint failurePoint, int indent)
		{
			Array array = expected as Array;
			Array array2 = actual as Array;
			int num = ((array == null) ? 1 : array.Rank);
			int num2 = ((array2 == null) ? 1 : array2.Rank);
			bool flag = num == num2;
			if (array != null && array2 != null)
			{
				for (int i = 1; i < num; i++)
				{
					if (!flag)
					{
						break;
					}
					if (array.GetLength(i) != array2.GetLength(i))
					{
						flag = false;
					}
				}
			}
			int[] arrayIndicesFromCollectionIndex = MsgUtils.GetArrayIndicesFromCollectionIndex(expected, failurePoint.Position);
			if (flag)
			{
				writer.WriteMessageLine(indent, ValuesDiffer_1, MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex));
				return;
			}
			int[] arrayIndicesFromCollectionIndex2 = MsgUtils.GetArrayIndicesFromCollectionIndex(actual, failurePoint.Position);
			writer.WriteMessageLine(indent, ValuesDiffer_2, MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex), MsgUtils.GetArrayIndicesAsString(arrayIndicesFromCollectionIndex2));
		}

		private static object GetValueFromCollection(ICollection collection, int index)
		{
			Array array = collection as Array;
			if (array != null && array.Rank > 1)
			{
				return array.GetValue(MsgUtils.GetArrayIndicesFromCollectionIndex(array, index));
			}
			if (collection is IList)
			{
				return ((IList)collection)[index];
			}
			foreach (object item in collection)
			{
				if (--index < 0)
				{
					return item;
				}
			}
			return null;
		}

		private void DisplayEnumerableDifferences(MessageWriter writer, IEnumerable expected, IEnumerable actual, int depth)
		{
			DisplayTypesAndSizes(writer, expected, actual, depth);
			if (failurePoints.Count > depth)
			{
				NUnitEqualityComparer.FailurePoint failurePoint = failurePoints[depth];
				DisplayFailurePoint(writer, expected, actual, failurePoint, depth);
				if (failurePoint.ExpectedHasData && failurePoint.ActualHasData)
				{
					DisplayDifferences(writer, failurePoint.ExpectedValue, failurePoint.ActualValue, ++depth);
				}
			}
		}
	}
}
