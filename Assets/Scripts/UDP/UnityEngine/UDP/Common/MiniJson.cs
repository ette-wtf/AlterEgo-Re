using System.Collections.Generic;
using UnityEngine.UDP.Common.MiniJSON;

namespace UnityEngine.UDP.Common
{
	public class MiniJson
	{
		public static string JsonEncode(object json)
		{
			return Json.Serialize(json);
		}

		public static Dictionary<string, object> JsonDecode(string json)
		{
			return (Dictionary<string, object>)Json.Deserialize(json);
		}
	}
}
