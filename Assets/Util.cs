
using UnityEngine;

public static class Util
{
	public static Vector3 DiscardYFromPosition(Vector3 mousePosition, float replacement=0)
	{
		Vector3 worldPosition = new Vector3 ();
		worldPosition.x = mousePosition.x;
		worldPosition.y = replacement;
		worldPosition.z = mousePosition.z;
		return worldPosition;
		
	}
}