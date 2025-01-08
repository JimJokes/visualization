using UnityEngine;

[System.Serializable]
public class Lane
{
    public Point[] points;
    public string side;
    public float length;

    public Mesh CreateMeshAlongPoints(float lane_width = 4.5f)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Length * 2];
        int[] triangles = new int[(points.Length - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i].ToVector3();
            Vector3 direction;

            if (i < points.Length - 1)
            {
                direction = (points[i + 1].ToVector3() - point).normalized;
            }
            else
            {
                direction = (point - points[i - 1].ToVector3()).normalized;
            }

            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 tangent = direction;

            vertices[i * 2] = point + normal * lane_width / 2;
            vertices[i * 2 + 1] = point - normal * lane_width / 2;
            uv[i * 2] = new Vector2(0, (float)i / (points.Length - 1));
            uv[i * 2 + 1] = new Vector2(1, (float)i / (points.Length - 1));
            normals[i * 2] = normals[i * 2 + 1] = Vector3.up;
            tangents[i * 2] = tangents[i * 2 + 1] = new Vector4(tangent.x, tangent.y, tangent.z, 1);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;
            triangles[i * 6 + 3] = i * 2 + 1;
            triangles[i * 6 + 4] = i * 2 + 3;
            triangles[i * 6 + 5] = i * 2 + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}

[System.Serializable]
public class RoadLook
{
    public string token;
    public string name;
    public string[] lanes_left;
    public string[] lanes_right;
    public float offset;
    public float lane_offset;
    public float shoulder_space_left;
    public float shoulder_space_right;
}

[System.Serializable]
public class Road
{
    public int uid;
    public int type;
    public int x;
    public int y;
    public int sector_x;
    public int sector_y;
    public int dlc_guard;
    public bool hidden;
    public RoadLook road_look;
    public int start_node_uid;
    public int end_node_uid;
    public float length;
    public bool maybe_divided;
    public Point[] points;
    public Lane[] lanes;
}