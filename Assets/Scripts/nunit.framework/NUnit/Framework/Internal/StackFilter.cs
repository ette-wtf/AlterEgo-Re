using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NUnit.Framework.Internal
{
	public static class StackFilter
	{
		private static readonly Regex assertOrAssumeRegex = new Regex(" NUnit\\.Framework\\.Ass(ert|ume)\\.");

		public static string Filter(string rawTrace)
		{
			if (rawTrace == null)
			{
				return null;
			}
			StringReader stringReader = new StringReader(rawTrace);
			StringWriter stringWriter = new StringWriter();
			try
			{
				string text;
				while ((text = stringReader.ReadLine()) != null && assertOrAssumeRegex.IsMatch(text))
				{
				}
				while (text != null && text.IndexOf(" System.Reflection.") < 0)
				{
					stringWriter.WriteLine(text.Trim());
					text = stringReader.ReadLine();
				}
			}
			catch (Exception)
			{
				return rawTrace;
			}
			return stringWriter.ToString();
		}
	}
}
