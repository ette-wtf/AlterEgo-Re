using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MiniJSON;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("installation")]
	public class NCMBInstallation : NCMBObject
	{
		public string ApplicationName
		{
			get
			{
				return (string)this["applicationName"];
			}
			internal set
			{
				this["applicationName"] = value;
			}
		}

		public string AppVersion
		{
			get
			{
				return (string)this["appVersion"];
			}
			internal set
			{
				this["appVersion"] = value;
			}
		}

		public string DeviceToken
		{
			set
			{
				this["deviceToken"] = value;
			}
		}

		public string DeviceType
		{
			get
			{
				return (string)this["deviceType"];
			}
			internal set
			{
				this["deviceType"] = value;
			}
		}

		public string SdkVersion
		{
			get
			{
				return (string)this["sdkVersion"];
			}
			internal set
			{
				this["sdkVersion"] = value;
			}
		}

		public string TimeZone
		{
			get
			{
				return (string)this["timeZone"];
			}
			internal set
			{
				this["timeZone"] = value;
			}
		}

		private void setDefaultProperty()
		{
			IDictionary<string, object> installationDefaultProperty = NCMBManager.installationDefaultProperty;
			object value;
			if (installationDefaultProperty.TryGetValue("applicationName", out value))
			{
				ApplicationName = (string)value;
			}
			if (installationDefaultProperty.TryGetValue("appVersion", out value))
			{
				AppVersion = (string)value;
			}
			if (installationDefaultProperty.TryGetValue("deviceType", out value))
			{
				DeviceType = (string)value;
			}
			if (installationDefaultProperty.TryGetValue("timeZone", out value))
			{
				TimeZone = (string)value;
			}
			this["pushType"] = "fcm";
			SdkVersion = CommonConstant.SDK_VERSION;
		}

		public NCMBInstallation()
			: this("")
		{
		}

		internal NCMBInstallation(string jsonText)
		{
			if (jsonText != null && jsonText != "")
			{
				Dictionary<string, object> dictionary = Json.Deserialize(jsonText) as Dictionary<string, object>;
				object value;
				if (dictionary.TryGetValue("data", out value))
				{
					dictionary = (Dictionary<string, object>)value;
				}
				_mergeFromServer(dictionary, false);
			}
			DeviceToken = NCMBManager._token;
			setDefaultProperty();
		}

		public void GetDeviceToken(NCMBGetCallback<string> callback)
		{
			if (ContainsKey("deviceToken") && this["deviceToken"] != null)
			{
				callback((string)this["deviceToken"], null);
				return;
			}
			new Thread((ThreadStart)delegate
			{
				for (int i = 0; i < 10; i++)
				{
					if (NCMBManager._token != null)
					{
						this["deviceToken"] = NCMBManager._token;
						break;
					}
					Thread.Sleep(500);
				}
				if (callback != null)
				{
					if (ContainsKey("deviceToken") && this["deviceToken"] != null)
					{
						callback((string)this["deviceToken"], null);
					}
					else
					{
						callback(null, new NCMBException("Can not get device token"));
					}
				}
			}).Start();
		}

		public static NCMBInstallation getCurrentInstallation()
		{
			NCMBInstallation nCMBInstallation = null;
			try
			{
				string currentInstallation = NCMBManager.GetCurrentInstallation();
				if (currentInstallation != "")
				{
					return new NCMBInstallation(currentInstallation);
				}
				return new NCMBInstallation();
			}
			catch (SystemException error)
			{
				throw new NCMBException(error);
			}
		}

		public static NCMBQuery<NCMBInstallation> GetQuery()
		{
			return NCMBQuery<NCMBInstallation>.GetQuery("installation");
		}

		internal override string _getBaseUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/installations";
		}

		internal override void _afterSave(int statusCode, NCMBException error)
		{
			if (error != null)
			{
				if (error.ErrorCode == "E404001")
				{
					NCMBManager.DeleteCurrentInstallation(NCMBManager.SearchPath());
				}
			}
			else if (statusCode == 201 || statusCode == 200)
			{
				string text = NCMBManager.SearchPath();
				if (text != NCMBSettings.currentInstallationPath)
				{
					NCMBManager.DeleteCurrentInstallation(text);
				}
				_saveInstallationToDisk("currentInstallation");
			}
		}

		internal void _saveInstallationToDisk(string fileName)
		{
			string path = NCMBSettings.filePath + "/" + fileName;
			lock (mutex)
			{
				try
				{
					string value = _toJsonDataForDataFile();
					using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
					{
						streamWriter.Write(value);
						streamWriter.Close();
					}
				}
				catch (Exception error)
				{
					throw new NCMBException(error);
				}
			}
		}
	}
}
