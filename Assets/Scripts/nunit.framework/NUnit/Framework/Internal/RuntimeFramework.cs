using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace NUnit.Framework.Internal
{
	[Serializable]
	public sealed class RuntimeFramework
	{
		public static readonly Version DefaultVersion = new Version(0, 0);

		private static readonly Lazy<RuntimeFramework> currentFramework = new Lazy<RuntimeFramework>(delegate
		{
			Type type = Type.GetType("Mono.Runtime", false);
			Type type2 = Type.GetType("MonoTouch.UIKit.UIApplicationDelegate,monotouch");
			bool flag = (object)type2 != null;
			bool flag2 = (object)type != null;
			RuntimeType runtime = (flag ? RuntimeType.MonoTouch : (flag2 ? RuntimeType.Mono : ((Environment.OSVersion.Platform != PlatformID.WinCE) ? RuntimeType.Net : RuntimeType.NetCF)));
			int num = Environment.Version.Major;
			int minor = Environment.Version.Minor;
			if (flag2)
			{
				switch (num)
				{
				case 1:
					minor = 0;
					break;
				case 2:
					num = 3;
					minor = 5;
					break;
				}
			}
			else
			{
				switch (num)
				{
				case 2:
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework"))
					{
						if (registryKey != null)
						{
							string text = registryKey.GetValue("InstallRoot") as string;
							if (text != null)
							{
								if (Directory.Exists(Path.Combine(text, "v3.5")))
								{
									num = 3;
									minor = 5;
								}
								else if (Directory.Exists(Path.Combine(text, "v3.0")))
								{
									num = 3;
									minor = 0;
								}
							}
						}
					}
					break;
				}
				case 4:
					if ((object)Type.GetType("System.Reflection.AssemblyMetadataAttribute") != null)
					{
						minor = 5;
					}
					break;
				}
			}
			RuntimeFramework runtimeFramework = new RuntimeFramework(runtime, new Version(num, minor))
			{
				ClrVersion = Environment.Version
			};
			if (flag2)
			{
				MethodInfo method = type.GetMethod("GetDisplayName", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.ExactBinding);
				if ((object)method != null)
				{
					runtimeFramework.DisplayName = (string)method.Invoke(null, new object[0]);
				}
			}
			return runtimeFramework;
		});

		public static RuntimeFramework CurrentFramework
		{
			get
			{
				return currentFramework.Value;
			}
		}

		public RuntimeType Runtime { get; private set; }

		public Version FrameworkVersion { get; private set; }

		public Version ClrVersion { get; private set; }

		public bool AllowAnyVersion
		{
			get
			{
				return ClrVersion == DefaultVersion;
			}
		}

		public string DisplayName { get; private set; }

		public RuntimeFramework(RuntimeType runtime, Version version)
		{
			Runtime = runtime;
			if (version.Build < 0)
			{
				InitFromFrameworkVersion(version);
			}
			else
			{
				InitFromClrVersion(version);
			}
			DisplayName = GetDefaultDisplayName(runtime, version);
		}

		private void InitFromFrameworkVersion(Version version)
		{
			Version frameworkVersion = (ClrVersion = version);
			FrameworkVersion = frameworkVersion;
			if (version.Major <= 0)
			{
				return;
			}
			switch (Runtime)
			{
			case RuntimeType.Any:
			case RuntimeType.Net:
			case RuntimeType.Mono:
				switch (version.Major)
				{
				case 1:
					switch (version.Minor)
					{
					case 0:
						ClrVersion = ((Runtime == RuntimeType.Mono) ? new Version(1, 1, 4322) : new Version(1, 0, 3705));
						break;
					case 1:
						if (Runtime == RuntimeType.Mono)
						{
							FrameworkVersion = new Version(1, 0);
						}
						ClrVersion = new Version(1, 1, 4322);
						break;
					default:
						ThrowInvalidFrameworkVersion(version);
						break;
					}
					break;
				case 2:
				case 3:
					ClrVersion = new Version(2, 0, 50727);
					break;
				case 4:
					ClrVersion = new Version(4, 0, 30319);
					break;
				default:
					ThrowInvalidFrameworkVersion(version);
					break;
				}
				break;
			case RuntimeType.Silverlight:
				ClrVersion = ((version.Major >= 4) ? new Version(4, 0, 60310) : new Version(2, 0, 50727));
				break;
			case RuntimeType.NetCF:
			{
				int major = version.Major;
				if (major == 3)
				{
					major = version.Minor;
					if (major == 5)
					{
						ClrVersion = new Version(3, 5, 7283);
					}
				}
				break;
			}
			case RuntimeType.SSCLI:
				break;
			}
		}

		private static void ThrowInvalidFrameworkVersion(Version version)
		{
			throw new ArgumentException("Unknown framework version " + version, "version");
		}

		private void InitFromClrVersion(Version version)
		{
			FrameworkVersion = new Version(version.Major, version.Minor);
			ClrVersion = version;
			if (Runtime == RuntimeType.Mono && version.Major == 1)
			{
				FrameworkVersion = new Version(1, 0);
			}
		}

		public static RuntimeFramework Parse(string s)
		{
			RuntimeType runtime = RuntimeType.Any;
			Version version = DefaultVersion;
			string[] array = s.Split('-');
			if (array.Length == 2)
			{
				runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), array[0], true);
				string text = array[1];
				if (text != "")
				{
					version = new Version(text);
				}
			}
			else if (char.ToLower(s[0]) == 'v')
			{
				version = new Version(s.Substring(1));
			}
			else if (IsRuntimeTypeName(s))
			{
				runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), s, true);
			}
			else
			{
				version = new Version(s);
			}
			return new RuntimeFramework(runtime, version);
		}

		public override string ToString()
		{
			if (AllowAnyVersion)
			{
				return Runtime.ToString().ToLower();
			}
			string text = FrameworkVersion.ToString();
			if (Runtime == RuntimeType.Any)
			{
				return "v" + text;
			}
			return Runtime.ToString().ToLower() + "-" + text;
		}

		public bool Supports(RuntimeFramework target)
		{
			if (Runtime != 0 && target.Runtime != 0 && Runtime != target.Runtime)
			{
				return false;
			}
			if (AllowAnyVersion || target.AllowAnyVersion)
			{
				return true;
			}
			if (!VersionsMatch(ClrVersion, target.ClrVersion))
			{
				return false;
			}
			return (Runtime != RuntimeType.Silverlight) ? (FrameworkVersion.Major >= target.FrameworkVersion.Major && FrameworkVersion.Minor >= target.FrameworkVersion.Minor) : (FrameworkVersion.Major == target.FrameworkVersion.Major && FrameworkVersion.Minor == target.FrameworkVersion.Minor);
		}

		private static bool IsRuntimeTypeName(string name)
		{
			return TypeHelper.GetEnumNames(typeof(RuntimeType)).Any((string item) => item.ToLower() == name.ToLower());
		}

		private static string GetDefaultDisplayName(RuntimeType runtime, Version version)
		{
			if (version == DefaultVersion)
			{
				return runtime.ToString();
			}
			if (runtime == RuntimeType.Any)
			{
				return "v" + version;
			}
			return string.Concat(runtime, " ", version);
		}

		private static bool VersionsMatch(Version v1, Version v2)
		{
			return v1.Major == v2.Major && v1.Minor == v2.Minor && (v1.Build < 0 || v2.Build < 0 || v1.Build == v2.Build) && (v1.Revision < 0 || v2.Revision < 0 || v1.Revision == v2.Revision);
		}
	}
}
