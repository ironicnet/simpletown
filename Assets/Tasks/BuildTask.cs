using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class BuildTask : ITask
{
    public bool IsCancelable
    {
        get
        {
            return false;
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
    
    protected float porcentage;
    protected Building Building;

    public BuildTask(Building building)
    {
        Building = building;
    }
    
    public void Execute(Worker worker)
    {
        worker.SetStatus(WorkerStatus.Working);
        var building = Building.GetComponent<Building>();
        if (building.RequirementsMet)
            building.AddBuildForce(worker);
    }
    
    public bool EvaluateCompletion(Worker worker)
    {
        return Building.GetComponent<Building>().IsBuilt;
    }

    public void OnDrawGizmos(Worker worker)
    {
    }
}