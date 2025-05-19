using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public float value;
    public Sprite icon;
    public ItemType itemType;
    public string Description;
    public int quantity; // ¼ö·® °ü¸®

    public enum ItemType
    {
        Crystal,
        Health_Potion,
        Speed_Potion,
        Damage_Potion,
        Ruby
    }
}
