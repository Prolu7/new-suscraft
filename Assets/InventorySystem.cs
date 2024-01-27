using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] Item Air;
    public Item[] inventory = new Item[9*4];

    public delegate void InventoryChangeEventHandler(object sender, InventoryChangeEventArgs e);
    public InventoryChangeEventHandler InventoryChangeEvent;

    private void Awake()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = Air;
        }
    }

    public void AddToInventory(Item item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i])
                continue;
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory));
            inventory[i] = Air;
            return;
        }
    }
    public void RemoveFromInventory(int index)
    {
        inventory[index] = Air;
        InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory));
    }
    public void RemoveNextItemFromInventoy()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == Air)
                continue;
            inventory[i] = Air;
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory));
            return;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            InventoryChangeEvent?.Invoke(this, new InventoryChangeEventArgs(inventory));
    }
}
public class InventoryChangeEventArgs
{
    public InventoryChangeEventArgs(Item[] inventory) { Inventory = inventory; }
    public Item[] Inventory { get; private set; }
}