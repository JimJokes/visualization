using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Vehicle : MonoBehaviour
{
    public GameObject[] trailers;
    public BackendSocket backend;
    public TrafficBuilder trafficBuilder;
    public int uid;
    private string type = "car";

    public GameObject car;
    public GameObject van;
    public GameObject bus;
    public GameObject truck;

    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
        trafficBuilder = GameObject.Find("Traffic").GetComponent<TrafficBuilder>();
    }

    void EnableChild(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    void ColorChild(int index, Color color)
    {
        GameObject child = transform.GetChild(index).gameObject;
        if (child.GetComponent<MeshRenderer>() == null)
        {
            child = child.transform.GetChild(0).gameObject;
        }
        Material material = child.GetComponent<MeshRenderer>().material;
        material.color = color;
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

        int[] uids = new int[backend.world.traffic.Length];
        for (int i = 0; i < backend.world.traffic.Length; i++)
        {
            uids[i] = backend.world.traffic[i].id;
        }

        if (!System.Array.Exists(uids, element => element == uid))
        {
            trafficBuilder.RemoveVehicle(uid);
            return;
        }

        VehicleClass self = backend.world.traffic[System.Array.FindIndex(uids, element => element == uid)];

        Vector3 target_position = new Vector3(self.position.z, self.position.y + self.size.height / 2, self.position.x);
        Vector3 truck_position = new Vector3(backend.truck.transform.z, backend.truck.transform.y, backend.truck.transform.x);
        if(Vector3.Distance(transform.position, target_position) > 5f)
        {
            transform.position = target_position;
            Material material = GetComponent<Renderer>().material;
            material.SetFloat("Opacity", 0);
            material.DOFloat(1, "Opacity", 1);
        }
        else
        {
            transform.DOMove(target_position, 0.25f).SetLink(gameObject);
        }
        
        transform.localEulerAngles = new Vector3(self.rotation.pitch, -self.rotation.yaw + 90, self.rotation.roll);
        transform.localScale = new Vector3(self.size.width, self.size.height, self.size.length);

        int trailer_count = self.trailer_count;
        for(int i = 0; i < 2; i++)
        {
            if (i >= trailer_count)
            {
                trailers[i].SetActive(false);
                continue;
            }

            trailers[i].SetActive(true);
            trailers[i].GetComponent<Trailer>().uid = uid;
            trailers[i].GetComponent<Trailer>().backend = backend;

            Vector3 target_trailer_position = new Vector3(self.trailers[i].position.z, self.trailers[i].position.y + self.trailers[i].size.height / 2, self.trailers[i].position.x);
            if (Vector3.Distance(trailers[i].transform.position, target_trailer_position) > 5f)
            {
                trailers[i].transform.position = target_trailer_position;
                Material material = trailers[i].GetComponent<Renderer>().material;
                material.SetFloat("Opacity", 0);
                material.DOFloat(1, "Opacity", 1);
            }
            else
            {
                trailers[i].transform.DOMove(target_trailer_position, 0.25f).SetLink(gameObject);
            }
            trailers[i].transform.eulerAngles = new Vector3(self.trailers[i].rotation.pitch, -self.trailers[i].rotation.yaw + 90, self.trailers[i].rotation.roll);
            trailers[i].transform.localScale = new Vector3(self.trailers[i].size.width, self.trailers[i].size.height, self.trailers[i].size.length);
        }

        if(trailer_count != 0 && self.size.height > 2)
        {
            type = "truck";
        }
        else if (self.size.length > 8)
        {
            type = "bus";
        }
        else if (self.size.height > 1.8)
        {
            type = "van";
        }
        else
        {
            type = "car";
        }

        float distance = Vector3.Distance(truck_position, target_position);

        switch (type)
        {
            case "car":
                EnableChild(0);
                if(backend.world.highlights != null && backend.world.highlights.vehicles.Contains(uid) && distance < 100)
                    ColorChild(0, new Color(0.5f, 0.9f, 1.0f));
                else
                    ColorChild(0, Color.white);

                break;
            case "van":
                EnableChild(1);
                if(backend.world.highlights != null && backend.world.highlights.vehicles.Contains(uid) && distance < 100)
                    ColorChild(1, new Color(0.5f, 0.9f, 1.0f));
                else
                    ColorChild(1, Color.white);

                break;
            case "bus":
                EnableChild(2);
                if(backend.world.highlights != null && backend.world.highlights.vehicles.Contains(uid) && distance < 100)
                    ColorChild(2, new Color(0.5f, 0.9f, 1.0f));
                else
                    ColorChild(2, Color.white);

                break;
            case "truck":
                EnableChild(3);
                if(backend.world.highlights != null && backend.world.highlights.vehicles.Contains(uid) && distance < 100)
                    ColorChild(3, new Color(0.5f, 0.9f, 1.0f));
                else
                    ColorChild(3, Color.white);

                break;
        }
    }
}