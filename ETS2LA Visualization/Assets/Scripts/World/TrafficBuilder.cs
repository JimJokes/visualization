using UnityEngine;

public class TrafficBuilder : MonoBehaviour
{

    public GameObject vehiclePrefab;
    public GameObject trailerPrefab;
    public int vehicleCount = 20;
    public int trailerCount = 40;
    public GameObject[] vehicles;
    public GameObject[] trailers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailers = new GameObject[trailerCount];
        for (int i = 0; i < trailerCount; i++)
        {
            trailers[i] = Instantiate(trailerPrefab, new Vector3(i * 10, 0, 0), Quaternion.identity);
            trailers[i].name = "Trailer " + i + " truck " + i / 2;
            trailers[i].transform.parent = transform;
        }
        vehicles = new GameObject[vehicleCount];
        for (int i = 0; i < vehicleCount; i++)
        {
            vehicles[i] = Instantiate(vehiclePrefab, new Vector3(i * 10, 0, 0), Quaternion.identity);
            vehicles[i].name = "Vehicle " + i;
            vehicles[i].transform.parent = transform;
            vehicles[i].GetComponent<Vehicle>().id = i;
            vehicles[i].GetComponent<Vehicle>().trailers = new GameObject[2];
            vehicles[i].GetComponent<Vehicle>().trailers[0] = trailers[i * 2];
            vehicles[i].GetComponent<Vehicle>().trailers[1] = trailers[i * 2 + 1];
        }
    }
}
