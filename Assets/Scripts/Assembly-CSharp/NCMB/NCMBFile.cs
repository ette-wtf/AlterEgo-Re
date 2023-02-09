using System;
using System.Collections.Generic;
using MiniJSON;
using NCMB.Internal;

namespace NCMB
{
	[NCMBClassName("files")]
	public class NCMBFile : NCMBObject
	{
		public string FileName
		{
			get
			{
				object value = null;
				estimatedData.TryGetValue("fileName", out value);
				if (value == null)
				{
					return null;
				}
				return (string)this["fileName"];
			}
			set
			{
				this["fileName"] = value;
			}
		}

		public byte[] FileData
		{
			get
			{
				object value = null;
				estimatedData.TryGetValue("fileData", out value);
				if (value == null)
				{
					return null;
				}
				return (byte[])this["fileData"];
			}
			set
			{
				this["fileData"] = value;
			}
		}

		public NCMBFile()
			: this(null)
		{
		}

		public NCMBFile(string fileName)
			: this(fileName, null)
		{
		}

		public NCMBFile(string fileName, byte[] fileData)
			: this(fileName, fileData, null)
		{
		}

		public NCMBFile(string fileName, byte[] fileData, NCMBACL acl)
		{
			FileName = fileName;
			FileData = fileData;
			base.ACL = acl;
		}

		public override void SaveAsync(NCMBCallback callback)
		{
			if (FileName == null)
			{
				throw new NCMBException("fileName must not be null.");
			}
			ConnectType method = ((!base.CreateDate.HasValue) ? ConnectType.POST : ConnectType.PUT);
			IDictionary<string, INCMBFieldOperation> currentOperations = null;
			currentOperations = StartSave();
			string content = _toJSONObjectForSaving(currentOperations);
			new NCMBConnection(_getBaseUrl(), method, content, NCMBUser._getCurrentSessionToken(), this).Connect(delegate(int statusCode, string responseData, NCMBException error)
			{
				try
				{
					if (error != null)
					{
						_handleSaveResult(false, null, currentOperations);
					}
					else
					{
						Dictionary<string, object> responseDic = Json.Deserialize(responseData) as Dictionary<string, object>;
						_handleSaveResult(true, responseDic, currentOperations);
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

		public override void SaveAsync()
		{
			SaveAsync(null);
		}

		public void FetchAsync(NCMBGetFileCallback callback)
		{
			if (FileName == null)
			{
				throw new NCMBException("fileName must not be null.");
			}
			new NCMBConnection(_getBaseUrl(), ConnectType.GET, null, NCMBUser._getCurrentSessionToken(), this).Connect(delegate(int statusCode, byte[] responseData, NCMBException error)
			{
				estimatedData["fileData"] = responseData;
				if (callback != null)
				{
					callback(responseData, error);
				}
			});
		}

		public override void FetchAsync()
		{
			FetchAsync(null);
		}

		public static NCMBQuery<NCMBFile> GetQuery()
		{
			return NCMBQuery<NCMBFile>.GetQuery("file");
		}

		internal override string _getBaseUrl()
		{
			if (FileName != null)
			{
				return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/files/" + FileName;
			}
			return NCMBSettings.DomainURL + "/" + NCMBSettings.APIVersion + "/files";
		}
	}
}
