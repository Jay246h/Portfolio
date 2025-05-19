using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item Item;

    void PickUp()
    {
        InventoryManager.Instance.Add(Item);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Drill"))
        {
            PickUp();
        }
    }
}
