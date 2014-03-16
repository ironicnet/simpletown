using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveToPositionTask : ITask
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

    public MoveToPositionTask(Vector3 destination)
    {
        Destination = destination;
    }
    
    public Vector3 Destination;
    private float increment;
    private Quaternion rotation;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.01f;
    

    public void Execute(Worker worker)
    {
        worker.SetStatus(WorkerStatus.Travelling, "Moving to position");
        if (worker.Destination != Destination)
        worker.Destination = Destination;
    }


    public bool EvaluateCompletion(Worker worker)
    {
        return worker.DestinationArrived;
    }
    public void OnDrawGizmos(Worker worker)
    {
    }
}