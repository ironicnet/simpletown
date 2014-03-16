using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class Worker : BaseComponent
{
    public enum Animations
    {
        Idle,
        Walking,
        WalkingCarrying,
        IdleCarrying,
        Building,
        Chopping
    }
    public WorkerType WorkerType
    {
        get;
        set;
    }

    public Building WorkingBuilding
    {
        get;
        set;

    }
    public static string PrefabPath = "Workers/";
    public static Dictionary<WorkerType, GameObject> Prefabs = new Dictionary<WorkerType, GameObject>();

    public static Worker Create(GameManager gameManager, Building building, WorkerType workerType)
    {
        if (!Prefabs.ContainsKey(workerType))
            Prefabs.Add(workerType, Resources.Load<GameObject>(PrefabPath + WorkerType.Builder.ToString()));
        GameObject workerGO = GameObject.Instantiate(Prefabs[workerType]) as GameObject;
        var worker = workerGO.GetComponent<Worker>();
        worker.ID = gameManager.LastID++;
        workerGO.name = workerType.ToString()+  " "  + worker.ID.ToString();
        workerGO.transform.position = new Vector3(building.Waypoint.transform.position.x, 0, building.Waypoint.transform.position.z);
        
        
        Debug.Log(string.Format("#{0}. Worker Pos: {1}. Waypoint: {2}",worker.ID, workerGO.transform.position, building.Waypoint.transform.position));
        return worker;
    }
    public int ID;
    private bool showOptions = false;
    public WorkerStatus Status = WorkerStatus.Free;
    GameObject baseBuilding = null;
    private GameObject _graphics = null;
    

    // Use this for initialization
    protected virtual void Awake()
    {
    }
    protected virtual void Start()
    {
        baseBuilding = GameObject.FindGameObjectWithTag("Base");
    }

    protected virtual GameObject Graphics
    {
        get{
            if (_graphics==null)
                _graphics = transform.FindChild("Graphics").gameObject;
            return _graphics;
        }
    }

    public float Speed = 0.05f;
    public float BuildForce = 0.05f;
    public List<ITask> CurrentTaskPlan = new List<ITask>();


    // Update is called once per frame
    void Update()
    {
        var currentTask = CurrentTaskPlan.FirstOrDefault();
        if (currentTask != null)
        {
            currentTask.Execute(this);
            if (currentTask.EvaluateCompletion(this))
            {
                CurrentTaskPlan.Remove(currentTask);
            }
        } else if (WorkingBuilding == null && Status != WorkerStatus.Free)
        {
            SetStatus(WorkerStatus.Free, "No working building...");
        }
        else
        {
            Graphics.GetComponent<Animation>().Play("idle_0");
        }
    }

    void OnDrawGizmos()
    {
        var currentTask = CurrentTaskPlan.FirstOrDefault();
        if (currentTask != null)
        {
            currentTask.OnDrawGizmos(this);
        }
    }
    void OnGUI()
    {
        if (showOptions)
        {
            Vector3 V = Camera.main.WorldToScreenPoint(this.transform.position);
            if (GUI.Button(new Rect(V.x, Screen.height - V.y, 150, 30), "H"))
            {
                SetStatus(WorkerStatus.Working, "Requested House to be Built");
                SendMessageTo(baseBuilding, "Build", new BuildPlan() { BuildType= "House", Worker=this, Location=this.transform.position});
            }
            if (GUI.Button(new Rect(V.x, Screen.height - V.y + 40, 150, 30), "W"))
            {
                SetStatus(WorkerStatus.Working, "Requested WoodcutterHouse to be Built");
                SendMessageTo(baseBuilding, "Build", new BuildPlan() { BuildType= "WoodcutterHouse", Worker=this, Location=this.transform.position});
            }
            if (GUI.Button(new Rect(V.x, Screen.height - V.y + 80, 150, 30), "Test"))
            {
                CurrentTaskPlan.Add(new MoveToPositionTask(transform.position *1.3f));
                CurrentTaskPlan.Add(new MoveToPositionTask(transform.position *-1.4f));
                CurrentTaskPlan.Add(new MoveToPositionTask(transform.position *1.4f));
                CurrentTaskPlan.Add(new MoveToPositionTask(transform.position *-1.3f));
            }
        }
    }
    
    public void ShowOptions()
    {
        if (Status == WorkerStatus.Free)
        {
            showOptions = true;
        }
    }

    public void HideOptions()
    {
        showOptions = false;
    }

    public void SetStatus(WorkerStatus status, string reason)
    {
        if (status!=this.Status)
            Debug.Log(string.Format("#{0}. Changing status from {1} to {2}. Reason: {3}", this.ID, this.Status, status, reason));

        this.Status = status;
        switch (status)
        {
            case WorkerStatus.Free:
                //Graphics.renderer.material.color = Color.gray;
                break;
            case WorkerStatus.Active:
                //Graphics.renderer.material.color = Color.yellow;
                break;
            case WorkerStatus.Travelling:
                //Graphics.renderer.material.color = Color.green;
                break;
            case WorkerStatus.Working:
                //Graphics.renderer.material.color = Color.red;
                break;
            default:
                break;
        }

    }
    
    public void GoAndWait(Vector3 mousePosition)
    {
        SetStatus(WorkerStatus.Active, "Going to position and wait");
        CurrentTaskPlan.Add(new MoveToPositionTask(mousePosition));
    }

    public void StartBuild(Building building)
    {
        SetStatus(WorkerStatus.Active, "Start Building!");
        CurrentTaskPlan.Add(new BuildTask(building));
    }

    public void Kill()
    {

    }

    public void SetAnimation(Animations animation)
    {
        
    }
}

