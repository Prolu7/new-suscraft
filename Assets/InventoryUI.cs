using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    bool inventoryIsOpen = false;
    [SerializeField] GameObject inventoryUI;
    GameManager gM;
    InventorySystem inventorySystem;
    public Button[] Slots = new Button[9*4];
    public Image[] HotbarSlots = new Image[9];
    public Image Selector;
    

    private void Awake()
    {
        gM = FindObjectOfType<GameManager>();
        gM.PlayerSpawnEvent += OnPlayerSpawn;
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
            ColorBlock cb = Slots[i].colors;
            
            if (!e.Inventory[i])
            {
                cb.normalColor = new Vector4(1f, 1f, 1f, 0.01f);
                
                if (i >= 9 * 3)
                    HotbarSlots[i % HotbarSlots.Length].color = Color.clear;

                cb.selectedColor = cb.normalColor;
                Slots[i].colors = cb;
                continue;
            }

            Slots[i].image.sprite = e.Inventory[i].Icon;
            cb.normalColor = Vector4.one;
            if(i >= 9 * 3)
            {
                HotbarSlots[i%HotbarSlots.Length].sprite = e.Inventory[i].Icon;
                HotbarSlots[i % HotbarSlots.Length].color = Color.white;
            }
            cb.selectedColor = cb.normalColor;
            Slots[i].colors = cb;
        }
        Selector.rectTransform.position = HotbarSlots[e.Selection].rectTransform.position;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryIsOpen = !inventoryIsOpen;
            OpenInventory(inventoryIsOpen);
        }
    }
    void OpenInventory(bool open)
    {
        gM.LockInput(open);
        inventoryUI.SetActive(open);
    }

}
