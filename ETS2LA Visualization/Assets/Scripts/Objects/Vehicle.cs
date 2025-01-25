using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Vehicle : MonoBehaviour
{
    public GameObject[] trailers;
    public BackendSocket backend;
    public int id;

    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
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
        if (backend.world.traffic.Length <= id)
        {
            return;
        }

        VehicleClass self = backend.world.traffic[id];

        Vector3 target_position = new Vector3(self.position.z, self.position.y + self.size.height / 2, self.position.x);
        if (Vector3.Distance(target_position, transform.position) > 4.5)
        {
            transform.position = target_position;
            Material material = GetComponent<Renderer>().material;
            material.SetFloat("Opacity", 0);
            material.DOFloat(1, "Opacity", 1);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target_position, Time.deltaTime * 30);
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
            if (Vector3.Distance(target_trailer_position, trailers[i].transform.position) > 5)
            {
                trailers[i].transform.position = target_trailer_position;
                Material material = trailers[i].GetComponent<Renderer>().material;
                material.SetFloat("Opacity", 0);
                material.DOFloat(1, "Opacity", 1);
            }
            else
            {
                trailers[i].transform.position = Vector3.Lerp(trailers[i].transform.position, target_trailer_position, Time.deltaTime * 2);
            }

            trailers[i].transform.position = target_trailer_position;
            trailers[i].transform.eulerAngles = new Vector3(self.trailers[i].rotation.pitch, -self.trailers[i].rotation.yaw + 90, self.trailers[i].rotation.roll);
            trailers[i].transform.localScale = new Vector3(self.trailers[i].size.width, self.trailers[i].size.height, self.trailers[i].size.length);
        }
    }
}