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
                backend.truck.transform.x, 
                backend.truck.transform.y, 
                backend.truck.transform.z
            );
            transform.rotation = Quaternion.Euler(
                backend.truck.transform.ry, 
                backend.truck.transform.rx + 180, 
                backend.truck.transform.rz
            );

            position = transform.position;
            rotation = transform.rotation.eulerAngles;
        }
    }
}
