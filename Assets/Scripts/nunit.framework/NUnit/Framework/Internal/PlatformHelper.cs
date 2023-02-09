using System;
using System.Linq;

namespace NUnit.Framework.Internal
{
	public class PlatformHelper
	{
		private const string CommonOSPlatforms = "Win,Win32,Win32S,Win32NT,Win32Windows,WinCE,Win95,Win98,WinMe,NT3,NT4,NT5,NT6,Win2008Server,Win2008ServerR2,Win2012Server,Win2012ServerR2,Win2K,WinXP,Win2003Server,Vista,Win7,Windows7,Win8,Windows8,Win8.1,Windows8.1,Win10,Windows10,WindowsServer10,Unix,Linux";

		public const string OSPlatforms = "Win,Win32,Win32S,Win32NT,Win32Windows,WinCE,Win95,Win98,WinMe,NT3,NT4,NT5,NT6,Win2008Server,Win2008ServerR2,Win2012Server,Win2012ServerR2,Win2K,WinXP,Win2003Server,Vista,Win7,Windows7,Win8,Windows8,Win8.1,Windows8.1,Win10,Windows10,WindowsServer10,Unix,Linux,Xbox,MacOSX";

		private readonly OSPlatform _os;

		private readonly RuntimeFramework _rt;

		private string _reason = string.Empty;

		public static readonly string RuntimePlatforms = "Net,NetCF,SSCLI,Rotor,Mono,MonoTouch";

		public string Reason
		{
			get
			{
				return _reason;
			}
		}

		public PlatformHelper()
		{
			_os = OSPlatform.CurrentPlatform;
			_rt = RuntimeFramework.CurrentFramework;
		}

		public PlatformHelper(OSPlatform os, RuntimeFramework rt)
		{
			_os = os;
			_rt = rt;
		}

		public bool IsPlatformSupported(string[] platforms)
		{
			return platforms.Any(IsPlatformSupported);
		}

		public bool IsPlatformSupported(PlatformAttribute platformAttribute)
		{
			string include = platformAttribute.Include;
			string exclude = platformAttribute.Exclude;
			return IsPlatformSupported(include, exclude);
		}

		public bool IsPlatformSupported(TestCaseAttribute testCaseAttribute)
		{
			string includePlatform = testCaseAttribute.IncludePlatform;
			string excludePlatform = testCaseAttribute.ExcludePlatform;
			return IsPlatformSupported(includePlatform, excludePlatform);
		}

		private bool IsPlatformSupported(string include, string exclude)
		{
			try
			{
				if (include != null && !IsPlatformSupported(include))
				{
					_reason = string.Format("Only supported on {0}", include);
					return false;
				}
				if (exclude != null && IsPlatformSupported(exclude))
				{
					_reason = string.Format("Not supported on {0}", exclude);
					return false;
				}
			}
			catch (Exception ex)
			{
				_reason = ex.Message;
				return false;
			}
			return true;
		}

		public bool IsPlatformSupported(string platform)
		{
			if (platform.IndexOf(',') >= 0)
			{
				return IsPlatformSupported(platform.Split(','));
			}
			string text = platform.Trim();
			bool flag;
			switch (text.ToUpper())
			{
			case "WIN":
			case "WIN32":
				flag = _os.IsWindows;
				break;
			case "WIN32S":
				flag = _os.IsWin32S;
				break;
			case "WIN32WINDOWS":
				flag = _os.IsWin32Windows;
				break;
			case "WIN32NT":
				flag = _os.IsWin32NT;
				break;
			case "WINCE":
				flag = _os.IsWinCE;
				break;
			case "WIN95":
				flag = _os.IsWin95;
				break;
			case "WIN98":
				flag = _os.IsWin98;
				break;
			case "WINME":
				flag = _os.IsWinME;
				break;
			case "NT3":
				flag = _os.IsNT3;
				break;
			case "NT4":
				flag = _os.IsNT4;
				break;
			case "NT5":
				flag = _os.IsNT5;
				break;
			case "WIN2K":
				flag = _os.IsWin2K;
				break;
			case "WINXP":
				flag = _os.IsWinXP;
				break;
			case "WIN2003SERVER":
				flag = _os.IsWin2003Server;
				break;
			case "NT6":
				flag = _os.IsNT6;
				break;
			case "VISTA":
				flag = _os.IsVista;
				break;
			case "WIN2008SERVER":
				flag = _os.IsWin2008Server;
				break;
			case "WIN2008SERVERR2":
				flag = _os.IsWin2008ServerR2;
				break;
			case "WIN2012SERVER":
				flag = _os.IsWin2012ServerR1 || _os.IsWin2012ServerR2;
				break;
			case "WIN2012SERVERR2":
				flag = _os.IsWin2012ServerR2;
				break;
			case "WIN7":
			case "WINDOWS7":
				flag = _os.IsWindows7;
				break;
			case "WINDOWS8":
			case "WIN8":
				flag = _os.IsWindows8;
				break;
			case "WINDOWS8.1":
			case "WIN8.1":
				flag = _os.IsWindows81;
				break;
			case "WINDOWS10":
			case "WIN10":
				flag = _os.IsWindows10;
				break;
			case "WINDOWSSERVER10":
				flag = _os.IsWindowsServer10;
				break;
			case "UNIX":
			case "LINUX":
				flag = _os.IsUnix;
				break;
			case "XBOX":
				flag = _os.IsXbox;
				break;
			case "MACOSX":
				flag = _os.IsMacOSX;
				break;
			case "64-BIT":
			case "64-BIT-PROCESS":
				flag = IntPtr.Size == 8;
				break;
			case "32-BIT":
			case "32-BIT-PROCESS":
				flag = IntPtr.Size == 4;
				break;
			default:
				flag = IsRuntimeSupported(text);
				break;
			}
			if (!flag)
			{
				_reason = "Only supported on " + platform;
			}
			return flag;
		}

		private bool IsRuntimeSupported(string platformName)
		{
			string versionSpecification = null;
			string[] array = platformName.Split('-');
			if (array.Length == 2)
			{
				platformName = array[0];
				versionSpecification = array[1];
			}
			switch (platformName.ToUpper())
			{
			case "NET":
				return IsRuntimeSupported(RuntimeType.Net, versionSpecification);
			case "NETCF":
				return IsRuntimeSupported(RuntimeType.NetCF, versionSpecification);
			case "SSCLI":
			case "ROTOR":
				return IsRuntimeSupported(RuntimeType.SSCLI, versionSpecification);
			case "MONO":
				return IsRuntimeSupported(RuntimeType.Mono, versionSpecification);
			case "SL":
			case "SILVERLIGHT":
				return IsRuntimeSupported(RuntimeType.Silverlight, versionSpecification);
			case "MONOTOUCH":
				return IsRuntimeSupported(RuntimeType.MonoTouch, versionSpecification);
			default:
				throw new ArgumentException("Invalid platform name", platformName);
			}
		}

		private bool IsRuntimeSupported(RuntimeType runtime, string versionSpecification)
		{
			Version version = ((versionSpecification == null) ? RuntimeFramework.DefaultVersion : new Version(versionSpecification));
			RuntimeFramework target = new RuntimeFramework(runtime, version);
			return _rt.Supports(target);
		}
	}
}
