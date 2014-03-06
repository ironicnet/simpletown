using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class Worker : BaseComponent
{
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

    public static Worker Create(Building building, WorkerType workerType)
    {
        if (!Prefabs.ContainsKey(workerType))
            Prefabs.Add(workerType, Resources.Load<GameObject>(PrefabPath + workerType.ToString()));
        GameObject workerGO = GameObject.Instantiate(Prefabs[workerType]) as GameObject;
        workerGO.name = workerType.ToString();
        workerGO.transform.position = new Vector3(building.transform.position.x, building.transform.position.y, building.transform.position.y);
        return workerGO.GetComponent<Worker>();
    }

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
            SetStatus(WorkerStatus.Free);
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
                SetStatus(WorkerStatus.Working);
                SendMessageTo(baseBuilding, "Build", new BuildPlan() { BuildType= "House", Worker=this, Location=this.transform.position});
            }
            if (GUI.Button(new Rect(V.x, Screen.height - V.y + 40, 150, 30), "W"))
            {
                SetStatus(WorkerStatus.Working);
                SendMessageTo(baseBuilding, "Build", new BuildPlan() { BuildType= "WoodcutterHouse", Worker=this, Location=this.transform.position});
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

    public void SetStatus(WorkerStatus status)
    {
        this.Status = status;
        switch (status)
        {
            case WorkerStatus.Free:
                Graphics.renderer.material.color = Color.gray;
                break;
            case WorkerStatus.Active:
                Graphics.renderer.material.color = Color.yellow;
                break;
            case WorkerStatus.Travelling:
                Graphics.renderer.material.color = Color.green;
                break;
            case WorkerStatus.Working:
                Graphics.renderer.material.color = Color.red;
                break;
            default:
                break;
        }

    }
    
    public void GoAndWait(Vector3 mousePosition)
    {
        SetStatus(WorkerStatus.Active);
        CurrentTaskPlan.Add(new MoveToPositionTask(mousePosition));
    }

    public void StartBuild(Building building)
    {
        SetStatus(WorkerStatus.Active);
        CurrentTaskPlan.Add(new BuildTask(building));
    }

    public void Kill()
    {

    }
}

