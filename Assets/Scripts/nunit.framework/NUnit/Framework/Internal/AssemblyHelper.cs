using System;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public static class AssemblyHelper
	{
		public static string GetAssemblyPath(Assembly assembly)
		{
			string codeBase = assembly.CodeBase;
			if (IsFileUri(codeBase))
			{
				return GetAssemblyPathFromCodeBase(codeBase);
			}
			return assembly.Location;
		}

		public static string GetDirectoryName(Assembly assembly)
		{
			return Path.GetDirectoryName(GetAssemblyPath(assembly));
		}

		public static AssemblyName GetAssemblyName(Assembly assembly)
		{
			return assembly.GetName();
		}

		public static Assembly Load(string nameOrPath)
		{
			string text = Path.GetExtension(nameOrPath).ToLower();
			if (text == ".dll" || text == ".exe")
			{
				return Assembly.Load(AssemblyName.GetAssemblyName(nameOrPath));
			}
			return Assembly.Load(nameOrPath);
		}

		private static bool IsFileUri(string uri)
		{
			return uri.ToLower().StartsWith(Uri.UriSchemeFile);
		}

		public static string GetAssemblyPathFromCodeBase(string codeBase)
		{
			int num = Uri.UriSchemeFile.Length + Uri.SchemeDelimiter.Length;
			if (codeBase[num] == '/')
			{
				if (codeBase[num + 2] == ':')
				{
					num++;
				}
			}
			else if (codeBase[num + 1] != ':')
			{
				num -= 2;
			}
			return codeBase.Substring(num);
		}
	}
}
