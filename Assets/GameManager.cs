using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject player;

    public delegate void PlayerSpawnEventHandler(object sender, PlayerSpawnEventArgs e);
    public PlayerSpawnEventHandler PlayerSpawnEvent;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        player.SetActive(false);
        StartCoroutine(TrySpawnPlayer());
    }

    IEnumerator TrySpawnPlayer()
    {
        Vector3 rayOrigin = new Vector3(0, ChunkManager.MAX_HEIGHT, 0);
        RaycastHit hitInfo;
        while (!Physics.SphereCast(rayOrigin, .5f, Vector3.down, out hitInfo))
            yield return null;
        player.SetActive(true);
        player.transform.position = hitInfo.point + Vector3.up;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerSpawnEvent?.Invoke(this, new PlayerSpawnEventArgs(player));
    }
}
public class PlayerSpawnEventArgs
{
    public PlayerSpawnEventArgs(GameObject playerGameObject) { PlayerGameObject = playerGameObject; }
    public GameObject PlayerGameObject { get; private set; }
}