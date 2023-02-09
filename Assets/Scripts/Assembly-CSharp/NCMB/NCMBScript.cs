using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MiniJSON;
using NCMB.Internal;
using SimpleJSON;
using UnityEngine;

namespace NCMB
{
	public class NCMBScript
	{
		public enum MethodType
		{
			POST = 0,
			PUT = 1,
			GET = 2,
			DELETE = 3
		}

		private static readonly string SERVICE_PATH = "script";

		private static readonly string DEFAULT_SCRIPT_ENDPOINT = "https://script.mbaas.api.nifcloud.com";

		private static readonly string DEFAULT_SCRIPT_API_VERSION = "2015-09-01";

		private string _scriptName;

		private MethodType _method;

		private string _baseUrl;

		public string ScriptName
		{
			get
			{
				return _scriptName;
			}
			set
			{
				_scriptName = value;
			}
		}

		public MethodType Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}

		public string BaseUrl
		{
			get
			{
				return _baseUrl;
			}
			set
			{
				_baseUrl = value;
			}
		}

		public NCMBScript(string scriptName, MethodType method)
			: this(scriptName, method, DEFAULT_SCRIPT_ENDPOINT)
		{
		}

		public NCMBScript(string scriptName, MethodType method, string baseUrl)
		{
			_scriptName = scriptName;
			_method = method;
			_baseUrl = baseUrl;
		}

		public void ExecuteAsync(IDictionary<string, object> header, IDictionary<string, object> body, IDictionary<string, object> query, NCMBExecuteScriptCallback callback)
		{
			string domain = DEFAULT_SCRIPT_ENDPOINT;
			string text = DEFAULT_SCRIPT_ENDPOINT + "/" + DEFAULT_SCRIPT_API_VERSION + "/" + SERVICE_PATH + "/" + _scriptName;
			if (_baseUrl == null || _baseUrl.Length == 0)
			{
				throw new ArgumentException("Invalid baseUrl.");
			}
			if (!_baseUrl.Equals(DEFAULT_SCRIPT_ENDPOINT))
			{
				domain = _baseUrl;
				text = _baseUrl + "/" + _scriptName;
			}
			ConnectType method;
			switch (_method)
			{
			case MethodType.POST:
				method = ConnectType.POST;
				break;
			case MethodType.PUT:
				method = ConnectType.PUT;
				break;
			case MethodType.GET:
				method = ConnectType.GET;
				break;
			case MethodType.DELETE:
				method = ConnectType.DELETE;
				break;
			default:
				throw new ArgumentException("Invalid methodType.");
			}
			string content = null;
			if (body != null)
			{
				content = MiniJSON.Json.Serialize(body);
			}
			string text2 = "";
			string text3 = "?";
			if (query != null && query.Count > 0)
			{
				int num = query.Count;
				foreach (KeyValuePair<string, object> item in query)
				{
					text3 = string.Concat(str3: Uri.EscapeDataString((item.Value is IList || item.Value is IDictionary) ? SimpleJSON.Json.Serialize(item.Value) : ((!(item.Value is DateTime)) ? item.Value.ToString() : NCMBUtility.encodeDate((DateTime)item.Value))), str0: text3, str1: item.Key, str2: "=");
					if (num > 1)
					{
						text3 += "&";
						num--;
					}
				}
				text += text3;
			}
			ServicePointManager.ServerCertificateValidationCallback = (object _003Cp0_003E, X509Certificate _003Cp1_003E, X509Chain _003Cp2_003E, SslPolicyErrors _003Cp3_003E) => true;
			NCMBConnection nCMBConnection = new NCMBConnection(text, method, content, NCMBUser._getCurrentSessionToken(), null, domain);
			if (header != null && header.Count > 0)
			{
				foreach (KeyValuePair<string, object> item2 in header)
				{
					nCMBConnection._request.SetRequestHeader(item2.Key, item2.Value.ToString());
				}
			}
			Connect(nCMBConnection, callback);
		}

		internal void Connect(NCMBConnection connection, NCMBExecuteScriptCallback callback)
		{
			GameObject.Find("NCMBSettings").GetComponent<NCMBSettings>().Connection(connection, callback);
		}
	}
}
