using System.Collections;

namespace NUnit.Framework.Interfaces
{
	public interface IParameterDataSource
	{
		IEnumerable GetData(IParameterInfo parameter);
	}
}
