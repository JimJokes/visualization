using System.Linq;
using UnityEngine;

public class PrefabHandler : MonoBehaviour
{
    public GameObject[] lanes;
    public Vector3[] lane_positions;
    public BackendSocket backend;
    public string uid;

    void Start()
    {
        uid = gameObject.name.Replace("Prefab ", "");
        lanes = new GameObject[transform.childCount];
        lane_positions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            lanes[i] = transform.GetChild(i).gameObject;
            lane_positions[i] = lanes[i].transform.position;
            lanes[i].GetComponent<Renderer>().material.color = new Color(0.04f, 0.04f, 0.042f);
        }

        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
    }

    void Update()
    {
        if (backend.world == null){
            return;
        }
        if (backend.world.route_information == null){
            return;
        }
        foreach (RouteInformation information in backend.world.route_information)
        {
            if (information.uids.Contains(uid))
            {

                if(lanes.Length == 0)
                {
                    return;
                }

                if(lanes.Length == 2)
                {
                    if(Vector3.Distance(lane_positions[0], lane_positions[1]) < 4.6f)
                    {
                        lanes[0].transform.position = lane_positions[0] + Vector3.left * 0.5f;
                        lanes[1].transform.position = lane_positions[1] + Vector3.right * 0.5f;
                        lanes[0].GetComponent<Renderer>().material.color = new Color(0.04f, 0.04f, 0.042f);
                        lanes[1].GetComponent<Renderer>().material.color = new Color(0.04f, 0.04f, 0.042f);
                    }
                }

                int highlighted_lane = information.lane_index;
                for (int i = 0; i < lanes.Length; i++)
                {
                    if (i == highlighted_lane)
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.04f, 0.05f, 0.076f);
                        lanes[i].transform.position = lane_positions[i] + Vector3.up * 0.02f;
                    }
                    else
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.04f, 0.04f, 0.042f);
                        lanes[i].transform.position = lane_positions[i];
                    }
                }
            }
        }
    }

}