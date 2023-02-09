using System.Collections;

namespace NUnit.Framework.Interfaces
{
	public interface IParameterDataProvider
	{
		bool HasDataFor(IParameterInfo parameter);

		IEnumerable GetDataFor(IParameterInfo parameter);
	}
}
