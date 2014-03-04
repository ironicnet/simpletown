using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class Worker : BaseComponent
{
	public WorkerType WorkerType {
		get;
		set;
	}
	public Building WorkingBuilding {
				get;
				set;

	}
	public static Worker Create (Building building)
	{
		var workerGO = GameObject.CreatePrimitive (PrimitiveType.Capsule);
		workerGO.name = "Worker";

		workerGO.renderer.material.color = Color.gray;
		workerGO.transform.position = new Vector3(building.transform.position.x, building.transform.position.y,building.transform.position.y);
		workerGO.AddComponent<Seeker> ();
		workerGO.AddComponent<CharacterController> ();
		var worker = workerGO.AddComponent<Worker> ();
		workerGO.transform.localScale -= new Vector3 (0.7f, 0.7f, 0.7f);
		return worker;
	}
		private bool showOptions = false;
		public WorkerStatus Status = WorkerStatus.Free;
		GameObject baseBuilding = null;
		// Use this for initialization
		void Start ()
		{
				baseBuilding = GameObject.FindGameObjectWithTag ("Base");
		}

		public float Speed = 0.05f;
		public float BuildForce = 0.05f;

		public List<ITask> CurrentTaskPlan = new List<ITask> ();


		// Update is called once per frame
		void Update ()
		{
		var currentTask = CurrentTaskPlan.FirstOrDefault ();
				if (currentTask != null) {
						currentTask.Execute (this);
						if (currentTask.EvaluateCompletion (this)) {
				CurrentTaskPlan.Remove (currentTask);
						}
		} else if (WorkingBuilding ==null && Status != WorkerStatus.Free) {
						SetStatus (WorkerStatus.Free);
				}
		}

		void OnDrawGizmos ()
	{
		var currentTask = CurrentTaskPlan.FirstOrDefault ();
		if (currentTask != null) {
			currentTask.OnDrawGizmos(this);
				}
		}

		void OnGUI ()
		{
				if (showOptions) {
			Vector3 V = Camera.main.WorldToScreenPoint (this.transform.position);
			if (GUI.Button (new Rect (V.x, Screen.height - V.y, 150, 30), "H")) {
				SetStatus (WorkerStatus.Working);
				SendMessageTo (baseBuilding, "Build", new BuildPlan () { BuildType= "House", Worker=this, Location=this.transform.position});
			}
			if (GUI.Button (new Rect (V.x, Screen.height - V.y+40, 150, 30), "W")) {
				SetStatus (WorkerStatus.Working);
				SendMessageTo (baseBuilding, "Build", new BuildPlan () { BuildType= "WoodcutterHouse", Worker=this, Location=this.transform.position});
			}
				}
		}
	
		public void ShowOptions ()
		{
				if (Status == WorkerStatus.Free) {
						showOptions = true;
				}
		}

		public void HideOptions ()
		{
				showOptions = false;
		}

		public void SetStatus (WorkerStatus status)
		{
				this.Status = status;
		switch (status) {
				case WorkerStatus.Free:
					this.renderer.material.color = Color.gray;
					break;
				case WorkerStatus.Active:
					this.renderer.material.color = Color.yellow;
					break;
				case WorkerStatus.Travelling:
						this.renderer.material.color = Color.green;
						break;
				case WorkerStatus.Working:
						this.renderer.material.color = Color.red;
						break;
				default:
						break;
				}

		}
	
	
		public void GoAndWait (Vector3 mousePosition)
	{
		SetStatus (WorkerStatus.Active);
		CurrentTaskPlan.Add (new MoveToPositionTask(mousePosition));
		}
	public void StartBuild (Building building)
		{
			SetStatus (WorkerStatus.Active);
		CurrentTaskPlan.Add (new BuildTask(building));
		}

	public void Kill ()
	{

	}
}

