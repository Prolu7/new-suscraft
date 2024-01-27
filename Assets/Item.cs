using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "ScriptableObject/Item", order = 0)]
public class Item : ScriptableObject
{
    public string Name;
    public int StackSize = 64;
    public Sprite Icon;
    public string Description;
}
