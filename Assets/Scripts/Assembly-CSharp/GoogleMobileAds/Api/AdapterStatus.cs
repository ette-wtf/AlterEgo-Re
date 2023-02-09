namespace GoogleMobileAds.Api
{
	public class AdapterStatus
	{
		public AdapterState InitializationState { get; private set; }

		public string Description { get; private set; }

		public int Latency { get; private set; }

		internal AdapterStatus(AdapterState state, string description, int latency)
		{
			InitializationState = state;
			Description = description;
			Latency = latency;
		}
	}
}
