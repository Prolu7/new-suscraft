using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    //debug
    [SerializeField] Item Stone;
    public Item[] inventory = new Item[9*4];

    public delegate void InventoryChangeEventHandler(object sender, InventoryChangeEventArgs e);
    public InventoryChangeEventHandler InventoryChangeEvent;

    public int HotbarSlots = 9;
    int hotbarSelection = 0;

    public void AddToInventory(Item item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i])
            {
                if (i == inventory.Length)
                    print("could not add to inventory");
                continue;
            }
            inventory[i] = item;
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory, hotbarSelection));
            return;
        }
    }
    public void RemoveFromInventory(int index)
    {
        inventory[index] = null;
        InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory, hotbarSelection));
    }
    public void RemoveNextItemFromInventoy()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i])
                continue;
            inventory[i] = null;
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory, hotbarSelection));
            return;
        }
    }
    int lastSelection;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory, hotbarSelection));

        if(Input.GetKeyDown(KeyCode.F))
            AddToInventory(Stone);
        
        hotbarSelection = SusHelper.PosiMod(
            Mathf.RoundToInt(hotbarSelection - Input.mouseScrollDelta.y), HotbarSlots);
        if (hotbarSelection != lastSelection)
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory, hotbarSelection));
        lastSelection = hotbarSelection;
    }
}
public class InventoryChangeEventArgs
{
    public InventoryChangeEventArgs(Item[] inventory, int selection) 
    { Inventory = inventory; Selection = selection; }
    public Item[] Inventory { get; private set; }
    public int Selection { get; private set; }
}