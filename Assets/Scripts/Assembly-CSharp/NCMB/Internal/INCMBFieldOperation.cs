namespace NCMB.Internal
{
	internal interface INCMBFieldOperation
	{
		object Encode();

		INCMBFieldOperation MergeWithPrevious(INCMBFieldOperation previous);

		object Apply(object oldValue, NCMBObject obj, string key);
	}
}
