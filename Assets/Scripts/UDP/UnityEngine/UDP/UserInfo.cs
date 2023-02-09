using UnityEngine.Scripting;

namespace UnityEngine.UDP
{
	public class UserInfo
	{
		[Preserve]
		public string Channel { get; set; }

		[Preserve]
		public string UserId { get; set; }

		[Preserve]
		public string UserLoginToken { get; set; }
	}
}
