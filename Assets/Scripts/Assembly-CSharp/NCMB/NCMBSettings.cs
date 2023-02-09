using NCMB.Internal;
using UnityEngine;

namespace NCMB
{
	public class NCMBSettings : MonoBehaviour
	{
		private static string _applicationKey = "";

		private static string _clientKey = "";

		internal static bool _responseValidationFlag = false;

		internal static bool _isInitialized = false;

		private static bool _usePush = false;

		private static bool _useAnalytics = false;

		private static string _domainURL = "";

		private static string _apiVersion = "";

		internal string applicationKey = "a416aff3f1a784b55ff9638f243d78dc962b1da30b8eabf072d1cf0c7009e5cc";

		internal string clientKey = "deb34de45084b86952d73d2a999e8ac291a0bef464804a3ee43b0edb4bb47722";

		[SerializeField]
		internal bool usePush;

		[SerializeField]
		internal bool useAnalytics;

		[SerializeField]
		internal bool responseValidation;

		internal string domainURL = "";

		internal string apiVersion = "";

		private static string _currentUser = null;

		internal static string filePath = "";

		internal static string currentInstallationPath = "";

		internal static string CurrentUser
		{
			get
			{
				return _currentUser;
			}
			set
			{
				_currentUser = value;
			}
		}

		public static string ApplicationKey
		{
			get
			{
				return _applicationKey;
			}
			set
			{
				_applicationKey = value;
			}
		}

		public static string ClientKey
		{
			get
			{
				return _clientKey;
			}
			set
			{
				_clientKey = value;
			}
		}

		public static bool UsePush
		{
			get
			{
				return _usePush;
			}
		}

		public static bool UseAnalytics
		{
			get
			{
				return _useAnalytics;
			}
		}

		internal static string DomainURL
		{
			get
			{
				return _domainURL;
			}
			set
			{
				_domainURL = value;
			}
		}

		internal static string APIVersion
		{
			get
			{
				return _apiVersion;
			}
			set
			{
				_apiVersion = value;
			}
		}

		public static void Initialize(string applicationKey, string clientKey, string domainURL, string apiVersion)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_domainURL = (string.IsNullOrEmpty(domainURL) ? CommonConstant.DOMAIN_URL : domainURL);
			_apiVersion = (string.IsNullOrEmpty(apiVersion) ? CommonConstant.API_VERSION : apiVersion);
		}

		private static void RegisterPush(bool usePush, bool useAnalytics, bool getLocation = false)
		{
			_usePush = usePush;
			_useAnalytics = useAnalytics;
			if (usePush)
			{
				NCMBManager.CreateInstallationProperty();
				if (!getLocation)
				{
					NCMBPush.Register();
				}
				else
				{
					NCMBPush.RegisterWithLocation();
				}
			}
		}

		public static void EnableResponseValidation(bool checkFlag)
		{
			_responseValidationFlag = checkFlag;
		}

		public virtual void Awake()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				_responseValidationFlag = responseValidation;
				Object.DontDestroyOnLoad(base.gameObject);
				Initialize(applicationKey, clientKey, domainURL, apiVersion);
				filePath = Application.persistentDataPath;
				currentInstallationPath = filePath + "/currentInstallation";
				RegisterPush(usePush, useAnalytics);
			}
		}

		internal void Connection(NCMBConnection connection, object callback)
		{
			StartCoroutine(NCMBConnection.SendRequest(connection, connection._request, callback));
		}
	}
}
