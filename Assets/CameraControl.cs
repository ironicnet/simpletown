using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    string xAxisName = "Mouse X";
    string zAxisName = "Mouse Y";
    string yAxisName = "Mouse ScrollWheel";
    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
        var axis = Input.GetAxis(yAxisName);
        if (axis != 0) // back
        {
            
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + axis, 1f);
        }
        if (Input.GetMouseButton(2))
        {
            var xAxis = Input.GetAxis(xAxisName);
            var zAxis = Input.GetAxis(zAxisName);
            if (xAxis != 0 || zAxis != 0)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + xAxis, Camera.main.transform.position.y, Camera.main.transform.position.z + zAxis);
            }
        }
    }
}
