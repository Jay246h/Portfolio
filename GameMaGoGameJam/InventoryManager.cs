using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();  // 인벤토리 아이템 리스트

    public Transform ItemContent;  // 아이템 UI를 배치할 부모 객체
    public GameObject InventoryItem;  // 아이템 UI 프리팹

    public Toggle EnableRemove;  // 아이템 삭제 여부를 조정하는 Toggle

    public GameObject inVenPanel;

    bool isInven;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isInven)
        {
            InvenOpen();
        }
        else if (Input.GetKeyDown(KeyCode.E) && isInven)
        {
            InvenClose();
        }


        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1번 키 눌렀을 때
        {
            UseItemFromInventory(0);  // 첫 번째 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2번 키 눌렀을 때
        {
            UseItemFromInventory(1);  // 두 번째 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3번 키 눌렀을 때
        {
            UseItemFromInventory(2);  // 두 번째 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4번 키 눌렀을 때
        {
            UseItemFromInventory(3);  // 두 번째 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // 5번 키 눌렀을 때
        {
            UseItemFromInventory(4);  // 두 번째 아이템 사용
        }

    }

    public void InvenOpen()
    {
        inVenPanel.SetActive(true);
        isInven = true;
        ListItems();
    }
    public void InvenClose()
    {
        isInven = false;
        inVenPanel.SetActive(false);

    }
    public Item GetItemById(int id)
    {
        return Items.Find(item => item.id == id);  // 아이템을 찾고 반환
    }

    public void CreateAndAddItem(int id, string name, int value, Sprite icon, string type)
    {
        // 문자열을 열거형으로 변환
        if (!System.Enum.TryParse(type, out Item.ItemType parsedType))
        {
            Debug.LogError($"잘못된 아이템 타입: {type}");
            return;
        }

        // 기존 아이템 확인
        var existingItem = Items.Find(i => i.id == id);

        if (existingItem != null)
        {
            // 기존 아이템이 있으면 수량 증가
            existingItem.quantity += 1;
        }
        else
        {
            // 새로운 아이템 생성
            var newItem = new Item
            {
                id = id,
                itemName = name,
                value = value,
                icon = icon,
                itemType = parsedType, // 변환된 열거형 타입 사용
                quantity = 1
            };

            // 아이템 리스트에 추가
            Items.Add(newItem);
        }

        // UI 갱신
        ListItems();
    }


    public bool SpendItem(int itemId, int quantity)
    {
        Item item = GetItemById(itemId); // 아이템 찾기
        if (item != null && item.quantity >= quantity) // 아이템 수량 확인
        {
            item.quantity -= quantity; // 수량 차감

            // 수량이 0이 되면 아이템을 목록에서 삭제
            if (item.quantity <= 0)
            {
                Items.Remove(item); // 아이템 제거
            }

            // UI 즉시 갱신
            ListItems();

            return true;
        }
        else
        {
            Debug.Log("아이템이 부족하거나 존재하지 않습니다.");
            return false;
        }
    }


    // 아이템을 추가합니다.
    public void Add(Item item)
    {
        // 기존 아이템을 ID로 찾아서 수량을 증가시킵니다.
        var existingItem = Items.Find(i => i.id == item.id);

        if (existingItem != null)
        {
            // 기존 아이템이 있다면 수량만 증가
            existingItem.quantity += 1;
        }
        else
        {
            // 새로운 아이템 추가 (아이템의 id, 이름, 값 등을 그대로 복사)
            // 여기서는 ScriptableObject.CreateInstance 사용하지 않고, 단순히 인스턴스를 추가합니다.
            var newItem = new Item
            {
                id = item.id,
                itemName = item.itemName,
                value = item.value,
                icon = item.icon,
                itemType = item.itemType,
                quantity = 1
            };

            Items.Add(newItem);
        }

        // UI 갱신
        ListItems();
    }

    // 아이템을 제거합니다.
    public void Remove(Item item)
    {
        // 해당 아이템을 ID로 찾고, 수량을 감소시킵니다.
        var existingItem = Items.Find(i => i.id == item.id);

        if (existingItem != null)
        {
            if (existingItem.quantity > 1)
            {
                existingItem.quantity -= 1;
            }
            else
            {
                Items.Remove(existingItem);
            }
        }

        // UI 갱신
        ListItems();
    }

    // 인벤토리 UI를 갱신합니다.
    public void ListItems()
    {
        // 기존 UI 요소들을 삭제합니다.
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        // 아이템을 UI에 새로 추가합니다.
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = $"{item.itemName} x{item.quantity}";
            itemIcon.sprite = item.icon;

            // 삭제 버튼의 활성화 여부를 설정
            removeButton.gameObject.SetActive(EnableRemove.isOn);

            // 아이템 클릭 시 `InventoryItemController`에 아이템 설정
            var itemController = obj.GetComponent<InventoryItemController>();
            itemController.Add(item);
        }
    }

    // 아이템 삭제 기능 활성화/비활성화
    public void EnableItemsRemove()
    {
        foreach (Transform item in ItemContent)
        {
            item.Find("RemoveButton").gameObject.SetActive(EnableRemove.isOn);
        }
    }

    public void UseItemFromInventory(int index)
    {
        if (index >= 0 && index < Items.Count)
        {
            var item = Items[index];
            var itemController = ItemContent.GetChild(index).GetComponent<InventoryItemController>();
            itemController.UseItem();
        }
        else
        {
            Debug.Log("해당 인덱스에 아이템이 없습니다.");
        }
    }
}
