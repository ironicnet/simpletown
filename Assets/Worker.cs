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
            Prefabs.Add(workerType, Resources.Load<GameObject>(PrefabPath + WorkerType.Builder.ToString()));
        GameObject workerGO = GameObject.Instantiate(Prefabs[workerType]) as GameObject;
        workerGO.name = workerType.ToString();
        workerGO.transform.position = new Vector3(building.Waypoint.transform.position.x, 0, building.Waypoint.transform.position.z);
        Debug.Log(string.Format("Worker Pos: {0}. Waypoint: {1}", workerGO.transform.position, building.Waypoint.transform.position));
        return workerGO.GetComponent<Worker>();
    }

    private bool showOptions = false;
    public WorkerStatus Status = WorkerStatus.Free;
    GameObject baseBuilding = null;
    private GameObject _graphics = null;
    private Vector3 _destination;
    public Vector3 Destination 
    {
        get
        {
            return _destination;
        }
        set
        {
            _destination = value;
            Debug.Log(string.Format("New Destination: {0}", value));
            DestinationArrived=false;
            seeking=false;
        }
    }public bool DestinationArrived
    {
        get;
        private set;
    }

    
    //The calculated path
    public Pathfinding.Path path;
    bool seeking = false;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

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
        Debug.Log(string.Format("DestinationArrived: {0}. Seeking: {1}. Destination: {2}", DestinationArrived, seeking, Destination));
        if (!DestinationArrived)
        {
            if (!seeking)
            {
                Seeker seeker = this.GetComponent<Seeker>();
                currentWaypoint=0;
                
                //Start a new path to the targetPosition, return the result to the OnPathComplete function
                seeker.StartPath(this.transform.position, Destination, OnPathComplete);
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
                    Graphics.GetComponent<Animation>().Play("walk");
                    MoveTowardsTarget(waypointDestination);
                    var distance = Vector3.Distance(transform.position, waypointDestination);
                    if (distance < 0.2f)
                    {
                        currentWaypoint++;
                        Debug.Log(string.Format("Distance from {0} to {1}: {2}", transform.position, Destination, distance));
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
            }
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
        if (!DestinationArrived)
        {
            if (path != null)
            {
                foreach (var waypoint in path.vectorPath)
                {
                    Gizmos.DrawWireSphere(waypoint, 0.05f);
                }
                if (currentWaypoint<path.vectorPath.Count)
                Gizmos.DrawLine(transform.position, path.vectorPath [currentWaypoint]);
            }
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
        SetStatus(WorkerStatus.Active);
        CurrentTaskPlan.Add(new MoveToPositionTask(mousePosition));
    }

    public void StartBuild(Building building)
    {
        SetStatus(WorkerStatus.Active);
        CurrentTaskPlan.Add(new BuildTask(building));
    }

    protected void MoveTowardsTarget(Vector3 target) 
    {
        var cc = GetComponent<CharacterController>();
        var offset = Util.DiscardYFromPosition(target, transform.position.y) - transform.position;
        //Get the difference.
        if(offset.magnitude > .01f) {
            //If we're further away than .1 unit, move towards the target.
            //The minimum allowable tolerance varies with the speed of the object and the framerate. 
            // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
            offset = offset.normalized * Speed;
            //normalize it and account for movement speed.
            cc.Move(offset * Time.deltaTime);
            //actually move the character.
        }
    }
    public void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        } else
        {
            
            Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
        }
    }
    
    public void Kill()
    {

    }
}

