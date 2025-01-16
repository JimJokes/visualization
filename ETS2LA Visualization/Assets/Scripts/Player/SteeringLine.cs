using UnityEngine;

public class SteeringLine : MonoBehaviour
{
    public BackendSocket backend;
    public GameObject truck;
    public Vector3[] last_data = new Vector3[0];

    // Update is called once per frame
    void Update()
    {
        if (backend.truck == null)
        {
            return;
        }
        if (backend.truck.state == null)
        {
            return;
        }
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, truck.transform.position + Vector3.up * 0.1f);
        
        if(backend.truck.steering != last_data)
        {
            last_data = backend.truck.steering;
            lineRenderer.positionCount = last_data.Length + 1;

            for (int i = 0; i < last_data.Length; i++)
            {
                lineRenderer.SetPosition(i+1, last_data[i]);
            }
        }
    }
}
