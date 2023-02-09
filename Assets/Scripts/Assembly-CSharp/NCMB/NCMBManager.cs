using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiniJSON;
using UnityEngine;

namespace NCMB
{
	public class NCMBManager : MonoBehaviour
	{
		public delegate void OnRegistrationDelegate(string errorMessage);

		public delegate void OnNotificationReceivedDelegate(NCMBPushPayload payload);

		private const string NS = "NCMB_SPLITTER";

		internal static string _token;

		internal static IDictionary<string, object> installationDefaultProperty = new Dictionary<string, object>();

		public static OnRegistrationDelegate onRegistration;

		public static OnNotificationReceivedDelegate onNotificationReceived;

		internal static bool Inited { get; set; }

		public virtual void Awake()
		{
			if (!NCMBSettings._isInitialized)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private void OnRegistration(string message)
		{
			Inited = true;
			if (onRegistration != null)
			{
				if (message == "")
				{
					message = null;
				}
				onRegistration(message);
			}
		}

		private void OnNotificationReceived(string message)
		{
			if (onNotificationReceived != null)
			{
				string[] array = message.Split(new string[1] { "NCMB_SPLITTER" }, StringSplitOptions.None);
				NCMBPushPayload payload = new NCMBPushPayload(array[0], array[1], array[2], array[3], array[4], array[5], array[6]);
				onNotificationReceived(payload);
			}
		}

		internal static string SearchPath()
		{
			try
			{
				string currentInstallationPath = NCMBSettings.currentInstallationPath;
				currentInstallationPath = NCMBSettings.filePath;
				currentInstallationPath = currentInstallationPath.Replace("files", "");
				currentInstallationPath += "app_NCMB/currentInstallation";
				if (!File.Exists(currentInstallationPath))
				{
					currentInstallationPath = NCMBSettings.currentInstallationPath;
				}
				return currentInstallationPath;
			}
			catch (FileNotFoundException ex)
			{
				throw ex;
			}
		}

		internal void onTokenReceived(string token)
		{
			_token = token;
			string path = SearchPath();
			string text = "";
			NCMBInstallation installation = null;
			if ((text = ReadFile(path)) != "")
			{
				installation = new NCMBInstallation(text);
			}
			else
			{
				installation = new NCMBInstallation();
			}
			installation.DeviceToken = _token;
			installation.SaveAsync(delegate(NCMBException saveError)
			{
				if (saveError != null)
				{
					if (saveError.ErrorCode.Equals(NCMBException.DUPPLICATION_ERROR))
					{
						updateExistedInstallation(installation, path);
					}
					else if (saveError.ErrorCode.Equals(NCMBException.DATA_NOT_FOUND))
					{
						installation.ObjectId = null;
						installation.SaveAsync(delegate(NCMBException updateError)
						{
							if (updateError != null)
							{
								OnRegistration(updateError.ErrorMessage);
							}
							else
							{
								OnRegistration("");
							}
						});
					}
					else
					{
						OnRegistration(saveError.ErrorMessage);
					}
				}
				else
				{
					OnRegistration("");
				}
			});
		}

		private void updateExistedInstallation(NCMBInstallation installation, string path)
		{
			NCMBQuery<NCMBInstallation> query = NCMBInstallation.GetQuery();
			installation.GetDeviceToken(delegate(string token, NCMBException error)
			{
				query.WhereEqualTo("deviceToken", token);
				query.FindAsync(delegate(List<NCMBInstallation> objList, NCMBException findError)
				{
					if (findError != null)
					{
						OnRegistration(findError.ErrorMessage);
					}
					else if (objList.Count != 0)
					{
						installation.ObjectId = objList[0].ObjectId;
						installation.SaveAsync(delegate(NCMBException installationUpdateError)
						{
							if (installationUpdateError != null)
							{
								OnRegistration(installationUpdateError.ErrorMessage);
							}
							else
							{
								OnRegistration("");
							}
						});
					}
				});
			});
		}

		private void SaveFile(string path, string text)
		{
			try
			{
				Encoding encoding = Encoding.GetEncoding("UTF-8");
				StreamWriter streamWriter = new StreamWriter(path, false, encoding);
				streamWriter.WriteLine(text);
				streamWriter.Close();
			}
			catch (Exception ex)
			{
				if (ex != null)
				{
					path = NCMBSettings.currentInstallationPath;
					try
					{
						Encoding encoding2 = Encoding.GetEncoding("UTF-8");
						StreamWriter streamWriter2 = new StreamWriter(path, false, encoding2);
						streamWriter2.WriteLine(text);
						streamWriter2.Close();
						return;
					}
					catch (IOException ex2)
					{
						throw new IOException("File save error" + ex2.Message);
					}
				}
			}
		}

		private static string ReadFile(string path)
		{
			string result = "";
			if (File.Exists(path))
			{
				try
				{
					StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
					result = streamReader.ReadToEnd();
					streamReader.Close();
					return result;
				}
				catch (Exception ex)
				{
					if (ex != null)
					{
						path = NCMBSettings.currentInstallationPath;
						try
						{
							StreamReader streamReader2 = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
							result = streamReader2.ReadToEnd();
							streamReader2.Close();
							return result;
						}
						catch (FileNotFoundException ex2)
						{
							throw ex2;
						}
					}
					return result;
				}
			}
			return result;
		}

		private void onAnalyticsReceived(string _pushId)
		{
			NCMBAnalytics.TrackAppOpened(_pushId);
		}

		internal static void DeleteCurrentInstallation(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch (IOException innerException)
			{
				throw new IOException("Delete currentInstallation failed.", innerException);
			}
		}

		internal static string GetCurrentInstallation()
		{
			return ReadFile(SearchPath());
		}

		internal static void CreateInstallationProperty()
		{
			string text = null;
			text = new AndroidJavaClass("com.nifcloud.mbaas.ncmbfcmplugin.FCMInit").CallStatic<string>("getInstallationProperty", Array.Empty<object>());
			if (text != null)
			{
				installationDefaultProperty = Json.Deserialize(text) as Dictionary<string, object>;
			}
		}
	}
}
