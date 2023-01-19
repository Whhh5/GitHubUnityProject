using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Gemeotry
{
    None = 0,
    Box = 1,
    Sphere = 2,
    Capsule = 4,
    Cylinder = 8,
    Plane = 16,
    Circle = 32,
    Count = 64,
}
public enum E_MeshSize
{
    None,
    Normal,
    Random,
    Count,
}
public enum E_Origin
{
    One = 1,
    Zero = 0,
    Contrary_One = -1,
    Custom = 20,
}

public class CreateGameObject : MonoBehaviour
{
    public string _documentation = "";
    public bool _isButton_Play = false;
    public E_Gemeotry _gemeotryType;
    public E_MeshSize _meshSize;
    public E_Origin _originX;
    public E_Origin _originY;
    public E_Origin _originZ;
    public Vector3 _offsetXYZ;


    public bool _isLink = true;
    public long _meshCount = 0;
    public long _radius = 0;
    public long _width = 0;
    public long _height = 0;

    #region Button
    public void Button_Create()
    {
        if (_isButton_Play)
        {
            Log("Please waiting create finish ......");
            return;
        }


        _isButton_Play = true;
        Log("Button_Create ......");

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        if (!TryGetComponent<MeshFilter>(out var filter))
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }
        if (!TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }




        switch (_originX)
        {
            case E_Origin.Custom:
                //offset.x = _offsetXYZ.x;
                break;
            default:
                _offsetXYZ.x = (int)_originX;
                break;
        }
        switch (_originY)
        {
            case E_Origin.Custom:
                //offset.y = _offsetXYZ.y;
                break;
            default:
                _offsetXYZ.y = (int)_originY;
                break;
        }
        switch (_originZ)
        {
            case E_Origin.Custom:
                //offset.z = _offsetXYZ.z;
                break;
            default:
                _offsetXYZ.z = (int)_originZ;
                break;
        }
        switch (_gemeotryType)
        {
            case E_Gemeotry.None:
                break;
            case E_Gemeotry.Box:
                break;
            case E_Gemeotry.Sphere:
                break;
            case E_Gemeotry.Capsule:
                break;
            case E_Gemeotry.Cylinder:
                break;
            case E_Gemeotry.Plane:
                StartCoroutine(Create_Plane(filter, mesh, vertices, triangles, normals, _offsetXYZ));
                break;
            case E_Gemeotry.Circle:
                break;
            case E_Gemeotry.Count:
                break;
            default:
                break;
        }

    }

    public void Button_Pause()
    {
        Log("Button_Pause");

        Log("Button_Pause finish ......");
    }
    public void Button_ClearUp()
    {
        Log("Button_ClearUp ......");
        if (!_isButton_Play)
        {
            return;
        }

        StopAllCoroutines();

        Log("Button_ClearUp Finish ......");
        _isButton_Play = false;
    }
    #endregion


    private IEnumerator Create_Plane(MeshFilter filter, Mesh mesh,
        List<Vector3> vertices, List<int> triangles, List<Vector3> normals, Vector3 offset)
    {
        yield return new WaitForSeconds(0.0f);

        var radius = _radius + 1;
        var width = _width + 1;

        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> verMatrix = new List<Vector3>();
        // ���ɶ�������
        ForTwo(width, radius, (line, row) =>
        {
            // ���ö���
            var x = (float)(row + (offset.x / 2 - 0.5) * (radius - 1));
            var y = (float)(0 + (offset.y / 2 - 0.5) * 0);
            var z = (float)(line + (offset.z / 2 - 0.5) * (width - 1));
            var ver = new Vector3(x, y, z);
            vertices.Add(ver);
            // ����������
            var index = line * radius + row;

            if (row + 1 < radius && line + 1 < width)
            {
                triangles.Add((int)index);
                triangles.Add((int)(index + radius));
                triangles.Add((int)(index + radius + 1));

                triangles.Add((int)(index + radius + 1));
                triangles.Add((int)(index + 1));
                triangles.Add((int)index);
            }
            // ���÷���
            normals.Add(Vector3.up);
            var uv = new Vector2((float)row / radius, (float)line / width);
            Log(uv.x, uv.y);
            uvs.Add(uv);
        });

        // ���ò���
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);

        filter.mesh = mesh;

        Log($"������ {vertices.Count} �����㣬{triangles.Count / 3} �������Σ�{normals.Count}    ������");
        Log("Button_Create Finish ......");
        _isButton_Play = false;
    }




    #region Public  Function
    private void Log(params object[] param)
    {
        var log = $"\t<color=#00FFFFFF>{GetType()}</color>\n";

        foreach (var item in param)
        {
            log += $"\t{item.ToString()}";
        }
        Debug.Log(log);
    }
    delegate void Del_ForTwo(long i, long j);
    private void ForTwo(long iNum, long jNum, Del_ForTwo callback)
    {
        for (long i = 0; i < iNum; i++)
        {
            for (long j = 0; j < jNum; j++)
            {
                callback.Invoke(i, j);
            }
        }
    }
    #endregion
}
