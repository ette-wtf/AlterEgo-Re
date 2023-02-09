using UnityEngine.UDP.Analytics.Events;

namespace UnityEngine.UDP.Analytics
{
	public class UdpAnalytics
	{
		public static AnalyticsResult Transaction(string productionId, string price, string currency, string receipt, string cpOrderId)
		{
			if (!isInitialized())
			{
				return AnalyticsResult.kNotInitialized;
			}
			if (string.IsNullOrEmpty(productionId))
			{
				return AnalyticsResult.kInvalidData;
			}
			return dispatchEvent(new TransactionEvent(cpOrderId, productionId, currency, price, receipt));
		}

		public static AnalyticsResult TransactionFailed(string productionId, string cpOrderId, string reason)
		{
			if (!isInitialized())
			{
				return AnalyticsResult.kNotInitialized;
			}
			if (string.IsNullOrEmpty(productionId))
			{
				return AnalyticsResult.kInvalidData;
			}
			return dispatchEvent(new TransactionFailedEvent(cpOrderId, productionId, reason));
		}

		public static AnalyticsResult PurchaseAttempt(string productionId, string uuid)
		{
			if (!isInitialized())
			{
				return AnalyticsResult.kNotInitialized;
			}
			if (string.IsNullOrEmpty(productionId))
			{
				return AnalyticsResult.kInvalidData;
			}
			return dispatchEvent(new PurchaseAttemptEvent(productionId, uuid));
		}

		private static AnalyticsResult dispatchEvent(object e)
		{
			EventDispatcher.DispatchEvent(e);
			return AnalyticsResult.kOk;
		}

		private static bool isInitialized()
		{
			if (string.IsNullOrEmpty(AnalyticsClient.GetSessionId()))
			{
				return false;
			}
			return true;
		}
	}
}
