namespace NCMB.Internal
{
	internal class NCMBDeleteOperation : INCMBFieldOperation
	{
		public object getValue()
		{
			return null;
		}

		public object Encode()
		{
			return null;
		}

		public INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous)
		{
			return this;
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			return null;
		}
	}
}
