using System.Collections.Generic;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    private List<int> instantiated_roads = new List<int>();
    public BackendWebrequests backend;
    public Material base_material;

    void Update()
    {
        if (backend.roads_count > 0)
        {
            List<int> roads_to_not_remove = new List<int>();

            foreach (Road road in backend.map.roads)
            {
                roads_to_not_remove.Add(road.uid);
                if (instantiated_roads.Contains(road.uid))
                {
                    continue;
                }
                
                GameObject road_object = new GameObject("Road " + road.uid.ToString());

                for(int i = 0; i < road.lanes.Length; i++)
                {
                    Lane lane = road.lanes[i];
                    Mesh mesh = lane.CreateMeshAlongPoints();
                    GameObject lane_object = new GameObject("Lane " + i.ToString());
                    lane_object.AddComponent<MeshFilter>().mesh = mesh;
                    MeshRenderer mesh_renderer = lane_object.AddComponent<MeshRenderer>();
                    mesh_renderer.material = base_material;
                    mesh_renderer.material.color = new Color(35/255, 35/255, 40/255);
                    lane_object.transform.parent = road_object.transform;
                }

                road_object.transform.parent = this.transform;
                instantiated_roads.Add(road.uid);
            }

            foreach (int road in instantiated_roads)
            {
                if (!roads_to_not_remove.Contains(road))
                {
                    Destroy(GameObject.Find("Road " + road));
                    instantiated_roads.Remove(road);
                }
            }
        } 
        else
        {
            foreach (int road in instantiated_roads)
            {
                Destroy(GameObject.Find("Road " + road.ToString()));
            }
        }
    }
}
