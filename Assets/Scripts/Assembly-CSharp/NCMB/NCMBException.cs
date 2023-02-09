using System;

namespace NCMB
{
	public class NCMBException : Exception
	{
		private string _errorCode;

		private string _errorMessage;

		public static readonly string BAD_REQUEST = "E400000";

		public static readonly string INVALID_JSON = "E400001";

		public static readonly string INVALID_TYPE = "E400002";

		public static readonly string REQUIRED = "E400003";

		public static readonly string INVALID_FORMAT = "E400004";

		public static readonly string INVALID_VALUE = "E400005";

		public static readonly string NOT_EXIST = "E400006";

		public static readonly string RELATION_ERROR = "E400008";

		public static readonly string INVALID_SIZE = "E400009";

		public static readonly string INCORRECT_HEADER = "E401001";

		public static readonly string INCORRECT_PASSWORD = "E401002";

		public static readonly string OAUTH_ERROR = "E401003";

		public static readonly string INVALID_ACL = "E403001";

		public static readonly string INVALID_OPERATION = "E403002";

		public static readonly string FORBIDDEN_OPERATION = "E403003";

		public static readonly string INVALID_SETTING = "E403005";

		public static readonly string INVALID_GEOPOINT = "E403006";

		public static readonly string INVALID_METHOD = "E405001";

		public static readonly string DUPPLICATION_ERROR = "E409001";

		public static readonly string FILE_SIZE_ERROR = "E413001";

		public static readonly string DOCUMENT_SIZE_ERROR = "E413002";

		public static readonly string REQUEST_LIMIT_ERROR = "E413003";

		public static readonly string UNSUPPORT_MEDIA = "E415001";

		public static readonly string REQUEST_OVERLOAD = "E429001";

		public static readonly string SYSTEM_ERROR = "E500001";

		public static readonly string STORAGE_ERROR = "E502001";

		public static readonly string MAIL_ERROR = "E502002";

		public static readonly string DATABASE_ERROR = "E502003";

		public static readonly string DATA_NOT_FOUND = "E404001";

		public string ErrorCode
		{
			get
			{
				return _errorCode;
			}
			set
			{
				_errorCode = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				if (_errorMessage != null && _errorMessage != "")
				{
					return _errorMessage;
				}
				return Message;
			}
			set
			{
				_errorMessage = value;
			}
		}

		public override string Message
		{
			get
			{
				return _errorMessage;
			}
		}

		public NCMBException()
		{
			_errorCode = "";
			ErrorMessage = "";
		}

		public NCMBException(Exception error)
		{
			_errorCode = "";
			ErrorMessage = error.Message;
			Debug.Log(string.Concat("Error occurred: ", error.Message, " \n with: ", error.Data, " ; \n ", error.StackTrace));
		}

		public NCMBException(string message)
		{
			_errorCode = "";
			ErrorMessage = message;
			Debug.Log("Error occurred: " + message);
		}
	}
}
