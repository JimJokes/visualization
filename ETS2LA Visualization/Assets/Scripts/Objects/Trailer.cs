using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Trailer : MonoBehaviour
{
    public int uid;
    public BackendSocket backend;

    void Update()
    {
        if(uid == 0)
        {
            return;
        }
        if(backend == null)
        {
            backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
        }
        if(backend.world != null && backend.world.highlights != null && backend.world.highlights.vehicles != null && backend.world.highlights.vehicles.Contains(uid))
        {
            Material material = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            material.color = new Color(0.5f, 0.9f, 1.0f);
            material = transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material;
            material.color = new Color(0.5f, 0.9f, 1.0f);
        }    
        else
        {
            Material material = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
            material.color = new Color(0.5f, 0.5f, 0.5f);
            material = transform.GetChild(1).GetChild(0).GetComponent<MeshRenderer>().material;
            material.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}