using UnityEngine;
using System.Collections;

public class House : Building
{
    public static string PrefabPath = "House";
    public static GameObject Prefab = null;
    public Worker owner = null;

    public static House Create(Vector3 position)
    {
        Prefab = Prefab ?? Resources.Load<GameObject>(PrefabPath);
        GameObject buildingGO = GameObject.Instantiate(Prefab) as GameObject;
        buildingGO.name = "House";
        buildingGO.transform.position = position;
        var building = buildingGO.GetComponent<House>();
        return building;
    }

    public House()
    {
        this.Requirements.Add("Wood", new ResourceAmount("Wood", 5));
    }

    protected override void BuildComplete()
    {
        owner = Worker.Create(this, WorkerType.Builder);
        owner.gameObject.layer = gameManager.UnitLayer.value;
        gameManager.maxWorkers++;
        base.BuildComplete();
    }

    protected override void BuildingDestroyed()
    {
        base.BuildComplete();
        gameManager.maxWorkers--;
        owner.Kill();
    }
}
