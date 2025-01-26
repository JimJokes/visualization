using UnityEngine;
using DG.Tweening;

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

        switch (type)
        {
            case "car":
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                break;
            case "van":
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                break;
            case "bus":
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(true);
                transform.GetChild(3).gameObject.SetActive(false);
                break;
            case "truck":
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(true);
                break;
        }
    }
}