using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal
{
	public class TextMessageWriter : MessageWriter
	{
		private static readonly int DEFAULT_LINE_LENGTH = 78;

		public static readonly string Pfx_Expected = "  Expected: ";

		public static readonly string Pfx_Actual = "  But was:  ";

		public static readonly int PrefixLength = Pfx_Expected.Length;

		private int maxLineLength = DEFAULT_LINE_LENGTH;

		public override int MaxLineLength
		{
			get
			{
				return maxLineLength;
			}
			set
			{
				maxLineLength = value;
			}
		}

		public TextMessageWriter()
		{
		}

		public TextMessageWriter(string userMessage, params object[] args)
		{
			if (userMessage != null && userMessage != string.Empty)
			{
				WriteMessageLine(userMessage, args);
			}
		}

		public override void WriteMessageLine(int level, string message, params object[] args)
		{
			if (message != null)
			{
				while (level-- >= 0)
				{
					Write("  ");
				}
				if (args != null && args.Length > 0)
				{
					message = string.Format(message, args);
				}
				WriteLine(MsgUtils.EscapeNullCharacters(message));
			}
		}

		public override void DisplayDifferences(ConstraintResult result)
		{
			WriteExpectedLine(result);
			WriteActualLine(result);
		}

		public override void DisplayDifferences(object expected, object actual)
		{
			WriteExpectedLine(expected);
			WriteActualLine(actual);
		}

		public override void DisplayDifferences(object expected, object actual, Tolerance tolerance)
		{
			WriteExpectedLine(expected, tolerance);
			WriteActualLine(actual);
		}

		public override void DisplayStringDifferences(string expected, string actual, int mismatch, bool ignoreCase, bool clipping)
		{
			int maxDisplayLength = MaxLineLength - PrefixLength - 2;
			if (clipping)
			{
				MsgUtils.ClipExpectedAndActual(ref expected, ref actual, maxDisplayLength, mismatch);
			}
			expected = MsgUtils.EscapeControlChars(expected);
			actual = MsgUtils.EscapeControlChars(actual);
			mismatch = MsgUtils.FindMismatchPosition(expected, actual, 0, ignoreCase);
			Write(Pfx_Expected);
			Write(MsgUtils.FormatValue(expected));
			if (ignoreCase)
			{
				Write(", ignoring case");
			}
			WriteLine();
			WriteActualLine(actual);
			if (mismatch >= 0)
			{
				WriteCaretLine(mismatch);
			}
		}

		public override void WriteActualValue(object actual)
		{
			WriteValue(actual);
		}

		public override void WriteValue(object val)
		{
			Write(MsgUtils.FormatValue(val));
		}

		public override void WriteCollectionElements(IEnumerable collection, long start, int max)
		{
			Write(MsgUtils.FormatCollection(collection, start, max));
		}

		private void WriteExpectedLine(ConstraintResult result)
		{
			Write(Pfx_Expected);
			WriteLine(result.Description);
		}

		private void WriteExpectedLine(object expected)
		{
			WriteExpectedLine(expected, null);
		}

		private void WriteExpectedLine(object expected, Tolerance tolerance)
		{
			Write(Pfx_Expected);
			Write(MsgUtils.FormatValue(expected));
			if (tolerance != null && !tolerance.IsUnsetOrDefault)
			{
				Write(" +/- ");
				Write(MsgUtils.FormatValue(tolerance.Value));
				if (tolerance.Mode != ToleranceMode.Linear)
				{
					Write(" {0}", tolerance.Mode);
				}
			}
			WriteLine();
		}

		private void WriteActualLine(ConstraintResult result)
		{
			Write(Pfx_Actual);
			result.WriteActualValueTo(this);
			WriteLine();
		}

		private void WriteActualLine(object actual)
		{
			Write(Pfx_Actual);
			WriteActualValue(actual);
			WriteLine();
		}

		private void WriteCaretLine(int mismatch)
		{
			WriteLine("  {0}^", new string('-', PrefixLength + mismatch - 2 + 1));
		}
	}
}
