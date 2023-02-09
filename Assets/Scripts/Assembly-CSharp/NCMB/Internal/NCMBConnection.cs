using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MimeTypes;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace NCMB.Internal
{
	public class NCMBConnection
	{
		private static readonly string RESPONSE_SIGNATURE = "X-NCMB-Response-Signature";

		private static readonly string SIGNATURE_METHOD_KEY = "SignatureMethod";

		private static readonly string SIGNATURE_METHOD_VALUE = "HmacSHA256";

		private static readonly string SIGNATURE_VERSION_KEY = "SignatureVersion";

		private static readonly string SIGNATURE_VERSION_VALUE = "2";

		private static readonly string HEADER_SIGNATURE = "X-NCMB-Signature";

		private static readonly string HEADER_APPLICATION_KEY = "X-NCMB-Application-Key";

		private static readonly string HEADER_TIMESTAMP_KEY = "X-NCMB-Timestamp";

		private static readonly string HEADER_SESSION_TOKEN = "X-NCMB-Apps-Session-Token";

		private static readonly string HEADER_USER_AGENT_KEY = "X-NCMB-SDK-Version";

		private static readonly string HEADER_USER_AGENT_VALUE = "unity-" + CommonConstant.SDK_VERSION;

		private static readonly int REQUEST_TIME_OUT = 10;

		private string _applicationKey = "";

		private string _clientKey = "";

		private string _headerTimestamp = "";

		private ConnectType _method;

		private string _url = "";

		private string _content = "";

		private string _sessionToken = "";

		private Uri _domainUri;

		private NCMBFile _file;

		internal UnityWebRequest _request;

		internal NCMBConnection(string url, ConnectType method, string content, string sessionToken)
			: this(url, method, content, sessionToken, null, NCMBSettings.DomainURL)
		{
		}

		internal NCMBConnection(string url, ConnectType method, string content, string sessionToken, NCMBFile file)
			: this(url, method, content, sessionToken, file, NCMBSettings.DomainURL)
		{
		}

		internal NCMBConnection(string url, ConnectType method, string content, string sessionToken, NCMBFile file, string domain)
		{
			_method = method;
			_content = content;
			_url = url;
			_sessionToken = sessionToken;
			_applicationKey = NCMBSettings.ApplicationKey;
			_clientKey = NCMBSettings.ClientKey;
			_domainUri = new Uri(domain);
			_file = file;
			_request = _returnRequest();
		}

		internal void Connect(HttpClientFileDataCallback callback)
		{
			_Connection(callback);
		}

		internal void Connect(HttpClientCallback callback)
		{
			_Connection(callback);
		}

		private void _Connection(object callback)
		{
			GameObject.Find("NCMBSettings").GetComponent<NCMBSettings>().Connection(this, callback);
		}

		private void _signatureCheck(string responseSignature, string statusCode, string responseData, byte[] responseByte, ref NCMBException error)
		{
			StringBuilder stringBuilder = _makeSignatureHashData();
			if (responseByte.Any() && responseData != "")
			{
				stringBuilder.Append("\n" + responseData);
			}
			else if (responseByte.Any())
			{
				stringBuilder.Append("\n" + AsHex(responseByte));
			}
			string text = _makeSignature(stringBuilder.ToString());
			if (responseSignature != text)
			{
				statusCode = "100";
				responseData = "{}";
				error = new NCMBException();
				error.ErrorCode = "E100001";
				error.ErrorMessage = "Authentication error by response signature incorrect.";
			}
		}

		public static string AsHex(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				if (b < 16)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(Convert.ToString(b, 16));
			}
			return stringBuilder.ToString();
		}

		private UnityWebRequest _setUploadHandlerForFile(UnityWebRequest req)
		{
			string text = "\r\n";
			string text2 = "_NCMBBoundary";
			string text3 = "--" + text2 + text;
			byte[] bytes = Encoding.Default.GetBytes(text + "--" + text2 + "--");
			text3 = text3 + "Content-Disposition: form-data; name=\"file\"; filename=" + Uri.EscapeUriString(_file.FileName) + text;
			text3 = text3 + "Content-Type: " + MimeTypeMap.GetMimeType(Path.GetExtension(_file.FileName)) + text + text;
			byte[] first = Encoding.Default.GetBytes(text3);
			if (_file.FileData != null)
			{
				first = first.Concat(_file.FileData).ToArray();
			}
			if (_file.ACL != null && _file.ACL._toJSONObject().Count > 0)
			{
				string text4 = Json.Serialize(_file.ACL._toJSONObject());
				text3 = text + "--" + text2 + text;
				text3 = text3 + "Content-Disposition: form-data; name=acl; filename=acl" + text + text;
				text3 += text4;
				byte[] bytes2 = Encoding.Default.GetBytes(text3);
				first = first.Concat(bytes2).ToArray();
			}
			first = first.Concat(bytes).ToArray();
			req.uploadHandler = new UploadHandlerRaw(first);
			return req;
		}

		internal UnityWebRequest _returnRequest()
		{
			Uri uri = new Uri(_url);
			_url = uri.AbsoluteUri;
			string method = "";
			switch (_method)
			{
			case ConnectType.POST:
				method = "POST";
				break;
			case ConnectType.PUT:
				method = "PUT";
				break;
			case ConnectType.GET:
				method = "GET";
				break;
			case ConnectType.DELETE:
				method = "DELETE";
				break;
			}
			UnityWebRequest unityWebRequest = new UnityWebRequest(_url, method);
			_makeTimeStamp();
			StringBuilder stringBuilder = _makeSignatureHashData();
			string value = _makeSignature(stringBuilder.ToString());
			if ((unityWebRequest.method.Equals("POST") && _file != null) || (unityWebRequest.method.Equals("PUT") && _file != null))
			{
				unityWebRequest.SetRequestHeader("Content-Type", "multipart/form-data; boundary=_NCMBBoundary");
			}
			else
			{
				unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			}
			unityWebRequest.SetRequestHeader(HEADER_APPLICATION_KEY, _applicationKey);
			unityWebRequest.SetRequestHeader(HEADER_SIGNATURE, value);
			unityWebRequest.SetRequestHeader(HEADER_TIMESTAMP_KEY, _headerTimestamp);
			unityWebRequest.SetRequestHeader(HEADER_USER_AGENT_KEY, HEADER_USER_AGENT_VALUE);
			if (_sessionToken != null && _sessionToken != "")
			{
				unityWebRequest.SetRequestHeader(HEADER_SESSION_TOKEN, _sessionToken);
			}
			if (unityWebRequest.GetRequestHeader("Content-Type").Equals("multipart/form-data; boundary=_NCMBBoundary"))
			{
				_setUploadHandlerForFile(unityWebRequest);
			}
			else if ((unityWebRequest.method.Equals("POST") || unityWebRequest.method.Equals("PUT")) && _content != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(_content);
				unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
			}
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}

		private StringBuilder _makeSignatureHashData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = _url.Substring(_domainUri.OriginalString.Length);
			string[] array = text.Split('?');
			text = array[0];
			string text2 = null;
			if (array.Length > 1)
			{
				text2 = array[1];
			}
			Hashtable hashtable = new Hashtable();
			hashtable[SIGNATURE_METHOD_KEY] = SIGNATURE_METHOD_VALUE;
			hashtable[SIGNATURE_VERSION_KEY] = SIGNATURE_VERSION_VALUE;
			hashtable[HEADER_APPLICATION_KEY] = _applicationKey;
			hashtable[HEADER_TIMESTAMP_KEY] = _headerTimestamp;
			if (text2 != null && _method == ConnectType.GET)
			{
				string[] array2 = text2.Split('&');
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Split('=');
					hashtable[array3[0]] = array3[1];
				}
			}
			List<string> list = new List<string>();
			foreach (DictionaryEntry item in hashtable)
			{
				list.Add(item.Key.ToString());
			}
			StringComparer ordinal = StringComparer.Ordinal;
			list.Sort(ordinal);
			stringBuilder.Append(_method);
			stringBuilder.Append("\n");
			stringBuilder.Append(_domainUri.Host);
			stringBuilder.Append("\n");
			stringBuilder.Append(text);
			stringBuilder.Append("\n");
			foreach (string item2 in list)
			{
				stringBuilder.Append(string.Concat(item2, "=", hashtable[item2], "&"));
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder;
		}

		private string _makeSignature(string stringData)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(_clientKey);
			byte[] bytes2 = Encoding.UTF8.GetBytes(stringData);
			return Convert.ToBase64String(new HMACSHA256
			{
				Key = bytes
			}.ComputeHash(bytes2));
		}

		private void _makeTimeStamp()
		{
			string text = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
			_headerTimestamp = text.Replace(":", "%3A");
		}

		internal void _checkInvalidSessionToken(string code)
		{
			if (NCMBException.INCORRECT_HEADER.Equals(code) && _sessionToken != null && _sessionToken.Equals(NCMBUser._getCurrentSessionToken()))
			{
				NCMBUser._logOutEvent();
			}
		}

		internal void _checkResponseSignature(string code, string responseData, UnityWebRequest req, ref NCMBException error)
		{
			if (NCMBSettings._responseValidationFlag && req.error == null && error == null && req.GetResponseHeader(RESPONSE_SIGNATURE) != null)
			{
				string responseSignature = req.GetResponseHeader(RESPONSE_SIGNATURE).ToString();
				string text = responseData;
				if (text != null)
				{
					text = NCMBUtility.unicodeUnescape(text);
				}
				_signatureCheck(responseSignature, code, text, req.downloadHandler.data, ref error);
			}
		}

		internal static IEnumerator SendRequest(NCMBConnection connection, UnityWebRequest req, object callback)
		{
			NCMBException error = null;
			byte[] byteData = new byte[32768];
			string json = "";
			req.SendWebRequest();
			float elapsedTime = 0f;
			float waitTime = 0.2f;
			while (!req.isDone)
			{
				elapsedTime += waitTime;
				if (elapsedTime >= (float)REQUEST_TIME_OUT)
				{
					req.Abort();
					error = new NCMBException();
					break;
				}
				yield return new WaitForSeconds(waitTime);
			}
			if (error != null)
			{
				error.ErrorCode = "408";
				error.ErrorMessage = "Request Timeout.";
			}
			else if (req.isNetworkError)
			{
				error = new NCMBException
				{
					ErrorCode = req.responseCode.ToString(),
					ErrorMessage = req.error
				};
			}
			else if (req.responseCode != 200 && req.responseCode != 201)
			{
				error = new NCMBException();
				Dictionary<string, object> dictionary = Json.Deserialize(req.downloadHandler.text) as Dictionary<string, object>;
				error.ErrorCode = dictionary["code"].ToString();
				error.ErrorMessage = dictionary["error"].ToString();
			}
			else
			{
				byteData = req.downloadHandler.data;
				json = req.downloadHandler.text;
			}
			if (error != null)
			{
				connection._checkInvalidSessionToken(error.ErrorCode);
			}
			if (callback != null && !(callback is NCMBExecuteScriptCallback))
			{
				string code = req.responseCode.ToString();
				string responseData = req.downloadHandler.text;
				if (callback is HttpClientFileDataCallback)
				{
					responseData = "";
				}
				connection._checkResponseSignature(code, responseData, req, ref error);
			}
			if (callback != null)
			{
				if (callback is NCMBExecuteScriptCallback)
				{
					((NCMBExecuteScriptCallback)callback)(byteData, error);
				}
				else if (callback is HttpClientCallback)
				{
					((HttpClientCallback)callback)((int)req.responseCode, json, error);
				}
				else if (callback is HttpClientFileDataCallback)
				{
					((HttpClientFileDataCallback)callback)((int)req.responseCode, byteData, error);
				}
			}
		}
	}
}
