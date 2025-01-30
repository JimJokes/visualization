using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadBuilder : MonoBehaviour
{
    private List<string> instantiated_roads = new List<string>();
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
            List<string> roads_to_not_remove = new List<string>();

            foreach (Road road in backend.map.roads)
            {
                roads_to_not_remove.Add(road.uid);
                if (instantiated_roads.Contains(road.uid))
                {
                    continue;
                }
                
                GameObject road_object = new GameObject("Road " + road.uid);
                road_object.AddComponent<RoadHandler>();
                road_object.GetComponent<RoadHandler>().road = road;
                
                Vector3 average = new Vector3(0, 0, 0);
                for (int i = 0; i < road.points.Length; i++)
                {
                    average += road.points[i].ToVector3();
                }
                road_object.transform.position = average / road.points.Length;

                for(int i = 0; i < road.lanes.Length; i++)
                {
                    bool is_left = i < road.road_look.lanes_left.Length;
                    // Lane Base
                    Lane lane = road.lanes[i];
                    Mesh mesh = new Mesh();
                    if (is_left)
                    {
                        mesh = lane.CreateMeshAlongPoints(left_shoulder: road.road_look.shoulder_space_left, right_shoulder: 0);
                    }
                    else
                    {
                        mesh = lane.CreateMeshAlongPoints(right_shoulder: road.road_look.shoulder_space_right, left_shoulder: 0);
                    }
                    //mesh = lane.CreateMeshAlongPoints(left_shoulder: 0, right_shoulder: 0);
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
                    if (left_marking == RoadMarkingType.DASHED || left_marking == RoadMarkingType.DASHED_SHORT)
                    {
                        marking_mesh_renderer.material = dashed_marking_material;
                        marking_mesh_renderer.material.SetFloat("_length", road.length);
                        if (left_marking == RoadMarkingType.DASHED_SHORT)
                        {
                            marking_mesh_renderer.material.SetFloat("_dashes_per_meter", 0.3f);
                        }
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
                    if (right_marking == RoadMarkingType.DASHED || right_marking == RoadMarkingType.DASHED_SHORT)
                    {
                        marking_mesh_renderer.material = dashed_marking_material;
                        marking_mesh_renderer.material.SetFloat("_length", road.length);
                        if (right_marking == RoadMarkingType.DASHED_SHORT)
                        {
                            marking_mesh_renderer.material.SetFloat("_dashes_per_meter", 0.3f);
                        }
                    }
                    else
                    {
                        marking_mesh_renderer.material = solid_marking_material;
                    }
                    marking_object.transform.parent = lane_object.transform;
                    marking_object.transform.parent = lane_object.transform;
                }

                road_object.transform.parent = this.transform;
                road_object.AddComponent<StaticObject>();
                road_object.GetComponent<StaticObject>().position = road_object.transform.position;
                instantiated_roads.Add(road.uid);
            }

            List<string> roads_to_remove = new List<string>();

            foreach (string road in instantiated_roads)
            {
                if (!roads_to_not_remove.Contains(road))
                {
                    roads_to_remove.Add(road);
                }
            }

            foreach (string road in roads_to_remove)
            {
                Destroy(GameObject.Find("Road " + road));
                instantiated_roads.Remove(road);
            }
        } 
        else
        {
            foreach (string road in instantiated_roads)
            {
                Destroy(GameObject.Find("Road " + road));
            }
        }
    }
}
