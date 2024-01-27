using System.Collections.Generic;
using UnityEngine;

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
        Vector2Int playerPos = WorldToCenteredChunkCoord(PlayerTransform.position);
        Vector2Int halfRenderDst = new Vector2Int(RenderDistance, RenderDistance) / 2;
        //print(playerPos);
        for (int z = 0; z < RenderDistance; z++)
        {
            for (int x = 0; x < RenderDistance; x++)
            {
                Vector2Int chunkVector = new Vector2Int(x, z) - halfRenderDst + playerPos;
                if (!Chunks.ContainsKey(chunkVector))
                    Chunks.Add(chunkVector, CreateChunk(chunkVector));
            }
        }

        Queue<Vector2Int> toRemove = new Queue<Vector2Int>();
        foreach (var chunk in Chunks)
        {
            Vector2Int maxRenderBox = playerPos + halfRenderDst;
            Vector2Int minRenderBox = playerPos - halfRenderDst;
            if(chunk.Key.x < minRenderBox.x || chunk.Key.x > maxRenderBox.x
                || chunk.Key.y < minRenderBox.y || chunk.Key.y > maxRenderBox.y)
            {
                toRemove.Enqueue(chunk.Key);
                print("ENQUEUE: " + chunk.Key);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            Vector2Int key = toRemove.Dequeue();
            print("REMOVING: " + key);
            Destroy(Chunks[key].gameObject);
            Chunks.Remove(key);
        }
    }
    Vector2Int WorldToCenteredChunkCoord(Vector3 WorldCoord)
    {
        return new Vector2Int(Mathf.FloorToInt(WorldCoord.x / MAX_SIZE), Mathf.FloorToInt(WorldCoord.z / MAX_SIZE));
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
