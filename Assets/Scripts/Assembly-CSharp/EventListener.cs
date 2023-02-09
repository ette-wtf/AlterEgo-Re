using UnityEngine;

public class EventListener : MonoBehaviour
{
	public virtual bool OnEvent(string[] eventList)
	{
		return false;
	}
}
