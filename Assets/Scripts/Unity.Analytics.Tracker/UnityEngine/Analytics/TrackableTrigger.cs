using System;

namespace UnityEngine.Analytics
{
	[Serializable]
	public class TrackableTrigger
	{
		[SerializeField]
		private GameObject m_Target;

		[SerializeField]
		private string m_MethodPath;
	}
}
