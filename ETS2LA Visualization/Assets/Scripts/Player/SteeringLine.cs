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
        if (backend.truck.steering == null)
        {
            return;
        }
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        
        if(backend.truck.steering != last_data)
        {
            last_data = backend.truck.steering;
            lineRenderer.positionCount = last_data.Length;

            for (int i = 0; i < last_data.Length; i++)
            {
                lineRenderer.SetPosition(i, last_data[i] + Vector3.up * 0.2f);
            }
        }
    }
}
