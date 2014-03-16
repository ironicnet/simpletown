using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveToBuildingTask : ITask
{
    public bool IsCancelable
    {
        get
        {
            return true;
        }
    }
    
    private bool _isComplete = false;
    
    public bool IsComplete
    {
        get
        {
            return _isComplete;
        }
    }

    private List<ITask> _subTasks = new List<ITask>();

    public List<ITask> SubTasks
    {
        get
        {
            return _subTasks;
        }
    }

    public MoveToBuildingTask(Building building)
    {
        Destination = building;
    }
    
    public Building Destination;

    public void Execute(Worker worker)
    {
        worker.SetStatus(WorkerStatus.Travelling, "Moving to building #" + Destination.ID.ToString());
        Debug.Log(String.Format("Pos: {0}. LPos: {1}", Destination.Waypoint.transform.position,Destination.Waypoint.transform.localPosition));
        if (worker.Destination != Destination.Waypoint.transform.position)
            worker.Destination = Destination.Waypoint.transform.position;
    }
    public bool EvaluateCompletion(Worker worker)
    {
        return worker.DestinationArrived;
    }
    public void OnDrawGizmos(Worker worker)
    {
    }
}