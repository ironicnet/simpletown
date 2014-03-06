using UnityEngine;
using System.Collections;

public class House : Building
{
    public Worker owner = null;

    public static House Create(Vector3 position)
    {
        GameObject buildingGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var building = buildingGO.AddComponent<House>();
        buildingGO.transform.position = position;
        buildingGO.name = "House";
        buildingGO.renderer.material.color = Color.yellow;
        return building;
    }

    public House()
    {
        this.Requirements.Add("Wood", new ResourceAmount("Wood", 5));
    }

    protected override void BuildComplete()
    {
        owner = Worker.Create(this);
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
