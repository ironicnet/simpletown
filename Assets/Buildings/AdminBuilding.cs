using UnityEngine;
using System.Collections;

public class AdminBuilding : Building {
	public static Building Create (Vector3 vector3)
	{
		var baseGO = GameObject.CreatePrimitive (PrimitiveType.Cube);
		baseGO.renderer.material.color = Color.blue;
		baseGO.name = "Base";
		var building = baseGO.AddComponent<AdminBuilding> ();
		//AstarPath.active.UpdateGraphs (building.collider.bounds);
		return building;
	}

	public override bool HasStorage ()
	{
		return true;
	}
	public override bool IsAdmin {
		get {
			return true;
		}
	}
}
