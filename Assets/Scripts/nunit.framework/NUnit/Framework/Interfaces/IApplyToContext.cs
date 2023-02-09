using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
	public interface IApplyToContext
	{
		void ApplyToContext(ITestExecutionContext context);
	}
}
