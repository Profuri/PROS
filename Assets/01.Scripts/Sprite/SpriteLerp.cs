using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpriteLerp : MonoBehaviour
{
    public List<BoxCollider2D> colliders = new ();
    public int _meshResolution = 1; // 관절의 사각형 개수
    int _childCount = 0;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Material _material;
    private Vector2 _localPos;
    private void Awake()
    {
        GetComponentsInChildren<BoxCollider2D>(colliders);
        _childCount = colliders.Count;
    }

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = new Material(Shader.Find("Sprites/Default"));
        _localPos = transform.localPosition;

    }

    private void Update()
    {
        Vector2[,] vertices_list = new Vector2[_childCount, 4]; // 4는 box의 vertex count
        for (int i = 0; i < colliders.Count; i++)
        {
            Vector2[] temp_vertices = GetBoxColliderCornersWorldPos(colliders[i]);
            for (int j = 0; j < temp_vertices.Length; j++)
            {
                vertices_list[i, j] = temp_vertices[j];
            }
        }

        //Mesh Create
        Mesh mesh = new Mesh();

        int vertexcount = vertices_list.Length;  // 기본 사각형을 구성하는 데 필요한 vertex개수
        vertexcount += ((_childCount - 1)) * _meshResolution * 2 - 2; // 관절 사이 연결에 필요한 vertex개수

        List<Vector3> result_vertices = new();

        int[] triangles = new int[18];
        //Vector2[] result_uv = new Vector2[];

        int currefboxidx = 0;
        for (int i = 0; i < vertices_list.GetLength(0); i++) // 박스 공간 갯수 + 관절 갯수 = (박스 갯수 * 2) - 1
        {
            if (i % 2 == 0) //박스 공간
            {
                for (int k = 0; k < 4; k++)
                {
                    result_vertices.Add(vertices_list[currefboxidx, k]);
                }
                triangles[currefboxidx * 6 + 0] = 0;
                triangles[currefboxidx * 6 + 1] = 1;
                triangles[currefboxidx * 6 + 2] = 2;
                triangles[currefboxidx * 6 + 3] = 1;
                triangles[currefboxidx * 6 + 4] = 2;
                triangles[currefboxidx * 6 + 5] = 3;
            }
            else // 관절 공간
            {
                triangles[currefboxidx * 6 + 3] = 1;
                //필요한 거 현재 ㅐ박스에 3, 4 idx 번호와 다음 박스에 1, 2 번호
                //Vector2 pos =  
                currefboxidx++;
            }
        }

        mesh.Clear();
        mesh.vertices = result_vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        _meshRenderer.material = _material;
        _meshFilter.mesh = mesh;

        #region Debug
        //World Pos
        //for (int i = 0; i < _childCount; i++)
        //{
        //    string vertices = string.Empty;
        //    for(int k = 0; k < 4; k++)
        //    {
        //        vertices += vertices_list[i,k].ToString();
        //    }
        //    Debug.Log($"{colliders[i].gameObject.name} : {vertices}");
        //}
        #endregion
    }

    public Vector2[] GetBoxColliderCornersWorldPos(BoxCollider2D _collider)
    {
        Vector2[] vertices = new Vector2[4];

        Bounds b = _collider.bounds;
        vertices[0] = new Vector2(b.min.x, b.max.y) - _localPos; // 왼쪽 위
        vertices[1] = new Vector2(b.max.x, b.max.y) - _localPos; // 오른쪽 위
        vertices[2] = new Vector2(b.min.x, b.min.y) - _localPos; // 왼쪽 아래
        vertices[3] = new Vector2(b.max.x, b.min.y) - _localPos; // 오른쪽 아래

        return vertices;
    }
}
