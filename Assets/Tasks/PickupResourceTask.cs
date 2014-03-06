using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class PickupResourceTask : ITask
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

    public PickupResourceTask(Building fromSourceBuilding, Building toTargetBuilding, string resourceName, int amountRequested)
    {
        FromSourceBuilding = fromSourceBuilding;
        ToTargetBuilding = toTargetBuilding;
        ResourceName = resourceName;
        AmountRequested = amountRequested;
        moveToSourceTask = new MoveToBuildingTask(FromSourceBuilding);
        moveToTargetTask = new MoveToBuildingTask(toTargetBuilding);
    }
    
    public Building FromSourceBuilding;
    public Building ToTargetBuilding;
    public string ResourceName;
    public int  AmountRequested;
    private bool ArrivedStorage = false;
    private bool ResourcePickedUp = false;
    private bool CarryingResouce = false;
    private bool ArrivedDestination = false;
    private bool ResourceDelivered = false;
    private MoveToBuildingTask moveToSourceTask;
    private MoveToBuildingTask moveToTargetTask;
    private float increment;
    private Quaternion rotation;
    
    public void Execute(Worker worker)
    {
        if (!_isComplete)
        {
            if (!ArrivedStorage)
            {
                moveToSourceTask.Execute(worker);
            } else if (ArrivedStorage && !ResourcePickedUp)
            {
                worker.SetStatus(WorkerStatus.Working);
                if (moveToSourceTask.EvaluateCompletion(worker))
                {
                    var queue = FromSourceBuilding.OutQueue;
                    queue.Pull(ResourceName, AmountRequested);
                    ResourcePickedUp = true;
                    CarryingResouce = true;
                }
            } else if (ArrivedStorage && CarryingResouce && !ArrivedDestination)
            {   
                moveToTargetTask.Execute(worker);
            } else if (ArrivedStorage && CarryingResouce && ArrivedDestination)
            {   
                worker.SetStatus(WorkerStatus.Working);
                if (moveToTargetTask.EvaluateCompletion(worker))
                {
                    ToTargetBuilding.InQueue.Put(ResourceName, AmountRequested);
                    ResourceDelivered = true;
                    CarryingResouce = false;
                }
            }
        }
    }
    
    public bool EvaluateCompletion(Worker worker)
    {
        if (!ArrivedStorage)
        {
            ArrivedStorage = moveToSourceTask.EvaluateCompletion(worker);
        } else if (ArrivedStorage && CarryingResouce && !ArrivedDestination)
        {   
            ArrivedDestination = moveToTargetTask.EvaluateCompletion(worker);
        }
        _isComplete = (ArrivedStorage && ResourcePickedUp && ArrivedDestination && ResourceDelivered);
        return _isComplete;
    }

    public void OnDrawGizmos(Worker worker)
    {
        Vector3 gizmoPosition = worker.transform.position;
        gizmoPosition.y = 10;
        if (!ArrivedStorage)
        {
            TextGizmo.Instance.DrawText(Camera.main, gizmoPosition, "Going to the storage");
            Gizmos.DrawLine(worker.transform.position, FromSourceBuilding.transform.position);
        } else if (ArrivedStorage && !ResourcePickedUp)
        {
            TextGizmo.Instance.DrawText(Camera.main, gizmoPosition, string.Format("Picking up {0} of {1}", AmountRequested, ResourceName));
        } else if (ArrivedStorage && CarryingResouce && !ArrivedDestination)
        {   
            TextGizmo.Instance.DrawText(Camera.main, gizmoPosition, string.Format("Delivering {0} of {1}", AmountRequested, ResourceName));
            Gizmos.DrawLine(worker.transform.position, ToTargetBuilding.transform.position);
        } else if (ArrivedStorage && CarryingResouce && ArrivedDestination)
        {   
            TextGizmo.Instance.DrawText(Camera.main, gizmoPosition, "Done!");
        }
    }
}