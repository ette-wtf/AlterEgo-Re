namespace NUnit.Framework.Interfaces
{
	public interface IReflectionInfo
	{
		T[] GetCustomAttributes<T>(bool inherit) where T : class;

		bool IsDefined<T>(bool inherit);
	}
}
