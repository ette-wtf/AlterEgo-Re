using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Interfaces
{
	public interface ICommandWrapper
	{
		TestCommand Wrap(TestCommand command);
	}
}
