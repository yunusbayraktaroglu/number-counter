using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using UnityEditor;

// https://catlikecoding.com/unity/tutorials/procedural-meshes/

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour {

    [Range(1, 10)]
    public int digitCount = 7;

    private string _meshName;

    public Mesh mesh { get; set; }

    public bool canBake { get; set; }

    void OnValidate() => enabled = true;

    void Awake()
    {
        mesh = new Mesh { name = name };
        canBake = true;
    }

    private void Start()
    {
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void Update()
    {
        CreateMesh();
        enabled = false;
    }

    /// <summary>
    /// Schedule Job
    /// </summary>
    public void CreateMesh()
	{
        if (mesh == null)
        {
            mesh = new Mesh { name = name };
            GetComponent<MeshFilter>().mesh = mesh;
        }

        _meshName = name; // GameObject name

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		Mesh.MeshData meshData = meshDataArray[0];
		MeshJob<DigitGeometry, DigitGeometryStream>.ScheduleParallel(
            mesh, meshData, digitCount, default
		).Complete();
		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }

    /// <summary>
    /// Bake mesh to Assets/BakedMeshes folder
    /// </summary>
    public void BakeMesh()
    {
        _meshName = name;
        AssetDatabase.CreateAsset(mesh, $"Assets/BakedMeshes/{_meshName}.asset");
        canBake = false;
    }

    /// <summary>
    /// Apply scale to mesh vertices
    /// </summary>
    public void ApplyScale()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x * transform.localScale.x, vertices[i].y * transform.localScale.y, vertices[i].z);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

}