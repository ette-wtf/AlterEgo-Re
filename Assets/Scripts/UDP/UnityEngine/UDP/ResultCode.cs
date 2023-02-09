namespace UnityEngine.UDP
{
	public class ResultCode
	{
		public const int SDK_INIT_SUCCESS = 0;

		public const int SDK_INIT_ERROR = -100;

		public const int SDK_NOT_INIT = -101;

		public const int SDK_PURCHASE_SUCCESS = 0;

		public const int SDK_PURCHASE_FAILED = -300;

		public const int SDK_PURCHASE_CANCEL = -301;

		public const int SDK_PURCHASE_REPEAT = -302;

		public const int SDK_CONSUME_PURCHASE_SUCCESS = 0;

		public const int SDK_CONSUME_PURCHASE_FAILED = -600;

		public const int SDK_QUERY_INVENTORY_SUCCESS = 0;

		public const int SDK_QUERY_INVENTORY_FAILED = -700;

		public const int SDK_SERVER_INVALID = -999;
	}
}
