using UnityEngine;

public class HotbarUpdater : MonoBehaviour
{
    GameManager GameManagerInstance;
    InventorySystem invSys;

    private void Start()
    {
        GameManagerInstance = FindObjectOfType<GameManager>();
        GameManagerInstance.PlayerSpawnEvent += PlayerSpawnEvent;
    }

    private void PlayerSpawnEvent(object sender, PlayerSpawnEventArgs e)
    {
        invSys = e.PlayerGameObject.GetComponent<InventorySystem>();
        invSys.InventoryChangeEvent += updateInv;
    }

    private void updateInv(object sender, InventoryChangeEventArgs e)
    {
        print("yes and " + e.Inventory);
    }
}
