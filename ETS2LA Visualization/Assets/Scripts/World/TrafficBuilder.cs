using System.Collections.Generic;
using UnityEngine;

public class TrafficBuilder : MonoBehaviour
{

    public BackendSocket backend;
    public GameObject vehiclePrefab;
    public GameObject trailerPrefab;
    public int trailerCountPerVehicle = 2;
    public List<GameObject> vehicles = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
    }

    void SpawnVehicle(int uid)
    {
        GameObject vehicle = Instantiate(vehiclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        vehicle.name = "Vehicle " + uid;
        vehicle.transform.parent = transform;
        vehicle.GetComponent<Vehicle>().uid = uid;
        vehicle.GetComponent<Vehicle>().trailers = new GameObject[2];
        vehicle.GetComponent<Vehicle>().trailers[0] = Instantiate(trailerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        vehicle.GetComponent<Vehicle>().trailers[0].name = "Trailer " + uid + " 0";
        vehicle.GetComponent<Vehicle>().trailers[0].transform.parent = transform;
        vehicle.GetComponent<Vehicle>().trailers[1] = Instantiate(trailerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        vehicle.GetComponent<Vehicle>().trailers[1].name = "Trailer " + uid + " 1";
        vehicle.GetComponent<Vehicle>().trailers[1].transform.parent = transform;
        
        vehicles.Add(vehicle);
    }

    public void RemoveVehicle(int uid)
    {
        GameObject vehicle = GameObject.Find("Vehicle " + uid);
        Destroy(vehicle);
        GameObject trailer0 = GameObject.Find("Trailer " + uid + " 0");
        Destroy(trailer0);
        GameObject trailer1 = GameObject.Find("Trailer " + uid + " 1");
        Destroy(trailer1);
        vehicles.Remove(vehicle);
    }

    void Update()
    {
        if (backend.world == null)
        {
            return;
        }
        if (backend.world.traffic == null)
        {
            return;
        }

        int[] uids = new int[vehicles.Count];
        for (int i = 0; i < vehicles.Count; i++)
        {
            uids[i] = vehicles[i].GetComponent<Vehicle>().uid;
        }

        foreach (VehicleClass vehicle in backend.world.traffic)
        {
            if (!System.Array.Exists(uids, element => element == vehicle.id))
            {
                SpawnVehicle(vehicle.id);
            }
        }
    }
}
