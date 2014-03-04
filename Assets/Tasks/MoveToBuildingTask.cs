using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveToBuildingTask : ITask
{
	public bool IsCancelable {
		get {
			return true;
		}
	}
	
	private bool _isComplete = false;
	
	public bool IsComplete {
		get {
			return _isComplete;
		}
	}
	private List<ITask> _subTasks = new List<ITask> ();
	public List<ITask> SubTasks {
		get{
			return _subTasks;
		}
	}
	public MoveToBuildingTask (Building building)
	{
		Destination = building;
	}
	
	public Building Destination;
	private float increment;
	private Quaternion rotation;
	
	bool seeking = false;
	//The calculated path
	public Pathfinding.Path path;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 0.01f;
	
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	public void Execute (Worker worker)
	{
		worker.SetStatus (WorkerStatus.Travelling);
		Vector3 direction = Util.DiscardYFromPosition(Destination.transform.position,worker.transform.position.y) - worker.transform.position;
		rotation = Quaternion.LookRotation (direction);
		worker.transform.rotation = Quaternion.Slerp (worker.transform.rotation, rotation, increment);
		
		if (!seeking) {
			
			Seeker seeker = worker.GetComponent<Seeker> ();
			
			//Start a new path to the targetPosition, return the result to the OnPathComplete function
			seeker.StartPath (worker.transform.position, Destination.transform.position, OnPathComplete);
			seeking = true;
		} else {
			if (path == null) {
				//We have no path to move after yet
				return;
			}
			
			if (currentWaypoint >= path.vectorPath.Count) {
				return;
			}
			var dist = Vector3.Distance (worker.transform.position, Util.DiscardYFromPosition (path.vectorPath [currentWaypoint], worker.transform.position.y));
			if (increment <= 1)
				increment += (worker.Speed * Time.deltaTime) / dist; 
			//Direction to the next waypoint
			Vector3 dir = (path.vectorPath [currentWaypoint] - worker.transform.position).normalized;
			dir *= worker.Speed * Time.fixedDeltaTime;
			
			worker.transform.position = Vector3.Lerp (worker.transform.position, Util.DiscardYFromPosition (path.vectorPath [currentWaypoint], worker.transform.position.y), increment);
			
			
			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (worker.transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance) {
				currentWaypoint++;
				increment = 0;
				return;
			}
		}
	}
	
	public void OnPathComplete (Pathfinding.Path p) {
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
		else
			
			Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
	}
	public bool EvaluateCompletion (Worker worker)
	{
		_isComplete= path!=null && (currentWaypoint >= path.vectorPath.Count);;
		return _isComplete;
	}
	public void OnDrawGizmos(Worker worker)
	{
		
		if (path != null) {
			foreach (var waypoint in path.vectorPath) {
				Gizmos.DrawWireSphere (waypoint, 0.05f);
			}
			Gizmos.DrawLine (worker.transform.position, path.vectorPath [currentWaypoint]);
		}
	}
}