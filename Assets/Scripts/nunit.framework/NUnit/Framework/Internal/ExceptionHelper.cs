using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class ExceptionHelper
	{
		private static readonly Action<Exception> PreserveStackTrace;

		static ExceptionHelper()
		{
			MethodInfo method = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
			if ((object)method != null)
			{
				try
				{
					PreserveStackTrace = (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>), method);
					return;
				}
				catch (InvalidOperationException)
				{
				}
			}
			PreserveStackTrace = delegate
			{
			};
		}

		public static void Rethrow(Exception exception)
		{
			PreserveStackTrace(exception);
			throw exception;
		}

		public static string BuildMessage(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0} : {1}", new object[2]
			{
				exception.GetType().ToString(),
				exception.Message
			});
			foreach (Exception item in FlattenExceptionHierarchy(exception))
			{
				stringBuilder.Append(Env.NewLine);
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "  ----> {0} : {1}", new object[2]
				{
					item.GetType().ToString(),
					item.Message
				});
			}
			return stringBuilder.ToString();
		}

		public static string BuildStackTrace(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(GetStackTrace(exception));
			foreach (Exception item in FlattenExceptionHierarchy(exception))
			{
				stringBuilder.Append(Env.NewLine);
				stringBuilder.Append("--");
				stringBuilder.Append(item.GetType().Name);
				stringBuilder.Append(Env.NewLine);
				stringBuilder.Append(GetStackTrace(item));
			}
			return stringBuilder.ToString();
		}

		public static string GetStackTrace(Exception exception)
		{
			try
			{
				return exception.StackTrace;
			}
			catch (Exception)
			{
				return "No stack trace available";
			}
		}

		private static List<Exception> FlattenExceptionHierarchy(Exception exception)
		{
			List<Exception> list = new List<Exception>();
			if (exception.InnerException != null)
			{
				list.Add(exception.InnerException);
				list.AddRange(FlattenExceptionHierarchy(exception.InnerException));
			}
			return list;
		}
	}
}
