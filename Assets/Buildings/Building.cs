using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Building : BaseComponent
{
    protected float buildProgress = 0f;
    private bool showOptions = false;
    protected GameManager gameManager;
    protected Dictionary<string, ResourceAmount> Requirements = new Dictionary<string, ResourceAmount>();
    public Queue InQueue = new Queue();
    public Queue OutQueue = new Queue();
    protected List<KeyValuePair<string, ResourceAmount>> Requests = new List<KeyValuePair<string, ResourceAmount>>();

    public virtual bool IsAdmin
    {
        get
        {
            return false;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        
        if (!IsBuilt)
        {
            var totalRequired = 0;
            foreach (var resourceName in Requirements.Keys)
            {
                int amountRequired = Requirements [resourceName].Amount;
                amountRequired -= InQueue.InStock(resourceName);

                totalRequired += amountRequired;
                if (Requests.Any(r => r.Key == resourceName))
                {
                    amountRequired -= Requests.Where(r => r.Key == resourceName).Sum(r => r.Value.Amount);
                }
                if (amountRequired > 0)
                {
                    gameManager.RequestMaterial(this, resourceName, amountRequired);
                    Requests.Add(new KeyValuePair<string, ResourceAmount>(resourceName, new ResourceAmount(resourceName, amountRequired)));
                }
            }
            RequirementsMet = (totalRequired == 0);
        }
    
    }

    protected virtual void OnGUI()
    {
        if (!IsBuilt)
        {
            Vector2 size = new Vector2(50, 5);
            Vector3 pos = Camera.main.WorldToScreenPoint(this.transform.position);
            // Constrain all drawing to be within a pixel area .
            GUI.BeginGroup(new Rect(pos.x - size.x / 2, Screen.height - pos.y + 25, size.x, size.y));
            // Define progress bar texture within customStyle under Normal > Background
            GUI.Box(new Rect(1, 0, buildProgress * size.x / 100, size.y), buildProgress.ToString());
            // Always match BeginGroup calls with an EndGroup call
            GUI.EndGroup();



            if (showOptions)
            {
                if (GUI.Button(new Rect(pos.x - size.x / 2, Screen.height - pos.y + 25, 50, 30), "Add Worker"))
                {
                    var worker = gameManager.RequestWorker(this);
                    if (worker != null)
                    {
                        worker.CurrentTaskPlan.Add(new MoveToPositionTask(this.transform.position));
                        worker.CurrentTaskPlan.Add(new BuildTask(this));
                    }
                }
            }
        }
    }

    public void AddBuildForce(Worker worker)
    {
        if (!IsBuilt)
        {
            buildProgress += worker.BuildForce;
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, buildProgress * 255 / 100);
            if (IsBuilt)
                BuildComplete();
        } else
            new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 255);

    }
    
    protected virtual void BuildComplete()
    {
        
    }

    protected virtual void BuildingDestroyed()
    {
        
    }

    public virtual bool HasStorage()
    {
        return false;
    }

    public void ShowOptions()
    {
        showOptions = true;
    }
    
    public void HideOptions()
    {
        showOptions = false;
    }

    public void SetAsBuilt()
    {
        buildProgress = 101;
    }
    
    public bool IsBuilt
    {
        get
        {
            return buildProgress > 100f;
        }
    }

    public bool RequirementsMet
    {
        get;
        private set;
    }
}
