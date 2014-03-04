using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;



public interface ITask
{
	bool IsCancelable { get; }
	
	bool IsComplete { get; }
	
	void Execute (Worker worker);
	
	bool EvaluateCompletion (Worker worker);
	
	void OnDrawGizmos (Worker worker);

	List<ITask> SubTasks{ get;}
}