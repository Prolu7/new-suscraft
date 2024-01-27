using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    bool inventoryIsOpen = false;
    [SerializeField] GameObject inventoryUI;
    GameManager Gm;
    InventorySystem inventorySystem;
    public Button[] Slots = new Button[9*4];
    public Image[] HotbarSlots = new Image[9];

    private void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Gm.PlayerSpawnEvent += OnPlayerSpawn;
    }

    private void OnPlayerSpawn(object sender, PlayerSpawnEventArgs e)
    {
        inventorySystem = e.PlayerGameObject.GetComponent<InventorySystem>();
        inventorySystem.InventoryChangeEvent += OnInventoryChange;
    }

    private void OnInventoryChange(object sender, InventoryChangeEventArgs e)
    {
        for (int i = 0; i < e.Inventory.Length; i++)
        {
            Slots[i].image.sprite = e.Inventory[i].Icon;
            if(i >= 9*3)
                HotbarSlots[i%HotbarSlots.Length].sprite = e.Inventory[i].Icon;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryIsOpen = !inventoryIsOpen;
            OpenInventory(inventoryIsOpen);
            inventoryUI.SetActive(inventoryIsOpen);
        }
    }
    void OpenInventory(bool open)
    {
        if (open)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

}
