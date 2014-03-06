
using UnityEngine;

public class Tree
{
    public static string PrefabPath = "Tree";
    public static GameObject Prefab = null;

    public static GameObject Create(Vector3 position)
    {
        Prefab = Prefab ?? Resources.Load<GameObject>(PrefabPath);
        GameObject go = GameObject.Instantiate(Prefab) as GameObject;
        go.transform.position = position;
        return go;
    }
}