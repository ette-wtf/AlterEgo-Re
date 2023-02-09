using System;
using System.Collections.Generic;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("facebookParameters")]
	public class NCMBFacebookParameters
	{
		internal Dictionary<string, object> param = new Dictionary<string, object>();

		public NCMBFacebookParameters(string userId, string accessToken, DateTime expirationDate)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(NCMBUtility.encodeDate(expirationDate)))
			{
				throw new NCMBException(new ArgumentException("userId or accessToken or expirationDate must not be null."));
			}
			Dictionary<string, object> value = new Dictionary<string, object>
			{
				{ "__type", "Date" },
				{
					"iso",
					NCMBUtility.encodeDate(expirationDate)
				}
			};
			Dictionary<string, object> value2 = new Dictionary<string, object>
			{
				{ "id", userId },
				{ "access_token", accessToken },
				{ "expiration_date", value }
			};
			param.Add("facebook", value2);
		}
	}
}
