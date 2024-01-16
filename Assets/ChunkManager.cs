using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.UI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

public class ChunkManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Chunk> Chunks = new Dictionary<Vector2Int, Chunk>();
    public int Seed;
    public const int MAX_SIZE = 32;
    public const int MAX_HEIGHT = 128;
    public int RenderDistance = 16;
    public Transform PlayerTransform;

    //for now
    [SerializeField] Material material;
    private void Awake()
    {
        if (Seed == 0)
            Seed = Mathf.RoundToInt(UnityEngine.Random.value * 2000000000f);
    }
    void Update()
    {
        Vector2Int playerPos = WorldToChunkCoord(PlayerTransform.position);
        Vector2Int middlePos = new Vector2Int(RenderDistance, RenderDistance) / 2;
        //print(playerPos);
        for (int z = 0; z < RenderDistance; z++)
        {
            for (int x = 0; x < RenderDistance; x++)
            {
                Vector2Int chunkVector = new Vector2Int(x, z) - middlePos + playerPos;
                if (!Chunks.ContainsKey(chunkVector))
                    Chunks.Add(chunkVector, CreateChunk(chunkVector));
            }
        }

        foreach (var chunk in Chunks)
        {

        }
    }
    Vector2Int WorldToChunkCoord(Vector3 WorldCoord)
    {
        return new Vector2Int(Mathf.RoundToInt(WorldCoord.x / MAX_SIZE), Mathf.RoundToInt(WorldCoord.z / MAX_SIZE));
    }
    public Chunk CreateChunk(Vector2Int coord)
    {
        Vector3 worldPosition = new Vector3(coord.x, 0, coord.y) * MAX_SIZE;
        GameObject chunkInstance = new GameObject("ChunkInstance");
        chunkInstance.transform.position = worldPosition;
        chunkInstance.transform.parent = transform;
        Chunk chunkScriptInstance = chunkInstance.AddComponent<Chunk>();
        chunkScriptInstance.chunkManager = this;
        StartCoroutine(chunkScriptInstance.GenerateChunk(Seed));
        MeshRenderer mR = chunkInstance.AddComponent<MeshRenderer>();
        mR.materials[0] = material;
        mR.material = material;
        return chunkScriptInstance;
    }
    public Chunk CreateChunk(int x, int z)
    {
        return CreateChunk(new Vector2Int(x,z));
    }
}
