using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniJSON;
using NCMB.Internal;
using UnityEngine.Networking;

namespace NCMB
{
	[NCMBClassName("user")]
	public class NCMBUser : NCMBObject
	{
		private static NCMBUser _currentUser;

		internal bool _isCurrentUser;

		public string UserName
		{
			get
			{
				return (string)this["userName"];
			}
			set
			{
				this["userName"] = value;
			}
		}

		public string Email
		{
			get
			{
				return (string)this["mailAddress"];
			}
			set
			{
				this["mailAddress"] = value;
			}
		}

		public string Password
		{
			private get
			{
				return (string)this["password"];
			}
			set
			{
				lock (mutex)
				{
					this["password"] = value;
					base.IsDirty = true;
				}
			}
		}

		public Dictionary<string, object> AuthData
		{
			get
			{
				return (Dictionary<string, object>)this["authData"];
			}
			internal set
			{
				this["authData"] = value;
			}
		}

		public string SessionToken
		{
			get
			{
				return (string)this["sessionToken"];
			}
			internal set
			{
				this["sessionToken"] = value;
			}
		}

		public static NCMBUser CurrentUser
		{
			get
			{
				if (_currentUser != null)
				{
					return _currentUser;
				}
				NCMBUser nCMBUser = null;
				nCMBUser = (NCMBUser)NCMBObject._getFromVariable();
				if (nCMBUser == null)
				{
					nCMBUser = (NCMBUser)NCMBObject._getFromDisk("currentUser");
				}
				if (nCMBUser != null)
				{
					_currentUser = nCMBUser;
					_currentUser._isCurrentUser = true;
					return _currentUser;
				}
				return null;
			}
		}

		public NCMBUser()
		{
			_isCurrentUser = false;
		}

		internal override void _onSettingValue(string key, object value)
		{
			base._onSettingValue(key, value);
		}

		public override void Add(string key, object value)
		{
			if ("userName".Equals(key))
			{
				throw new NCMBException("userName key is already exist. Use this.UserName to set it");
			}
			if ("password".Equals(key))
			{
				throw new NCMBException("password key is already exist. Use this.Password to set it");
			}
			if ("mailAddress".Equals(key))
			{
				throw new NCMBException("mailAdress key is already exist. Use this.Email to set it");
			}
			base.Add(key, value);
		}

		public override void Remove(string key)
		{
			if ("userName".Equals(key))
			{
				throw new NCMBException("Can not remove the userName key");
			}
			if ("password".Equals(key))
			{
				throw new NCMBException("Can not remove the Password key");
			}
			base.Remove(key);
		}

		public static NCMBQuery<NCMBUser> GetQuery()
		{
			return NCMBQuery<NCMBUser>.GetQuery("user");
		}

		internal override string _getBaseUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/users";
		}

		internal static string _getLogInUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/login";
		}

		internal static string _getLogOutUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/logout";
		}

		internal static string _getRequestPasswordResetUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/requestPasswordReset";
		}

		private static string _getmailAddressUserEntryUrl()
		{
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/requestMailAddressUserEntry";
		}

		internal override void _afterSave(int statusCode, NCMBException error)
		{
			if ((statusCode == 201 || statusCode == 200) && error == null)
			{
				_saveCurrentUser(this);
			}
		}

		internal override void _afterDelete(NCMBException error)
		{
			if (error == null)
			{
				_logOutEvent();
			}
		}

		public override void DeleteAsync()
		{
			DeleteAsync(null);
		}

		public override void DeleteAsync(NCMBCallback callback)
		{
			base.DeleteAsync(callback);
		}

		public void SignUpAsync(NCMBCallback callback)
		{
			base.SaveAsync(callback);
		}

		public void SignUpAsync()
		{
			SignUpAsync(null);
		}

		public override void SaveAsync()
		{
			SaveAsync(null);
		}

		public override void SaveAsync(NCMBCallback callback)
		{
			base.SaveAsync(callback);
		}

		internal static void _saveCurrentUser(NCMBUser user)
		{
			if (_currentUser != null)
			{
				lock (_currentUser.mutex)
				{
					if (_currentUser != null && _currentUser != user)
					{
						_logOutEvent();
					}
				}
			}
			lock (user.mutex)
			{
				user._isCurrentUser = true;
				user._saveToDisk("currentUser");
				user._saveToVariable();
				_currentUser = user;
			}
		}

		internal static void _logOutEvent()
		{
			string path = NCMBSettings.filePath + "/currentUser";
			if (_currentUser != null)
			{
				_currentUser._isCurrentUser = false;
			}
			_currentUser = null;
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				NCMBSettings.CurrentUser = "";
			}
			catch (Exception error)
			{
				throw new NCMBException(error);
			}
		}

		internal static string _getCurrentSessionToken()
		{
			if (CurrentUser != null)
			{
				return CurrentUser.SessionToken;
			}
			return "";
		}

		public bool IsAuthenticated()
		{
			if (SessionToken != null && CurrentUser != null)
			{
				return CurrentUser.ObjectId.Equals(base.ObjectId);
			}
			return false;
		}

		public static void RequestPasswordResetAsync(string email)
		{
			RequestPasswordResetAsync(email, null);
		}

		public static void RequestPasswordResetAsync(string email, NCMBCallback callback)
		{
			_requestPasswordReset(email, callback);
		}

		internal static void _requestPasswordReset(string email, NCMBCallback callback)
		{
			string url = _getRequestPasswordResetUrl();
			ConnectType method = ConnectType.POST;
			NCMBUser obj = new NCMBUser
			{
				Email = email
			};
			string content = obj._toJSONObjectForSaving(obj.StartSave());
			new NCMBConnection(url, method, content, _getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public static void LogInAsync(string name, string password)
		{
			LogInAsync(name, password, null);
		}

		public static void LogInAsync(string name, string password, NCMBCallback callback)
		{
			_ncmbLogIn(name, password, null, callback);
		}

		private static void _ncmbLogIn(string name, string password, string email, NCMBCallback callback)
		{
			string text = _getLogInUrl();
			ConnectType method = ConnectType.GET;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["password"] = password;
			if (name != null)
			{
				dictionary["userName"] = name;
			}
			else
			{
				if (email == null)
				{
					throw new NCMBException(new ArgumentException("UserName or Email can not be null."));
				}
				dictionary["mailAddress"] = email;
			}
			new NCMBConnection(_makeParamUrl(text + "?", dictionary), method, null, null).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error == null)
					{
						Dictionary<string, object> responseDic = Json.Deserialize(responseData) as Dictionary<string, object>;
						NCMBUser nCMBUser = new NCMBUser();
						nCMBUser._handleFetchResult(true, responseDic);
						_saveCurrentUser(nCMBUser);
					}
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		private static string _makeParamUrl(string url, Dictionary<string, object> parameter)
		{
			string text = url;
			foreach (KeyValuePair<string, object> item in parameter)
			{
				text = text + item.Key + "=" + UnityWebRequest.EscapeURL((string)item.Value) + "&";
			}
			if (parameter.Count > 0)
			{
				text = text.Remove(text.Length - 1);
			}
			return text;
		}

		public static void LogInWithMailAddressAsync(string email, string password, NCMBCallback callback)
		{
			_ncmbLogIn(null, password, email, callback);
		}

		public static void LogInWithMailAddressAsync(string email, string password)
		{
			_ncmbLogIn(null, password, email, null);
		}

		public static void RequestAuthenticationMailAsync(string email)
		{
			RequestAuthenticationMailAsync(email, null);
		}

		public static void RequestAuthenticationMailAsync(string email, NCMBCallback callback)
		{
			string url = _getmailAddressUserEntryUrl();
			NCMBUser obj = new NCMBUser
			{
				Email = email
			};
			string content = obj._toJSONObjectForSaving(obj.StartSave());
			ConnectType method = ConnectType.POST;
			new NCMBConnection(url, method, content, _getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public static void LogOutAsync()
		{
			LogOutAsync(null);
		}

		public static void LogOutAsync(NCMBCallback callback)
		{
			if (_currentUser != null)
			{
				_logOut(callback);
				return;
			}
			try
			{
				_logOutEvent();
			}
			catch (NCMBException error)
			{
				if (callback != null)
				{
					callback(error);
				}
				return;
			}
			if (callback != null)
			{
				callback(null);
			}
		}

		internal static void _logOut(NCMBCallback callback)
		{
			string url = _getLogOutUrl();
			ConnectType method = ConnectType.GET;
			string content = null;
			new NCMBConnection(url, method, content, _getCurrentSessionToken()).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error == null)
					{
						_logOutEvent();
					}
				}
				catch (Exception error2)
				{
					error = new NCMBException(error2);
				}
				if (callback != null)
				{
					if (error != null && NCMBException.INCORRECT_HEADER.Equals(error.ErrorCode))
					{
						callback(null);
					}
					else
					{
						callback(error);
					}
				}
			});
		}

		internal override void _mergeFromServer(Dictionary<string, object> responseDic, bool completeData)
		{
			base._mergeFromServer(responseDic, completeData);
		}

		public void LogInWithAuthDataAsync(NCMBCallback callback)
		{
			if (AuthData == null)
			{
				throw new NCMBException(new ArgumentException("Post authData not exist"));
			}
			SignUpAsync(delegate(NCMBException error)
			{
				if (error != null)
				{
					AuthData.Clear();
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public void LogInWithAuthDataAsync()
		{
			LogInWithAuthDataAsync(null);
		}

		public void LinkWithAuthDataAsync(Dictionary<string, object> linkParam, NCMBCallback callback)
		{
			if (AuthData == null)
			{
				AuthData = linkParam;
				LogInWithAuthDataAsync(callback);
			}
			Dictionary<string, object> currentParam = new Dictionary<string, object>();
			currentParam = AuthData;
			AuthData = linkParam;
			SignUpAsync(delegate(NCMBException error)
			{
				if (error == null)
				{
					Dictionary<string, object> authData = linkParam.Concat(currentParam).ToDictionary((KeyValuePair<string, object> x) => x.Key, (KeyValuePair<string, object> x) => x.Value);
					AuthData = authData;
				}
				else
				{
					AuthData = currentParam;
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public void LinkWithAuthDataAsync(Dictionary<string, object> linkParam)
		{
			LinkWithAuthDataAsync(linkParam, null);
		}

		public void UnLinkWithAuthDataAsync(string provider, NCMBCallback callback)
		{
			if (AuthData == null)
			{
				throw new NCMBException(new ArgumentException("Current User authData not exist"));
			}
			List<string> list = new List<string> { "facebook", "twitter" };
			if (string.IsNullOrEmpty(provider) || !list.Contains(provider))
			{
				throw new NCMBException(new ArgumentException("Provider must be facebook or twitter"));
			}
			Dictionary<string, object> currentParam = new Dictionary<string, object>();
			currentParam = AuthData;
			Dictionary<string, object> authData = new Dictionary<string, object> { { provider, null } };
			AuthData = authData;
			SignUpAsync(delegate(NCMBException error)
			{
				if (error == null)
				{
					currentParam.Remove(provider);
					AuthData = currentParam;
				}
				else
				{
					AuthData = currentParam;
				}
				if (callback != null)
				{
					callback(error);
				}
			});
		}

		public void UnLinkWithAuthDataAsync(string provider)
		{
			UnLinkWithAuthDataAsync(provider, null);
		}

		public bool IsLinkWith(string provider)
		{
			List<string> list = new List<string> { "facebook", "twitter" };
			if (string.IsNullOrEmpty(provider) || !list.Contains(provider))
			{
				throw new NCMBException(new ArgumentException("Provider must be facebook or twitter"));
			}
			if (AuthData == null)
			{
				return false;
			}
			return AuthData.ContainsKey(provider);
		}

		public Dictionary<string, object> GetAuthDataForProvider(string provider)
		{
			List<string> list = new List<string> { "facebook", "twitter" };
			if (string.IsNullOrEmpty(provider) || !list.Contains(provider))
			{
				throw new NCMBException(new ArgumentException("Provider must be facebook or twitter"));
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (!(provider == "facebook"))
			{
				if (provider == "twitter")
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)((Dictionary<string, object>)this["authData"])["twitter"];
					dictionary.Add("id", dictionary2["id"]);
					dictionary.Add("screen_name", dictionary2["screen_name"]);
					dictionary.Add("oauth_consumer_key", dictionary2["oauth_consumer_key"]);
					dictionary.Add("consumer_secret", dictionary2["consumer_secret"]);
					dictionary.Add("oauth_token", dictionary2["oauth_token"]);
					dictionary.Add("oauth_token_secret", dictionary2["oauth_token_secret"]);
				}
			}
			else
			{
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)((Dictionary<string, object>)this["authData"])["facebook"];
				dictionary.Add("id", dictionary3["id"]);
				dictionary.Add("access_token", dictionary3["access_token"]);
				dictionary.Add("expiration_date", NCMBUtility.encodeDate((DateTime)dictionary3["expiration_date"]));
			}
			return dictionary;
		}
	}
}
