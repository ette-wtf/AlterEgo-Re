namespace NUnit.Framework.Internal.Commands
{
	public abstract class DelegatingTestCommand : TestCommand
	{
		protected TestCommand innerCommand;

		public TestCommand GetInnerCommand()
		{
			return innerCommand;
		}

		protected DelegatingTestCommand(TestCommand innerCommand)
			: base(innerCommand.Test)
		{
			this.innerCommand = innerCommand;
		}
	}
}
