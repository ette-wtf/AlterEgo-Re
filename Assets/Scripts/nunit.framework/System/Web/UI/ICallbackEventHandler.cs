namespace System.Web.UI
{
	public interface ICallbackEventHandler
	{
		void RaiseCallbackEvent(string report);

		string GetCallbackResult();
	}
}
