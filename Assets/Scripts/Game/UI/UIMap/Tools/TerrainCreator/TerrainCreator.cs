using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCreator : MonoBehaviour
{
    public Mesh m_Mesh;
    public float m_Height = 1.0f;
    public float m_Random = 1.0f;
    [Range(0, 1)] public float m_DensityX = 0.2f;
    [Range(0, 1)] public float m_DensityY = 0.2f;
    // 块数
    public int chunkX = 20;
    public int chunkY = 30;
    // 顶点数/快
    public int subVerNum = 10;
    // 参照点
    public Vector3 target = new Vector3();

    public void PerlinNoiseTerrain()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.Log("meshFilter is a nil");
            return;
        }
        m_Mesh = meshFilter.sharedMesh;
        // init parameters
        var verticesList = new List<Vector3>();
        var normalList = new List<Vector3>();
        var uvList = new List<Vector2>();
        var tangentList = new List<Vector4>();
        m_Mesh.GetVertices(verticesList);
        m_Mesh.GetNormals(normalList);
        m_Mesh.GetUVs(0, uvList);
        m_Mesh.GetTangents(tangentList);

        var meshPointXCount = 100;
        var meshPointYCount = 100;



        // vertices -> weight
        List<float> randomWeightList = new List<float>(new float[verticesList.Count]);


        for (int i = 0; i <= 1 + meshPointXCount; i += 1)
        {
            for (int j = 0; j <= meshPointYCount; j += 1)
            {
                var index = i * meshPointYCount + j;

                var prelinValue = Mathf.PerlinNoise((float)i / meshPointXCount * 20, (float)j / meshPointYCount * 20) * m_Random;

                randomWeightList[index] = prelinValue;
            }
        }



        List<Vector3> newVertices = new List<Vector3>();
        for (int i = 0; i < randomWeightList.Count; i++)
        {
            var vertice = verticesList[i];
            var height = randomWeightList[i];
            newVertices.Add(new Vector3(vertice.x, height * m_Height, vertice.z));
        }
        m_Mesh.SetVertices(newVertices);

        meshFilter.mesh = m_Mesh;
    }



    public void PerlinNoise()
    {
        StartCoroutine(IEPerlinNoise());
    }
    public void UpdateWeight()
    {
        StartCoroutine(IEUpdateWeight());
    }
    public IEnumerator IEPerlinNoise()
    {
        int maxLine = (subVerNum - 1) * chunkY + 1;
        int maxColuom = (subVerNum - 1) * chunkX + 1;
        int verNum = maxColuom * maxLine;

        Debug.Log($"Create Prelin Terrain, chunk = ({chunkX}, {chunkY}), subVerNum = {subVerNum}, verNum = {verNum}, Line = {maxLine}, coluom = {maxColuom}");
        #region
        List<Vector3> vertices = new List<Vector3>(new Vector3[verNum]);
        List<Vector3> normals = new List<Vector3>(new Vector3[verNum]);
        List<Vector4> tangents = new List<Vector4>(new Vector4[verNum]);
        List<int> triangles = new List<int>();
        List<Vector2> uvs0 = new List<Vector2>(new Vector2[verNum]);
        List<Vector2> uvs2 = new List<Vector2>(new Vector2[verNum]);

        for (int i = 0; i < vertices.Count; i++)
        {
            var coluomNum = (subVerNum - 1) * chunkX + 1;
            var lineNum = (subVerNum - 1) * chunkY + 1; 
            var x = i % coluomNum;
            var z = Mathf.Floor(i / coluomNum);
            vertices[i] = new Vector3(x, 0, z);

            uvs0[i] = new Vector2(x / coluomNum, z / lineNum);

            var p1 = i;
            var p2 = i + coluomNum;
            var p3 = i + coluomNum + 1;
            var p4 = i + 1;

            var nowLineNum = Mathf.Floor(i / coluomNum);
            var nowColuomNum = i % maxColuom;
            if (nowLineNum + 1 < maxLine && nowColuomNum + 1 < maxColuom)
                PublicFunction.AddExList(ref triangles, p1, p2, p3, p1, p3, p4);
        }
        #endregion


        for (int i = 0; i <= chunkY; i++)
        {
            for (int j = 0; j <= chunkX; j++)
            {
                var index = i * (((subVerNum - 1) * chunkX + 1) * (subVerNum - 1)) + j * (subVerNum - 1);
                var normal = PublicFunction.GetRandomVector2();
                normals[index] = normal;
            }
        }



        #region
        GameObject go = gameObject;
        if (!TryGetComponent<MeshFilter>(out var meshFilter))
            meshFilter = go.AddComponent<MeshFilter>();

        if (!TryGetComponent<MeshRenderer>(out var meshRenderer))
            meshRenderer = go.AddComponent<MeshRenderer>();
        go.transform.SetParent(transform);
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.tangents = tangents.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs0.ToArray();
        mesh.uv2 = uvs2.ToArray();
        meshFilter.mesh = mesh;
        #endregion
        yield return 0;
    }


    public IEnumerator IEUpdateWeight()
    {
        int maxLine = (subVerNum - 1) * chunkY + 1;
        int maxColuom = (subVerNum - 1) * chunkX + 1;
        int verNum = maxColuom * maxLine;

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) yield break;
        var vertices = meshFilter.sharedMesh.vertices;
        var normals = meshFilter.sharedMesh.normals;
        var uv2 = meshFilter.sharedMesh.uv2;

        var targetHeight = new List<float>();
        for (int i = 0; i <= chunkY; i++)
        {
            for (int j = 0; j <= chunkX; j++)
            {
                var index = i * (((subVerNum - 1) * chunkX + 1) * (subVerNum - 1)) + j * (subVerNum - 1);
                var normal = normals[index];


                var t = target.normalized;
                var TdotN = Vector3.Dot(t, normal);
                targetHeight.Add(TdotN);
                var ver = vertices[index];
                ver.y = TdotN;
                vertices[index] = ver;
            }
        }

        for (int i = 0; i <= chunkY; i++)
        {
            for (int j = 0; j <= chunkX; j++)
            {
                var norIndex = i * (chunkX + 1) + j;
                float verRT = targetHeight[norIndex];
                float verLT = targetHeight[norIndex];
                float verLB = targetHeight[norIndex];
                float verRB = targetHeight[norIndex];

                if ((i + 1) <= chunkY && (j + 1) <= chunkX /*&& (norIndex + (chunkX + 1) + 1) < ((i + 1) * chunkX)*/)
                    verRT = targetHeight[norIndex + (chunkX + 1) + 1];

                if ((i + 1) <= chunkY /*&& (norIndex + (chunkX + 1)) < ((i + 1) * chunkX)*/)
                    verLT = targetHeight[norIndex + (chunkX + 1)];

                if ((j + 1) <= chunkX /*&& norIndex + 1 <= chunkX*/)
                    verRB = targetHeight[norIndex + 1];

                for (int k = 0; k < subVerNum - 1; k++)
                {
                    for (int g = 0; g < subVerNum - 1; g++)
                    {
                        var lineNum = ((subVerNum - 1) * chunkX + 1);
                        var coluomNum = ((subVerNum - 1) * chunkY + 1);

                        var liNum = (i * (subVerNum - 1) + k) * lineNum;
                        var colNum = j * (subVerNum - 1) + g;
                        var index = liNum + colNum;

                        if (i * (subVerNum - 1) + k - 1 >= maxLine || j * (subVerNum - 1) + g >= maxColuom || index >= verNum)
                            continue;

                        float lerpX = (float)g / (subVerNum - 1);
                        float lerpY = (float)k / (subVerNum - 1);

                        var lX = 6 * Mathf.Pow(lerpX, 5) - 15 * Mathf.Pow(lerpX, 4) + 10 * Mathf.Pow(lerpX, 3);
                        var lY = 6 * Mathf.Pow(lerpY, 5) - 15 * Mathf.Pow(lerpY, 4) + 10 * Mathf.Pow(lerpY, 3);

                        var heightT = lX * (verRT - verLT) + verLT;
                        var heightB = lX * (verRB - verLB) + verLB;
                        var widthL = lY * (verLT - verLB) + verLB;
                        var widthR = lY * (verRT - verRB) + verRB;

                        var x = lY * (heightT - heightB) + heightB;
                        var y = lX * (widthR - widthL) + widthL;

                        //var average = x * Mathf.Sin(x * Mathf.PI * 0.5f) + y * Mathf.Sin(y * Mathf.PI * 0.5f);  // 0.0f - 1.0f
                        var average = (x + y) * 0.5f;  // 0.0f - 1.0f

                        #region        new 
                        //var x1 = (1 - lerpX) * verLB + lerpX * verRB;
                        //var x2 = (1 - lerpX) * verLT + lerpX * verRT;

                        //var y1 = (1 - lerpY) * verLB + lerpY * verLT;
                        //var y2 = (1 - lerpY) * verRB + lerpY * verRT;

                        // 2
                        //var sLX = (Mathf.Cos(lerpX * Mathf.PI) + 1) / 2;
                        //var sLY = (Mathf.Cos(lerpY * Mathf.PI) + 1) / 2;
                        //x1 = sLX * verLB + (1 - sLX) * verRB;
                        //x2 = sLX * verLT + (1 - sLX) * verRT;

                        //y1 = sLY * verLB + (1 - sLY) * verLT;
                        //y2 = sLY * verRB + (1 - sLY) * verRT;
                        //--

                        //x = (1 - lY) * x1 + lY * x2;
                        //y = (1 - lX) * y1 + lX * y2;

                        //average = (x + y) * 0.5f;
                        #endregion

                        var ver = vertices[index];
                        ver.y = average * m_Height;
                        for (int h = 0; h < 3; h++)
                        {
                            if (ver.y > 0.5f)
                            {
                                var rendom = Random.Range(0, 1.0f);
                                if (rendom < 0.5f)
                                {
                                    ver.y *= 0.5f;
                                }
                            }
                        }
                        vertices[index] = ver;
                        uv2[index] = new Vector2(average, 0);
                    }
                }
            }
        }
        meshFilter.sharedMesh.vertices = vertices;
        meshFilter.sharedMesh.uv2 = uv2;
        yield return new WaitForSeconds(0.1f);
    }

    public void SaveToSprite()
    {

    }
}
