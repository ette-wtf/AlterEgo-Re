using System;
using System.Collections.Generic;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("twitterParameters")]
	public class NCMBTwitterParameters
	{
		internal Dictionary<string, object> param = new Dictionary<string, object>();

		public NCMBTwitterParameters(string userId, string screenName, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(screenName) || string.IsNullOrEmpty(consumerKey) || string.IsNullOrEmpty(consumerSecret) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(accessTokenSecret))
			{
				throw new NCMBException(new ArgumentException("constructor parameters must not be null."));
			}
			Dictionary<string, object> value = new Dictionary<string, object>
			{
				{ "id", userId },
				{ "screen_name", screenName },
				{ "oauth_consumer_key", consumerKey },
				{ "consumer_secret", consumerSecret },
				{ "oauth_token", accessToken },
				{ "oauth_token_secret", accessTokenSecret }
			};
			param.Add("twitter", value);
		}
	}
}
