using Baracuda.Monitoring;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerTruck : MonitoredBehaviour
{
    [Monitor]
    private Vector3 position;
    [Monitor]
    private Vector3 rotation;
    public BackendSocket backend;
    public GameObject trailer_prefab;
    public GameObject[] connected_trailers;
    void Update()
    {
        if(backend.truck.transform != null)
        {
            transform.DOMove(new Vector3(
                backend.truck.transform.z, 
                backend.truck.transform.y, 
                backend.truck.transform.x
            ), 0.2f);

            transform.DORotateQuaternion(Quaternion.Euler(
                -backend.truck.transform.ry, 
                -backend.truck.transform.rx - 90, 
                backend.truck.transform.rz
            ), 0.2f);

            position = transform.position;
            rotation = transform.rotation.eulerAngles;
        }

        if(backend.truck.trailers != null)
        {
            for(int i = 0; i < backend.truck.trailers.Length; i++)
            {
                if(connected_trailers.Length < backend.truck.trailers.Length)
                {
                    if(connected_trailers.Length > 0)
                    {
                        for (int j = 0; j < connected_trailers.Length; j++)
                        {
                            Destroy(connected_trailers[j]);
                        }
                    }

                    connected_trailers = new GameObject[backend.truck.trailers.Length];
                    for (int j = 0; j < backend.truck.trailers.Length; j++)
                    {
                        connected_trailers[j] = Instantiate(trailer_prefab);
                        connected_trailers[j].transform.GetChild(0).localScale = new Vector3(1, 1.33f, 2);
                    }
                }

                GameObject trailer = connected_trailers[i];
                GameObject trailer_hitch = trailer.transform.GetChild(0).Find("trailer_hitch").gameObject;

                Vector3 api_hook = new Vector3(
                    backend.truck.trailers[i].hook_z, 
                    backend.truck.trailers[i].hook_y, 
                    backend.truck.trailers[i].hook_x
                );

                float distance = api_hook.x + trailer_hitch.transform.localPosition.x;
                Vector3 trailer_offset = trailer.transform.forward * distance;

                Vector3 api_position = new Vector3(
                    backend.truck.trailers[i].z, 
                    backend.truck.trailers[i].y + 1, 
                    backend.truck.trailers[i].x
                );
                trailer.transform.DOMove(api_position + trailer_offset, 0.2f);

                Vector3 api_rotation = new Vector3(
                    backend.truck.trailers[i].ry, 
                    -backend.truck.trailers[i].rx + 90, 
                    -backend.truck.trailers[i].rz
                );
                trailer.transform.DORotate(api_rotation, 0.2f);
            }
        }
    }
}
