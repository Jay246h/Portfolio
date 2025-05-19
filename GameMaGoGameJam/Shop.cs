using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject drills;
    public GameObject Drill1;
    public GameObject Drill2;

    public Image[] upgradeImage;

    public Button hpPotion;
    public Button speedPotion;
    public Button damagePotion;
    public Button hpUpgrade;
    public Button speedUpgrade;
    public Button damageUpgrade;
    public Button drillUpgrade1;
    public Button drillUpgrade2;

    [SerializeField]
    private Vector3 detectionBoxSize = new Vector3(5f, 5f, 5f);
    [SerializeField]
    private bool isDrillDetected = false;


    [SerializeField]
    private string targetTag = "Drill";

    private InventoryManager inventoryManager;  // 인벤토리 매니저
    private Drill drill;
    public ToolTip toolTip;

    private int hpPotionCost = 2;
    private int speedPotionCost = 3;
    private int damagePotionCost = 3;
    private int hpUpgradeCost = 5;
    private int speedUpgradeCost = 7;
    private int damageUpgradeCost = 10;
    private int drillUpgrade1Cost = 30;
    private int drillUpgrade2Cost = 50;

    public Sprite HpPotionSprite;
    public Sprite SpeedPotionSprite;
    public Sprite DamagePotionSprite;
    public Sprite HpUpgradeSprite;
    public Sprite DamageUpgradeSprite;
    public Sprite SpeedUpgradeSprite;
    public Sprite Drill1Sprite;
    public Sprite Drill2Sprite;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();  // InventoryManager 찾기
        drill = FindObjectOfType<Drill>();

        AddButtonListeners();
    }

    private void Update()
    {
        FindObjectWithTagInRange();
        ShopOpenClose();

        //// 크리스탈 수량 표시
        //CryStal.text = "Crystal: " + inventoryManager.GetItemById(1).quantity.ToString();
    }

    private void AddButtonListeners()
    {
        AddToolTipEvents(hpPotion, "체력 포션", $"Cost: {hpPotionCost} Crystals", HpPotionSprite);
        AddToolTipEvents(speedPotion, "스피드 포션", $"Cost: {speedPotionCost} Crystals", SpeedPotionSprite);
        AddToolTipEvents(damagePotion, "공격력 포션", $"Cost: {damagePotionCost} Crystals", DamagePotionSprite);
        AddToolTipEvents(hpUpgrade, "드릴 최대 체력 증가", $"Cost: {hpUpgradeCost} Crystals", HpUpgradeSprite);
        AddToolTipEvents(speedUpgrade, "드릴 최대 속도 증가", $"Cost: {speedUpgradeCost} Crystals", SpeedUpgradeSprite);
        AddToolTipEvents(damageUpgrade, "드릴 최대 공격력 증가", $"Cost: {damageUpgradeCost} Crystals", DamageUpgradeSprite);
        AddToolTipEvents(drillUpgrade1, "드릴 1단계 강화", $"Cost: {drillUpgrade1Cost} Ruby", Drill1Sprite);
        AddToolTipEvents(drillUpgrade2, "드릴 2단계 강화", $"Cost: {drillUpgrade2Cost} Ruby", Drill2Sprite);
    }

    private void AddToolTipEvents(Button button, string title, string description, Sprite image)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((_) =>
        {
            toolTip.SetupToolTip(title, description, image);
            toolTip.Show();
        });

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((_) => toolTip.Hide());

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
    }

    private void ShopOpenClose()
    {
        if (isDrillDetected && Input.GetKeyDown(KeyCode.F))
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionBoxSize);
    }

    private void FindObjectWithTagInRange()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, detectionBoxSize / 2, Quaternion.identity);
        isDrillDetected = false;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                isDrillDetected = true;
                break;
            }
        }
    }

    #region 업그레이드
    public void HpPotionBuy()
    {
        // 크리스탈 아이템을 소비
        if (inventoryManager.SpendItem(1, hpPotionCost))  // 크리스탈 아이템 소비 (아이템ID=1, 수량=powerUpgradeCost)
        {
            Item hpPotion = new Item
            {
                id = 3,
                itemName = "HealthPotion",
                value = 2,
                quantity = 1,
                itemType = Item.ItemType.Health_Potion,
                icon = HpPotionSprite
            };
            inventoryManager.Add(hpPotion);
            Debug.Log("체력 포션 구매");

        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void SpeedPotionBuy()
    {
        if (inventoryManager.SpendItem(1, speedPotionCost))  // 아이템ID=1은 크리스탈
        {
            Item speedPotion = new Item
            {
                id = 2,
                itemName = "SpeedPotion",
                value = 3,
                quantity = 1,
                itemType = Item.ItemType.Speed_Potion,
                icon = SpeedPotionSprite
            };
            inventoryManager.Add(speedPotion);
            Debug.Log("스피드 포션 구매");
        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void DamagePotionBuy()
    {
        if (inventoryManager.SpendItem(1, damagePotionCost))  // 아이템ID=1은 크리스탈
        {
            Item damagePotion = new Item
            {
                id = 4,
                itemName = "DamagePotion",
                value = 0.1f,
                quantity = 1,
                itemType = Item.ItemType.Damage_Potion,
                icon = DamagePotionSprite
            };
            inventoryManager.Add(damagePotion);
            Debug.Log("공격력 포션 구매");
        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void HpUpgrade()
    {
        if (inventoryManager.SpendItem(1, hpUpgradeCost))  // 아이템ID=1은 크리스탈
        {
            hpUpgradeCost += 2;
            drill.maxHP += 3;
            Debug.Log("드릴 최대 체력 업");
        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void SpeedUpgrade()
    {
        if (inventoryManager.SpendItem(1, speedUpgradeCost))  // 아이템ID=1은 크리스탈
        {
            drill.moveSpeed += 0.5f;
            speedUpgradeCost += 2;
            Debug.Log("드릴 최대 속도 업");
        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void DamageUpgrade()
    {
        if (inventoryManager.SpendItem(1, damageUpgradeCost))  // 아이템ID=1은 크리스탈
        {
            drill.damageAmount += 0.1f;
            damageUpgradeCost += 5;
            Debug.Log("드릴 최대 공격력 업");
        }
        else
        {
            Debug.Log("크리스탈이 부족합니다.");
        }
    }

    public void DrillUpgrade1()
    {
        if (inventoryManager.SpendItem(5, drillUpgrade1Cost))  // 아이템ID=5은 루비
        {
            drills.SetActive(false);
            Drill1.SetActive(true);
            Drill2.SetActive(false);
            drill.maxHP += 20;
            drill.damageAmount = 1;
            drill.moveSpeed = 0.5f;
            Debug.Log("드릴 단계 업");
        }
        else
        {
            Debug.Log("루비가 부족합니다.");
        }
    }

    public void DrillUpgrade2()
    {
        if (inventoryManager.SpendItem(1, drillUpgrade2Cost))  // 아이템ID=5은 루비
        {
            Drill1.SetActive(false);
            Drill2.SetActive(true);
            drills.SetActive(false);
            drill.maxHP += 50;
            drill.damageAmount = 3;
            drill.moveSpeed = 1;
            Debug.Log("드릴 단계 업");
        }
        else
        {
            Debug.Log("루비가 부족합니다.");
        }
    }
    #endregion


}
