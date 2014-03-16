using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveToBuildingTask : MoveToPositionTask
{
    Building TargetBuilding;
    public MoveToBuildingTask(Building building) : base(building.Waypoint.transform.position)
    {
        TargetBuilding = building;
    }
}