using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PrefabBuilder : MonoBehaviour
{
    private List<int> instantiated_prefabs = new List<int>();
    public BackendWebrequests backend;
    public Material base_material;
    public float parallel_threshold = 0.5f; // meters
    public Material solid_marking_material;
    public Material dashed_marking_material;

    RoadMarkingType[] GetMarkingsForLane(Prefab prefab, int route_index)
    {
        NavRoute route = prefab.nav_routes[route_index];
        if (route == null)
        {
            return new RoadMarkingType[] { RoadMarkingType.NONE, RoadMarkingType.NONE };
        }
        if (route.points == null || route.points.Length == 0)
        {
            return new RoadMarkingType[] { RoadMarkingType.NONE, RoadMarkingType.NONE };
        }
        
        NavRoute next_route = null;
        NavRoute prev_route = null;

        Vector2 route_vector = new Vector2(route.points[route.points.Length - 1].x - route.points[0].x, route.points[route.points.Length - 1].z - route.points[0].z);
        
        RoadMarkingType left_marking = RoadMarkingType.NONE;
        RoadMarkingType right_marking = RoadMarkingType.NONE;

        if (route_index > 0)
        {
            try { prev_route = prefab.nav_routes[route_index - 1]; } catch { }
        }
        if (route_index < prefab.nav_routes.Length - 1)
        {
            try { next_route = prefab.nav_routes[route_index + 1]; } catch { }
        }

        // Check next route
        if (next_route != null)
        {
            if (next_route.points == null || next_route.points.Length == 0)
            {
                return new RoadMarkingType[] { RoadMarkingType.NONE, RoadMarkingType.NONE };
            }

            float next_start_distance = Vector3.Distance(route.points[0].ToVector3(), next_route.points[0].ToVector3());
            float next_end_distance = Vector3.Distance(route.points[route.points.Length - 1].ToVector3(), next_route.points[next_route.points.Length - 1].ToVector3());

            Vector2 next_vector = new Vector2(next_route.points[next_route.points.Length - 1].x - next_route.points[0].x, next_route.points[next_route.points.Length - 1].z - next_route.points[0].z);

            bool is_left = math.dot(route_vector, next_vector) < 0;

            if(math.abs(next_start_distance - next_end_distance) < parallel_threshold)
            {
                if(is_left)
                {
                    left_marking = RoadMarkingType.DASHED;
                }
                else
                {
                    right_marking = RoadMarkingType.DASHED;
                }
            }
            else
            {
                if(is_left)
                {
                    left_marking = RoadMarkingType.NONE;
                }
                else
                {
                    right_marking = RoadMarkingType.NONE;
                }
            }
        }

        // Check previous route
        if (prev_route != null)
        {
            if (prev_route.points == null || prev_route.points.Length == 0)
            {
                return new RoadMarkingType[] { RoadMarkingType.NONE, RoadMarkingType.NONE };
            }

            float prev_start_distance = Vector3.Distance(route.points[0].ToVector3(), prev_route.points[0].ToVector3());
            float prev_end_distance = Vector3.Distance(route.points[route.points.Length - 1].ToVector3(), prev_route.points[prev_route.points.Length - 1].ToVector3());

            Vector2 prev_vector = new Vector2(prev_route.points[prev_route.points.Length - 1].x - prev_route.points[0].x, prev_route.points[prev_route.points.Length - 1].z - prev_route.points[0].z);

            bool is_left = math.dot(route_vector, prev_vector) < 0;

            if(math.abs(prev_start_distance - prev_end_distance) < parallel_threshold)
            {
                if(is_left)
                {
                    left_marking = RoadMarkingType.DASHED;
                }
                else
                {
                    right_marking = RoadMarkingType.DASHED;
                }
            }
            else
            {
                if(is_left)
                {
                    left_marking = RoadMarkingType.NONE;
                }
                else
                {
                    right_marking = RoadMarkingType.NONE;
                }
            }
        }

        return new RoadMarkingType[] { left_marking, right_marking };
    }

    float GetRouteLength(Point[] points)
    {
        float length = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            length += Vector3.Distance(points[i].ToVector3(), points[i + 1].ToVector3());
        }
        return length;
    }

    void Update()
    {
        if (backend.prefabs_count > 0)
        {
            List<int> prefabs_to_not_remove = new List<int>();

            foreach (Prefab prefab in backend.map.prefabs)
            {
                prefabs_to_not_remove.Add(prefab.uid);
                if (instantiated_prefabs.Contains(prefab.uid))
                {
                    continue;
                }
                
                GameObject prefab_object = new GameObject("Prefabs " + prefab.uid.ToString());

                for(int i = 0; i < prefab.nav_routes.Length; i++)
                {
                    NavRoute route = prefab.nav_routes[i];
                    Mesh mesh = route.CreateMeshAlongPoints();
                    GameObject route_object = new GameObject("Route " + i.ToString());
                    route_object.AddComponent<MeshFilter>().mesh = mesh;
                    MeshRenderer mesh_renderer = route_object.AddComponent<MeshRenderer>();
                    mesh_renderer.material = base_material;
                    mesh_renderer.material.color = new Color(35/255, 35/255, 40/255);;
                    route_object.transform.parent = prefab_object.transform;

                    // Which markings to use
                    RoadMarkingType[] markings = GetMarkingsForLane(prefab, i);
                    RoadMarkingType left_marking = markings[0];
                    RoadMarkingType right_marking = markings[1];

                    // Left Side Marking
                    if (left_marking != RoadMarkingType.NONE)
                    {
                        Mesh marking_mesh = route.CreateMarkingMesh(Side.LEFT, left_marking);
                        GameObject marking_object = new GameObject("Marking Left " + i.ToString() + " " + left_marking.ToString());
                        marking_object.AddComponent<MeshFilter>().mesh = marking_mesh;
                        MeshRenderer marking_mesh_renderer = marking_object.AddComponent<MeshRenderer>();
                        if (left_marking == RoadMarkingType.DASHED)
                        {
                            marking_mesh_renderer.material = dashed_marking_material;
                            marking_mesh_renderer.material.SetFloat("_length", GetRouteLength(route.points));
                        }
                        else
                        {
                            marking_mesh_renderer.material = solid_marking_material;
                        }
                        marking_object.transform.parent = route_object.transform;
                    }
                
                    // Right Side Marking
                    if (right_marking != RoadMarkingType.NONE) {
                        Mesh marking_mesh = route.CreateMarkingMesh(Side.RIGHT, right_marking);
                        GameObject marking_object = new GameObject("Marking Right " + i.ToString() + " " + right_marking.ToString());
                        marking_object.AddComponent<MeshFilter>().mesh = marking_mesh;
                        MeshRenderer marking_mesh_renderer = marking_object.AddComponent<MeshRenderer>();
                        if (right_marking == RoadMarkingType.DASHED)
                        {
                            marking_mesh_renderer.material = dashed_marking_material;
                            marking_mesh_renderer.material.SetFloat("_length", GetRouteLength(route.points));
                        }
                        else
                        {
                            marking_mesh_renderer.material = solid_marking_material;
                        }
                        marking_object.transform.parent = route_object.transform;
                    }
                }

                prefab_object.transform.parent = this.transform;
                instantiated_prefabs.Add(prefab.uid);
            }

            foreach (int road in instantiated_prefabs)
            {
                if (!prefabs_to_not_remove.Contains(road))
                {
                    Destroy(GameObject.Find("Prefabs " + road));
                    instantiated_prefabs.Remove(road);
                }
            }
        } 
        else
        {
            foreach (int prefab_uid in instantiated_prefabs)
            {
                Destroy(GameObject.Find("Prefabs " + prefab_uid.ToString()));
            }
        }
    }
}
