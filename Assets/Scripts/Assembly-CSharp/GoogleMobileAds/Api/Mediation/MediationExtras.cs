using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation
{
	public abstract class MediationExtras
	{
		public Dictionary<string, string> Extras { get; protected set; }

		public abstract string AndroidMediationExtraBuilderClassName { get; }

		public abstract string IOSMediationExtraBuilderClassName { get; }

		public MediationExtras()
		{
			Extras = new Dictionary<string, string>();
		}
	}
}
