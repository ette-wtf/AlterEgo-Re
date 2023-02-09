namespace UnityEngine.Store
{
	internal class ResultCode
	{
		public static readonly int SDK_INIT_SUCCESS = 0;

		public static readonly int SDK_INIT_ERROR = -100;

		public static readonly int SDK_NOT_INIT = -101;

		public static readonly int SDK_LOGIN_SUCCESS = 0;

		public static readonly int SDK_LOGIN_CANCEL = -200;

		public static readonly int SDK_LOGIN_FAILED = -201;

		public static readonly int SDK_NOT_LOGIN = -202;

		public static readonly int SDK_PURCHASE_SUCCESS = 0;

		public static readonly int SDK_PURCHASE_FAILED = -300;

		public static readonly int SDK_PURCHASE_CANCEL = -301;

		public static readonly int SDK_PURCHASE_REPEAT = -302;

		public static readonly int SDK_RECEIPT_VALIDATE_SUCCESS = 0;

		public static readonly int SDK_RECEIPT_VALIDATE_FAILED = -400;

		public static readonly int SDK_RECEIPT_VALIDATE_ERROR = -401;

		public static readonly int SDK_CONFIRM_PURCHASE_SUCCESS = 0;

		public static readonly int SDK_CONFIRM_PURCHASE_FAILED = -501;

		public static readonly int SDK_CONFIRM_PURCHASE_ERROR = -502;

		public static readonly int SDK_SERVER_INVALID = -999;
	}
}
