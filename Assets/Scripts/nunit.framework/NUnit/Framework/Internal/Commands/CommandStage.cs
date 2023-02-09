namespace NUnit.Framework.Internal.Commands
{
	public enum CommandStage
	{
		Default = 0,
		BelowSetUpTearDown = 1,
		SetUpTearDown = 2,
		AboveSetUpTearDown = 3
	}
}
