using UnityEngine;

public class EffectController : MonoBehaviour
{
	public bool SoundOn;

	public void OnEndAnimation()
	{
		Object.Destroy(base.transform.parent.gameObject);
	}

	public void PlaySound(string soundName)
	{
		if (SoundOn)
		{
			AudioManager.PlaySound("SE", base.gameObject.name, soundName);
		}
	}
}
