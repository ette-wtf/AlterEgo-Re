using UnityEngine;

public class OnEventHandler : MonoBehaviour
{
	public void OnEvent()
	{
		base.gameObject.SendMessageUpwards("OnAnimationEvent", base.gameObject);
	}

	public void OnChanged(bool on)
	{
		base.gameObject.SendMessageUpwards("OnValueChanged", new string[2]
		{
			base.gameObject.name,
			on.ToString()
		}, SendMessageOptions.DontRequireReceiver);
	}
}
