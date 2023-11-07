using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpriteLerp : MonoBehaviour
{
    public List<BoxCollider2D> colliders = new ();
    public int _meshResolution = 1; // ������ �簢�� ����
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
        Vector2[,] vertices_list = new Vector2[_childCount, 4]; // 4�� box�� vertex count
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

        int vertexcount = vertices_list.Length;  // �⺻ �簢���� �����ϴ� �� �ʿ��� vertex����
        vertexcount += ((_childCount - 1)) * _meshResolution * 2 - 2; // ���� ���� ���ῡ �ʿ��� vertex����

        List<Vector3> result_vertices = new();

        int[] triangles = new int[18];
        //Vector2[] result_uv = new Vector2[];

        int currefboxidx = 0;
        for (int i = 0; i < vertices_list.GetLength(0); i++) // �ڽ� ���� ���� + ���� ���� = (�ڽ� ���� * 2) - 1
        {
            if (i % 2 == 0) //�ڽ� ����
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
            else // ���� ����
            {
                triangles[currefboxidx * 6 + 3] = 1;
                //�ʿ��� �� ���� ���ڽ��� 3, 4 idx ��ȣ�� ���� �ڽ��� 1, 2 ��ȣ
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
        vertices[0] = new Vector2(b.min.x, b.max.y) - _localPos; // ���� ��
        vertices[1] = new Vector2(b.max.x, b.max.y) - _localPos; // ������ ��
        vertices[2] = new Vector2(b.min.x, b.min.y) - _localPos; // ���� �Ʒ�
        vertices[3] = new Vector2(b.max.x, b.min.y) - _localPos; // ������ �Ʒ�

        return vertices;
    }
}
