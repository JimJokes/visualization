using UnityEngine;

public class StaticObject : MonoBehaviour
{
    public Vector3 position;
    private BackendSocket backend;

    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
        position = transform.position;
    }

    void Update()
    {
        if (backend.truck == null)
        {
            return;
        }
        if (backend.truck.transform == null)
        {
            return;
        }

        transform.position = new Vector3(
            position.x - backend.truck.transform.sector_y,
            position.y,
            position.z - backend.truck.transform.sector_x
        );
    }
}