using System.Linq;
using UnityEngine;

public class RoadHandler : MonoBehaviour
{
    public GameObject[] lanes;
    public BackendSocket backend;
    public string uid;   
    public Road road;

    void Start()
    {
        uid = gameObject.name.Replace("Road ", "");
        lanes = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            lanes[i] = transform.GetChild(i).gameObject;
            lanes[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
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
                int highlighted_lane = information.lane_index;
                int left_lanes = road.road_look.lanes_left.Length;
                if(highlighted_lane < left_lanes)
                { // Highlight all indeces until left_lanes (all the lanes on the left)
                    for (int i = 0; i < left_lanes; i++)
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.23f, 0.3f);
                    }
                    for (int i = left_lanes; i < lanes.Length; i++)
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
                    }
                }
                else
                { // Highlight all indeces after left_lanes (all the lanes on the right)
                    for (int i = left_lanes; i < lanes.Length; i++)
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.23f, 0.3f);
                    }
                    for (int i = 0; i < left_lanes; i++)
                    {
                        lanes[i].GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
                    }
                }
            }
        }
    }

}