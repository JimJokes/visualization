using System.Collections.Generic;
using UnityEngine;

public class PrefabBuilder : MonoBehaviour
{
    private List<int> instantiated_prefabs = new List<int>();
    public BackendWebrequests backend;
    public Material base_material;

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
