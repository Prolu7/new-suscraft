using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public int Seed = 0;
    BlockType[,,] data;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    public ChunkManager chunkManager;

    FastNoiseLite noiseLite = new FastNoiseLite();

    private void Awake()
    {
        data = new BlockType[ChunkManager.MAX_SIZE, ChunkManager.MAX_HEIGHT, ChunkManager.MAX_SIZE];
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }
    public IEnumerator GenerateChunk(int seed)
    {
        noiseLite.SetSeed(seed);
        Vector3 position = transform.position;
        Task dataTask = Task.Run(() => GenerateData(position));
        while(!dataTask.IsCompleted)
        {
            yield return new WaitForSeconds(0.2f);
        }
        Task meshTask = Task.Run(() => GenerateMesh());
        while (!meshTask.IsCompleted)
        {
            yield return new WaitForSeconds(0.2f);
        }
        //yield return t.GetAwaiter();
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, .3f));
        ImportMesh(vertices.ToArray(), tris.ToArray(), normals.ToArray(), uvs.ToArray());
    }
    void GenerateData(Vector3 position)
    {
        //noise pass
        for (int y = 0; y < ChunkManager.MAX_HEIGHT; y++)
        {
            for (int z = 0; z < ChunkManager.MAX_SIZE; z++)
            {
                for (int x = 0; x < ChunkManager.MAX_SIZE; x++)
                {
                    double noise = (noiseLite.GetNoise((position.x + x) * .5f, (position.z + z) * .5f)) * 40;
                    noise += (noiseLite.GetNoise((position.x + x) * 1f, (position.z + z) * 1f)) * 10;
                    noise += (noiseLite.GetNoise((position.x + x) * 3f, (position.z + z) * 3f)) * 5;
                    
                    if (y < (noise / 2 + 64))
                        data[x, y, z] = BlockType.Stone;
                }
            }
        }
        //block type pass
        for (int y = 0; y < ChunkManager.MAX_HEIGHT; y++)
        {
            for (int z = 0; z < ChunkManager.MAX_SIZE; z++)
            {
                for (int x = 0; x < ChunkManager.MAX_SIZE; x++)
                {
                    if (data[x, y, z] == BlockType.Stone && data[x, y + 1, z] == BlockType.Air)
                        data[x, y, z] = BlockType.Grass;
                }
            }
        }
    }
    void GenerateMesh() {
        vertices.Clear();
        tris.Clear();
        normals.Clear();
        uvs.Clear();

        for (int y = 0; y < ChunkManager.MAX_HEIGHT; y++)
        {
            for (int z = 0; z < ChunkManager.MAX_SIZE; z++)
            {
                for (int x = 0; x < ChunkManager.MAX_SIZE; x++)
                {
                    if (data[x, y, z] == BlockType.Air)
                        continue;

                    Vector3 offset = new Vector3(x, y, z);

                    if (x + 1 == ChunkManager.MAX_SIZE || x + 1 < ChunkManager.MAX_SIZE && data[x + 1, y, z] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.right);
                    }

                    if (x == 0 || x - 1 >= 0 && data[x - 1, y, z] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.left);
                    }

                    if (y + 1 == ChunkManager.MAX_HEIGHT || y + 1 < ChunkManager.MAX_HEIGHT && data[x, y + 1, z] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.up);
                    }

                    if (y == 0 || y - 1 >= 0 && data[x, y - 1, z] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.down);
                    }

                    if (z + 1 == ChunkManager.MAX_SIZE || z + 1 < ChunkManager.MAX_SIZE && data[x, y, z + 1] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.forward);
                    }

                    if (z == 0 || z - 1 >= 0 && data[x, y, z - 1] == BlockType.Air)
                    {
                        GenerateFace(offset, Vector3.back);
                    }
                }
            }
        }
        for (int i = 0; i < normals.Count; i++) {
            normals[i] = Vector3.Normalize(normals[i]);
            //Debug.DrawRay(vertices[i], normals[i] * .33f, Color.red, float.PositiveInfinity);
        }
    }
    void ImportMesh(Vector3[] vertices, int[] tris, Vector3[] normals, Vector2[] uvs)
    {
        Mesh mesh = new Mesh();
        mesh.name = $"Chunk, {UnityEngine.Random.value}";
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uvs;
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    private int AddVertexAndNormal(Vector3 vertex, Vector3 normal)
    {
        //Unique vertices
        //int indexOf = vertices.IndexOf(vertex);
        //if (indexOf >= 0)
        //{
        //    normals[indexOf] += normal;
        //    return indexOf;
        //}
        vertices.Add(vertex);
        normals.Add(normal);
        
        return vertices.Count - 1;
    }
    private Vector3[] GenerateFaceVertices(Vector3 direction)
    {
        Vector3[] faceVertices = new Vector3[4];
        Quaternion rotator = Quaternion.FromToRotation(Vector3.up, direction);
        
        faceVertices[0] = rotator * (new Vector3(-.5f, .5f, -.5f));
        faceVertices[1] = rotator * (new Vector3(.5f, .5f, -.5f));
        faceVertices[2] = rotator * (new Vector3(.5f, .5f, .5f));
        faceVertices[3] = rotator * (new Vector3(-.5f, .5f, .5f));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));
        return faceVertices;
    }
    private void GenerateFace(Vector3 offset, Vector3 direction)
    {
        int[] vertIndex = new int[4];
        Vector3[] faceVertices = GenerateFaceVertices(direction);
        for (int i = 0; i < faceVertices.Length; i++)
        {
            vertIndex[i] = AddVertexAndNormal(faceVertices[i] + offset, direction);
        }
        tris.Add(vertIndex[2]);
        tris.Add(vertIndex[1]);
        tris.Add(vertIndex[0]);

        tris.Add(vertIndex[0]);
        tris.Add(vertIndex[3]);
        tris.Add(vertIndex[2]);    
    }
}