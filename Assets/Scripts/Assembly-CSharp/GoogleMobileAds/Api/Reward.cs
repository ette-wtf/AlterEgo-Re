using System;

namespace GoogleMobileAds.Api
{
	public class Reward : EventArgs
	{
		public string Type { get; set; }

		public double Amount { get; set; }
	}
}
