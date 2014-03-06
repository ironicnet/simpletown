using UnityEngine;
using System.Collections;
using UnityEngine.Internal;

public abstract class BaseComponent : MonoBehaviour
{
    /*
    public virtual void ShowOptions ()
    {
        Debug.Log ("Requested to show options (" +  this.GetType().Name + ")");
    }
    public virtual void HideOptions ()
    {
        Debug.Log ("Requested to hide options (" +  this.GetType().Name + ")");
    }*/
    public void  SendMessageTo(GameObject target, string methodName)
    {
        SendMessageOptions options = SendMessageOptions.RequireReceiver;
        object value = null;
        SendMessageTo(target, methodName, value, options);
    }

    public void  SendMessageTo(GameObject target, string methodName, SendMessageOptions options)
    {
        SendMessageTo(target, methodName, null, options);
    }

    public void  SendMessageTo(GameObject target, string methodName, object value)
    {
        SendMessageOptions options = SendMessageOptions.RequireReceiver;
        SendMessageTo(target, methodName, value, options);
    }

    public void  SendMessageTo(GameObject target, string methodName, [DefaultValue ("null")] object value, [DefaultValue ("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
    {
        Debug.Log(string.Format("Sending message '{0}' from {1}({2}) to {3}({4}): {0}, {5}", methodName, this.name, this.GetType().Name, target.name, target.GetType().Name, value));
        target.SendMessage(methodName, value, options);
    }

}
