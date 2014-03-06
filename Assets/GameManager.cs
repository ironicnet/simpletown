using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : BaseComponent
{

    public int maxWorkers = 1;
    public LayerMask BuildingLayer;
    public LayerMask UnitLayer;
    List<Worker> currentWorkers = new List<Worker>();
    List<Building> buildings = new List<Building>();
    public List<GameObject> trees = new List<GameObject>();
    Dictionary<string, Func<Vector3, Building>> buildInstantiators = new Dictionary<string, Func<Vector3, Building>>()
    {
        { "House", House.Create},
        { "Admin", AdminBuilding.Create},
        { "WoodcutterHouse", WoodcutterHouse.Create}
    };
    GameObject activeObject = null;
    // Use this for initialization
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<MeshFilter>().mesh = null;
        CreateTrees(50);
        CreateBase();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeObject == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = default(RaycastHit);

                if (!Physics.Raycast(ray, out hit) || hit.transform.gameObject.layer == 8)
                {
                    var position = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    SendWorkerTo(position);
                } else
                {
                    Debug.Log(hit.transform.gameObject.layer);
                }
            }
        } else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = default(RaycastHit);
            bool hitSomething = Physics.Raycast(ray, out hit);

            if ((!hitSomething && (activeObject != null)) || (hit.transform != null && hit.transform.gameObject == activeObject))
            {
                SendMessageTo(activeObject, "HideOptions", SendMessageOptions.DontRequireReceiver);
                activeObject = null;
            } else if (hit.transform != null)
            {
                activeObject = hit.transform.gameObject;
                SendMessageTo(activeObject, "ShowOptions", SendMessageOptions.DontRequireReceiver);
            }
        }
        foreach (var queuedRequest in QueueMaterialRequest)
        {
            var requiredAmount = ExecuteRequestMaterial(queuedRequest.Key, queuedRequest.Value.ResourceName, queuedRequest.Value.Amount);
            queuedRequest.Value.Amount = requiredAmount;
        }
        QueueMaterialRequest.RemoveAll(q => q.Value.Amount <= 0);
    }
    
    void CreateBase()
    {
        Building adminBuilding = AdminBuilding.Create(new Vector3(0, 0, 0));
        adminBuilding.SetAsBuilt();
        RegisterBuilding(adminBuilding);
        adminBuilding.OutQueue.Put("Wood", 20);
    }

    void SendWorkerTo(Vector3 mousePosition)
    {
        Worker worker = RequestWorker(mousePosition);

        if (worker != null)
        {
            worker.GoAndWait(mousePosition);
        }

    }

    bool isWorkerAvailable()
    {
        return currentWorkers.Any(w => w.Status == WorkerStatus.Free);
    }

    Worker getWorkerAvailable()
    {
        return currentWorkers.First(w => w.Status == WorkerStatus.Free);
    }

    public Worker RequestWorker(Building building, WorkerType workerType = WorkerType.Builder)
    {
        if (workerType == WorkerType.Builder)
        {
            if (isWorkerAvailable())
            {
                return currentWorkers.Where(w => w.GetComponent<Worker>().Status == WorkerStatus.Free).OrderBy(w => Vector3.Distance(w.transform.position, building.transform.position)).Select(w => w.GetComponent<Worker>()).FirstOrDefault();
            } else if (currentWorkers.Count < maxWorkers)
            { 
                var worker = Worker.Create(GetNearestAdminBuilding(building.transform.position), workerType);
                //worker.gameObject.layer = UnitLayer.value;
                RegisterWorker(worker);
                worker.WorkerType = workerType;
                return worker;  
            }
        } else
        {
            var worker = Worker.Create(GetNearestAdminBuilding(building.transform.position), workerType);
            //worker.gameObject.layer = UnitLayer.value;
            worker.name = workerType.ToString();
            worker.WorkerType = workerType;
            return worker;
        }
        return null;
    }

    public Building GetNearestAdminBuilding(Vector3 position)
    {
        return buildings.Where(b => b.IsAdmin).OrderBy(b => Vector3.Distance(b.transform.position, position)).First();
    }

    public Worker RequestWorker(Vector3 position)
    {
        if (isWorkerAvailable())
        {
            return currentWorkers.Where(w => w.GetComponent<Worker>().Status == WorkerStatus.Free)
                                    .OrderBy(w => Vector3.Distance(w.transform.position, Util.DiscardYFromPosition(position, w.transform.position.y)))
                                    .Select(w => w.GetComponent<Worker>())
                                    .FirstOrDefault();
        } else if (currentWorkers.Count < maxWorkers)
        { 
            Worker worker = Worker.Create(GetNearestAdminBuilding(position), WorkerType.Builder);
            //worker.gameObject.layer = LayerMask.NameToLayer(UnitLayer);
            RegisterWorker(worker);
            return worker;  
        }
        return null;
    }

    public void RequestMaterial(Building building, string resourceName, int amountRequired)
    {
        Debug.Log(string.Format("Building {0} requested {1} of {2}", building.name, amountRequired, resourceName));
        
        QueueMaterialRequest.Add(new KeyValuePair<Building, ResourceAmount>(building, new ResourceAmount(resourceName, amountRequired)));
    }

    public int ExecuteRequestMaterial(Building building, string resourceName, int amountRequired)
    {
        var storageBuildings = buildings.Where(b => b.HasStorage() && b.OutQueue.InStock(resourceName) > 0).OrderBy(b => Vector3.Distance(b.transform.position, building.transform.position));
        foreach (var storageBuilding in storageBuildings)
        {
            if (amountRequired == 0)
                break;
            var stock = storageBuilding.OutQueue.InStock(resourceName);
            var amountRequested = (stock >= amountRequired) ? amountRequired : stock;
            
            var transportWorker = RequestWorker(storageBuilding);
            if (transportWorker != null)
            {
                Debug.Log(string.Format("Transporting to {0} requested {1} of {2}", building.name, amountRequired, resourceName));
                transportWorker.CurrentTaskPlan.Add(new PickupResourceTask(storageBuilding, building, resourceName, amountRequested));
                transportWorker.CurrentTaskPlan.Add(new MoveToPositionTask(transportWorker.transform.position));
                amountRequired -= amountRequested;
            }
        }
        return amountRequired;
    }

    List<KeyValuePair<Building, ResourceAmount>> QueueMaterialRequest = new List<KeyValuePair<Building, ResourceAmount>>();

    public void Build(BuildPlan buildPlan)
    {
        Building building = buildInstantiators [buildPlan.BuildType](buildPlan.Location); //House.Create (buildPlan.Worker.transform.position);
        Debug.Log(string.Format("BuildType: {0}. Result: {1}. Location: {2}", buildPlan.BuildType, building, buildPlan.Location));
        
        //building.gameObject.layer = this.BuildingLayer;
        //AstarPath.active.UpdateGraphs (building.collider.bounds);
        RegisterBuilding(building);
        SendMessageTo(buildPlan.Worker.gameObject, "HideOptions");
        activeObject = null;
        buildPlan.Worker.StartBuild(building);
    }

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);
    }

    public void RegisterWorker(Worker worker)
    {
        currentWorkers.Add(worker);
    }
    
    GameObject treeContainer = null;
    void CreateTrees(int radius)
    {
        
        treeContainer = treeContainer ?? GameObject.Find("TreeContainer");
        if (treeContainer==null) treeContainer = new GameObject("TreeContainer");
        for (var i=0; i < 400; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-radius / 2, radius / 2),0, UnityEngine.Random.Range(-radius / 2, radius / 2));
            
            var treeGO = Tree.Create(pos);
            treeGO.name = "tree " + i.ToString();
            treeGO.transform.parent = treeContainer.transform;
            RegisterTree(treeGO);
        }
    }

    void RegisterTree(GameObject tree)
    {
        AstarPath.active.UpdateGraphs(tree.collider.bounds);
        trees.Add(tree);
    }

    public void UnregisterTree(GameObject tree)
    {
        tree.SetActive(false);
        AstarPath.active.UpdateGraphs(tree.collider.bounds);
        trees.Remove(tree);
    }
}
