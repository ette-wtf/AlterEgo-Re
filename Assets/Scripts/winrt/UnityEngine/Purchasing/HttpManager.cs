using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace UnityEngine.Purchasing
{
	public class HttpManager
	{
		public void GetAsyncHttpWebData(string httpAddress, Action<IAsyncResult> asyncCompleate, Action<object, bool> asyncTimeout, string param = null, string requestMothod = "GET", int timeout = 5000)
		{
			ServicePointManager.ServerCertificateValidationCallback = VerifyServerCertificate;
			HttpWebRequest httpWebRequest = WebRequest.Create(httpAddress + ((param == null) ? "" : param)) as HttpWebRequest;
			httpWebRequest.Method = requestMothod;
			httpWebRequest.Timeout = timeout;
			ThreadPool.RegisterWaitForSingleObject(httpWebRequest.BeginGetResponse(asyncCompleate.Invoke, httpWebRequest).AsyncWaitHandle, asyncTimeout.Invoke, httpWebRequest, timeout, true);
		}

		private bool VerifyServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}
