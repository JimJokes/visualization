using UnityEngine;

[System.Serializable]
public class ModelDescription
{
    public string token;
    public Point center;
    public Point start;
    public Point end;
    public float height;
    public float width;
    public float length;
}

[System.Serializable]
public class Model
{
    public string uid;
    public int type;
    public float x;
    public float y;
    public int sector_x;
    public int sector_y;
    public float z;
    public float rotation;
    public string token;
    public string node_uid;
    public Point scale;
    public ModelDescription description;
}