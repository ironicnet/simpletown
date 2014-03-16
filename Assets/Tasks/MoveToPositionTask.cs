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
    //The calculated path
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.01f;
    private bool DestinationArrived
    {
        get;
        set;
    }
    Worker Worker;
    Pathfinding.Path path;
    bool seeking = false;
    

    public void Execute(Worker worker)
    {
        Worker = worker;
        worker.SetStatus(WorkerStatus.Travelling, "Moving to position");
        
        
        if (!DestinationArrived)
        {
            if (!seeking)
            {
                Seeker seeker = worker.GetComponent<Seeker>();
                currentWaypoint=0;
                
                //Start a new path to the targetPosition, return the result to the OnPathComplete function
                seeker.StartPath(worker.transform.position, Destination, OnPathComplete);
                seeking = true;
            } 
            else
            {
                if (path == null)
                {
                    //We have no path to move after yet
                    return;
                }
                
                if (currentWaypoint >= path.vectorPath.Count)
                {
                    return;
                }
                else
                {
                    DestinationArrived=false;
                    var waypointDestination = path.vectorPath[currentWaypoint];
                    worker.SetAnimation( Worker.Animations.Walking);
                    MoveTowardsTarget(waypointDestination);
                    var distance = Vector3.Distance(worker.transform.position, waypointDestination);
                    if (distance < 0.2f)
                    {
                        currentWaypoint++;
                        //                        Debug.Log(string.Format("Distance from {0} to {1}: {2}", transform.position, Destination, distance));
                    }
                    else
                    {
                    }
                }
            }
            if (!DestinationArrived)
            {
                DestinationArrived = path!=null && (currentWaypoint>=path.vectorPath.Count);
                if(DestinationArrived)
                {
                    path=null;
                    seeking = false;
                    currentWaypoint=0;
                }
                else
                {
                    Debug.Log(string.Format("Path: {2}. Waypoint {0}/{1}", currentWaypoint,path!=null ? path.vectorPath.Count : 0, path));
                }
            }
        }
    }
    public void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            path = p;
            Destination = p.vectorPath.Last();
            //Reset the waypoint counter
            currentWaypoint = 0;
        } else
        {
            
            Debug.Log(string.Format("#{0}. Yey, we got a path back. Did it have an error? {1}", Worker.ID,p.error));
        }
    }
    protected void MoveTowardsTarget(Vector3 target) 
    {
        var cc = Worker.GetComponent<CharacterController>();
        var offset = Util.DiscardYFromPosition(target, Worker.transform.position.y) - Worker.transform.position;
        //Get the difference.
        if(offset.magnitude > .01f) {
            var lookPos = target - Worker.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            Worker.transform.rotation = Quaternion.Slerp(Worker.transform.rotation, rotation, Time.deltaTime * 5);
            //If we're further away than .1 unit, move towards the target.
            //The minimum allowable tolerance varies with the speed of the object and the framerate. 
            // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
            offset = offset.normalized * Worker.Speed;
            //normalize it and account for movement speed.
            
            cc.Move(offset * Time.deltaTime);
            //actually move the character.
        }
    }
    


    public bool EvaluateCompletion(Worker worker)
    {
        _isComplete = DestinationArrived;
        return _isComplete;
    }
    public void OnDrawGizmos(Worker worker)
    {
        if (!DestinationArrived)
        {
            if (path != null)
            {
                foreach (var waypoint in path.vectorPath)
                {
                    Gizmos.DrawWireSphere(waypoint, 0.05f);
                }
                if (currentWaypoint<path.vectorPath.Count)
                    Gizmos.DrawLine(worker.transform.position, path.vectorPath [currentWaypoint]);
            }
        }
    }
}