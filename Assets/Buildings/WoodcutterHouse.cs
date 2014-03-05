using UnityEngine;
using System.Collections;
using System.Linq;

public class WoodcutterHouse : Building
{
		public enum Steps
		{
				Unoccupied,
				SearchingForWood,
				LookingForWood,
				CuttingWood,
				ReturningWithWood,
				ProcessingWood,
				DeliveringWood,
				ReturningFromDelivery
		}
		public Worker woodcutter = null;
		public Steps CurrentStep = Steps.Unoccupied;

		protected GameObject targetTree = null;

		public static WoodcutterHouse Create (Vector3 position)
		{
			GameObject buildingGO = GameObject.CreatePrimitive (PrimitiveType.Cube);
			var building = buildingGO.AddComponent<WoodcutterHouse> ();
			buildingGO.transform.position = position;
			buildingGO.name = "WoodcutterHouse";
			buildingGO.renderer.material.color = Color.green;

			return building;
		}

		public WoodcutterHouse ()
		{
			this.Requirements.Add ("Wood", new ResourceAmount ("Wood", 5));
		}

		protected override void BuildComplete ()
		{
			woodcutter = gameManager.RequestWorker (this, WorkerType.Woodcutter);
			woodcutter.WorkingBuilding = this;
			woodcutter.CurrentTaskPlan.Add (new MoveToBuildingTask (this));
			base.BuildComplete ();
		}

		protected override void BuildingDestroyed ()
		{
		}

	public int increment {
				get;
				private set;
		}
		protected ITask moveToTreeTask;
		protected ITask returnToHouseTask;
		protected ITask deliverWoodTask;
		protected override void Update ()
		{
			if (IsBuilt) {
						switch (CurrentStep) {
						case Steps.Unoccupied:
								if (Vector3.Distance (this.transform.position, woodcutter.transform.position) <= 0.1f) {
										CurrentStep = Steps.SearchingForWood;
								}
				break;
			case Steps.SearchingForWood:
				targetTree = gameManager.trees.OrderBy (t => Vector3.Distance (t.transform.position, woodcutter.transform.position)).FirstOrDefault ();
				if (targetTree != null) {
					CurrentStep = Steps.LookingForWood;
					increment=0;
					moveToTreeTask = new MoveToPositionTask (targetTree.transform.position);
					woodcutter.CurrentTaskPlan.Add (moveToTreeTask);
				}
				break;
			case Steps.LookingForWood:
				if (moveToTreeTask.IsComplete) {
					CurrentStep = Steps.CuttingWood;
				}
				break;
			case Steps.CuttingWood:
				increment++;
				if (increment>=100) {
					gameManager.UnregisterTree(targetTree);
					GameObject.Destroy(targetTree);
					CurrentStep = Steps.ReturningWithWood;
					returnToHouseTask = new MoveToBuildingTask (this);
					woodcutter.CurrentTaskPlan.Add (returnToHouseTask);
				}
				break;
				
			case Steps.ReturningWithWood:
				if (returnToHouseTask.IsComplete) {
					CurrentStep = Steps.ProcessingWood;
				}
				break;
			case Steps.ProcessingWood:
				
				increment++;
				if (increment>=20) {
					CurrentStep = Steps.DeliveringWood;
					deliverWoodTask = new MoveToPositionTask (this.transform .position+ this.transform.forward);
					woodcutter.CurrentTaskPlan.Add (deliverWoodTask);
				}
				break;
			case Steps.DeliveringWood:
				if (deliverWoodTask.IsComplete) {
					CurrentStep = Steps.ReturningFromDelivery;
					returnToHouseTask = new MoveToPositionTask (this.transform.position);
					woodcutter.CurrentTaskPlan.Add (returnToHouseTask);
				}
				break;
			case Steps.ReturningFromDelivery:
				if (returnToHouseTask.IsComplete) {
					CurrentStep = Steps.SearchingForWood;
				}
				break;
						default:
								break;
						}
			} else {
					base.Update();
			}
		}
}
