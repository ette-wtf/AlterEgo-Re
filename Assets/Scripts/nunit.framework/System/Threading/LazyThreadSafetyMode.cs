namespace System.Threading
{
	internal enum LazyThreadSafetyMode
	{
		None = 0,
		PublicationOnly = 1,
		ExecutionAndPublication = 2
	}
}
