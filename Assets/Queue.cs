using System.Collections.Generic;

public class Queue
{
	protected Dictionary<string, ResourceAmount> Stock = new Dictionary<string, ResourceAmount>();

	public virtual void Pull(string resourceName, int amount)
	{
		Stock [resourceName].Amount -= amount;
		if (Stock [resourceName].Amount <= 0)
			Stock.Remove (resourceName);
	}
	public virtual void Put(string resourceName, int amount)
	{
		if (!Stock.ContainsKey (resourceName)) {
			Stock.Add (resourceName, new ResourceAmount (resourceName, amount));
		} else {
			Stock [resourceName].Amount += amount;
		}
	}
	public int InStock(string resourceName)
	{
		return Stock.ContainsKey (resourceName) && Stock [resourceName].Amount > 0 ? Stock [resourceName].Amount : 0;
	}
}
