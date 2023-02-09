namespace NCMB.Internal
{
	internal class NCMBSetOperation : INCMBFieldOperation
	{
		public object Value;

		public NCMBSetOperation(object value)
		{
			Value = value;
		}

		public object getValue()
		{
			return Value;
		}

		public object Encode()
		{
			return NCMBUtility._maybeEncodeJSONObject(Value, true);
		}

		public INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous)
		{
			return this;
		}

		public object Apply(object oldValue, NCMBObject obj, string key)
		{
			return Value;
		}
	}
}
