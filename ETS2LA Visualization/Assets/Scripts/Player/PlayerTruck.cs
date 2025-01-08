using Baracuda.Monitoring;
using UnityEngine;

public class PlayerTruck : MonitoredBehaviour
{
    [Monitor]
    private Vector3 position;
    [Monitor]
    private Vector3 rotation;
    public BackendSocket backend;

    void Update()
    {
        if(backend.truck.transform != null)
        {
            transform.position = new Vector3(
                backend.truck.transform.z, 
                backend.truck.transform.y, 
                backend.truck.transform.x
            );
            transform.rotation = Quaternion.Euler(
                backend.truck.transform.ry, 
                -backend.truck.transform.rx - 90, 
                backend.truck.transform.rz
            );

            position = transform.position;
            rotation = transform.rotation.eulerAngles;
        }
    }
}
