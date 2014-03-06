using UnityEngine;
using System.Collections;

public class AdminBuilding : Building
{
    public static string PrefabPath = "Base";
    public static GameObject Prefab = null;
    public static Building Create(Vector3 vector3)
    {
        Prefab = Prefab ?? Resources.Load<GameObject>(PrefabPath);
        GameObject buildingGO = GameObject.Instantiate(Prefab) as GameObject;
        buildingGO.name = "Base";
        buildingGO.transform.position = vector3;
        var building = buildingGO.GetComponent<Building>();
        return building;
    }

    public override bool HasStorage()
    {
        return true;
    }

    public override bool IsAdmin
    {
        get
        {
            return true;
        }
    }
}
