using System.Collections.Generic;

namespace UnityEngine.UDP.Analytics
{
	internal class Common
	{
		private const string k_ClientId = "client_id";

		private const string k_DeviceId = "device_id";

		private const string k_EventType = "event_type";

		private const string k_Platform = "platform";

		private const string k_SystemInfo = "system_info";

		private const string k_TargetStore = "target_store";

		private const string k_TimeStamp = "ts";

		private const string k_Vr = "vr";

		private const string m_EventType_Runtime = "runtime";

		private const string k_Source = "source";

		private const string k_Source_Value = "sdk";

		public const string k_Duration = "duration";

		public const string k_Currency = "currency";

		public const string k_Price = "price";

		public const string k_ProductId = "product_id";

		public const string k_Receipt = "receipt";

		public const string k_CpOrderId = "cp_order_id";

		public const string k_Reason = "reason";

		public static Dictionary<string, object> GetCommonParams(SessionInfo sessionInfo)
		{
			return new Dictionary<string, object>
			{
				{ "client_id", sessionInfo.MClientId },
				{ "device_id", sessionInfo.MDeviceId },
				{ "event_type", "runtime" },
				{ "platform", sessionInfo.MPlatform },
				{ "system_info", sessionInfo.MSystemInfo },
				{ "target_store", sessionInfo.MTargetStore },
				{ "vr", sessionInfo.MVr },
				{
					"ts",
					PlatformWrapper.GetCurrentMillisecondsInUTC()
				},
				{ "source", "sdk" }
			};
		}
	}
}
