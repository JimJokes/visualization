using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    private List<int> instantiated_roads = new List<int>();
    public BackendWebrequests backend;
    public Material base_material;
    public Material solid_marking_material;
    public Material dashed_marking_material;

    RoadMarkingType[] GetMarkingsForLane(Road road, int lane_index)
    {
        Lane lane = road.lanes[lane_index];
        string[] lane_types = road.road_look.lanes_left.Concat(road.road_look.lanes_right).ToArray();
        string lane_type = lane_types[lane_index];

        Lane left_lane = null;
        string left_lane_type = null;

        Lane right_lane = null;
        string right_lane_type = null;

        RoadMarkingType left_marking = RoadMarkingType.SOLID;
        RoadMarkingType right_marking = RoadMarkingType.SOLID;

        if (lane_index > 0)
        {
            try { right_lane_type = lane_types[lane_index - 1]; right_lane = road.lanes[lane_index - 1]; } catch { }
        }
        if (lane_index < road.lanes.Length - 1)
        {
            try { left_lane_type = lane_types[lane_index + 1]; left_lane = road.lanes[lane_index + 1]; } catch { }
        }

        if (left_lane_type != null && !left_lane_type.Contains("no_overtake") && left_lane.side == lane.side)
        {
            if(left_lane_type != lane_type)
            {
                left_marking = RoadMarkingType.DASHED_SHORT;
            }
            else
            {
                left_marking = RoadMarkingType.DASHED;
            }
        }
        else if (left_lane_type != null && left_lane_type.Contains("no_overtake") && left_lane.side == lane.side)
        {
            left_marking = RoadMarkingType.DOUBLE;
        }

        
        if (right_lane_type != null && !right_lane_type.Contains("no_overtake") && right_lane.side == lane.side)
        {
            if(right_lane_type != lane_type)
            {
                right_marking = RoadMarkingType.DASHED_SHORT;
            }
            else
            {
                right_marking = RoadMarkingType.DASHED;
            }
        }
        else if (right_lane_type != null && right_lane_type.Contains("no_overtake") && right_lane.side == lane.side)
        {
            right_marking = RoadMarkingType.DOUBLE;
        }

        return new RoadMarkingType[] { left_marking, right_marking };
    }

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
                    // Lane Base
                    Lane lane = road.lanes[i];
                    Mesh mesh = lane.CreateMeshAlongPoints(left_shoulder: road.road_look.shoulder_space_left, right_shoulder: road.road_look.shoulder_space_right);
                    GameObject lane_object = new GameObject("Lane " + i.ToString());
                    lane_object.AddComponent<MeshFilter>().mesh = mesh;
                    MeshRenderer mesh_renderer = lane_object.AddComponent<MeshRenderer>();
                    mesh_renderer.material = base_material;
                    lane_object.transform.parent = road_object.transform;

                    // What is on each side?
                    RoadMarkingType[] markings = GetMarkingsForLane(road, i);
                    RoadMarkingType left_marking = markings[0];
                    RoadMarkingType right_marking = markings[1];

                    // Left Side Marking
                    Mesh marking_mesh = lane.CreateMarkingMesh(Side.LEFT, left_marking);
                    GameObject marking_object = new GameObject("Marking Left " + i.ToString() + " " + left_marking.ToString());
                    marking_object.AddComponent<MeshFilter>().mesh = marking_mesh;
                    MeshRenderer marking_mesh_renderer = marking_object.AddComponent<MeshRenderer>();
                    if (left_marking == RoadMarkingType.DASHED)
                    {
                        marking_mesh_renderer.material = dashed_marking_material;
                        marking_mesh_renderer.material.SetFloat("_length", road.length);
                    }
                    else
                    {
                        marking_mesh_renderer.material = solid_marking_material;
                    }
                    marking_object.transform.parent = lane_object.transform;
                
                    // Right Side Marking
                    marking_mesh = lane.CreateMarkingMesh(Side.RIGHT, right_marking);
                    marking_object = new GameObject("Marking Right " + i.ToString() + " " + right_marking.ToString());
                    marking_object.AddComponent<MeshFilter>().mesh = marking_mesh;
                    marking_mesh_renderer = marking_object.AddComponent<MeshRenderer>();
                    if (right_marking == RoadMarkingType.DASHED)
                    {
                        marking_mesh_renderer.material = dashed_marking_material;
                        marking_mesh_renderer.material.SetFloat("_length", road.length);
                    }
                    else
                    {
                        marking_mesh_renderer.material = solid_marking_material;
                    }
                    marking_object.transform.parent = lane_object.transform;
                    marking_object.transform.parent = lane_object.transform;
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
