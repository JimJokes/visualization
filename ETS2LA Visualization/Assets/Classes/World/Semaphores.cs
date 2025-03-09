using UnityEngine;

[System.Serializable]
public class Semaphore
{
    public GamePosition position;
    public float cx;
    public float cy;
    public GameQuaternion rotation;
    public string type;
    public float time_left;
    public int state;
    public int id;
    public string uid;
}

[System.Serializable]
public class TrafficLight : Semaphore
{
    public string state_text;
    public int[] color;
}

[System.Serializable]
public class Gate : Semaphore
{
    public string state_text;
}