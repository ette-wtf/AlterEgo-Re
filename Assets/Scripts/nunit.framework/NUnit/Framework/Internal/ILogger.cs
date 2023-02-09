namespace NUnit.Framework.Internal
{
	public interface ILogger
	{
		void Error(string message);

		void Error(string message, params object[] args);

		void Warning(string message);

		void Warning(string message, params object[] args);

		void Info(string message);

		void Info(string message, params object[] args);

		void Debug(string message);

		void Debug(string message, params object[] args);
	}
}
