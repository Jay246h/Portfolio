using Lop.Survivor.inventroy;
using Lop.Survivor.inventroy.Item;
using UnityEngine;
using static Item;

namespace Lop.Survivor.Island.Buildingbase
{
    public class BuildingSelectManager : MonoBehaviour
    {
        private Vector3 boxSize;
        [Header("GameObject")]
        [Space(5f)]
        public GameObject buildingRoot;                      //건축물의 루트 오브젝트를 참조하는 변수
        [SerializeField] private GameObject destroyIcon;     //건축물 선택했을 때 나타나는 파괴 아이콘
        [SerializeField] private GameObject repairIcon;      //건축물 선택했을 때 나타나는 수리 아이콘
        [SerializeField] private GameObject selectIcon;      //건축물 선택했을 때 나타나는 선택 아이콘

        [Header("bool")]
        [Space(5f)]
        //[SerializeField] private bool isBuildingDestroyed = false;   //건축물이 파괴되어있는지 확인
        public bool isEnter = false;
        public bool isLooking = false;                               //플레이어가 건축물을 바라보고 있는지 확인
        [SerializeField] private bool isPenguinDetected = false;
        public bool repairBuilding = false;                          //수리 아이콘을 이용한것인지 확인

        [Header("enum")]
        [Space(5f)]
        public CheckPlayerInHouse checkPlayerInHouse;       //플레이어가 건물 안팎에 있는것 나누기
        public OpenSelectIcon openSelectIcon;               //건축물 선택 아이콘 상태
        public State repairState;                           //수리가 가능한 건축물인지 나누기

        public enum CheckPlayerInHouse { OutHouse, InHouse }
        public enum OpenSelectIcon { Off, On }
        public enum State { Yes, No }

        [Header("float")]
        [Space(5f)]
        public float damageAmount = 10f;
        public float detectionRadius = 3f;

        [Header("string")]
        [Space(5f)]
        public string buildingName;

        [Header("Scripts")]
        [Space(5f)]
        [SerializeField] private BuildingFrame interactionButton;
        public InventoryHandler inventoryHandler;
        public PenguinFunction penguinFunction;
        public PenguinBody penguinBody;

        private void Start()
        {
            boxSize = new Vector3(detectionRadius, detectionRadius, detectionRadius);
            inventoryHandler = LopNetworkManager.GetPlayer().GetComponentInChildren<InventoryHandler>();
            penguinBody = LopNetworkManager.GetPlayer().GetComponent<PenguinBody>();
            penguinFunction = LopNetworkManager.GetPlayer().GetComponent<PenguinFunction>();

            // 건축물의 루트 오브젝트 설정
            buildingRoot = transform.root.gameObject;
        }

        void Update()
        {
            FindObjectsWithTagInRange();
            ChangeMode();
        }

        private void ChangeMode()
        {
            if (isPenguinDetected && isLooking)
            {
                BoolRepairActive();
                switch (openSelectIcon)
                {
                    case OpenSelectIcon.Off:
                        if (Input.GetKeyDown(KeyCode.A) && (inventoryHandler.currentState == InventoryHandlerState.Default || inventoryHandler.currentState == InventoryHandlerState.InBuilding))
                        {
                            penguinBody.isOpenUI = true;
                            selectIcon.SetActive(true);
                            inventoryHandler.currentState = InventoryHandlerState.BuildingUI;
                            openSelectIcon = OpenSelectIcon.On;
                            penguinBody.UnInput(false);
                            penguinFunction.MoveControllerOffFunction();
                        }
                        break;
                    case OpenSelectIcon.On:
                        switch (checkPlayerInHouse)
                        {
                            case CheckPlayerInHouse.InHouse:
                                Left(true);
                                Down(true);
                                break;
                            case CheckPlayerInHouse.OutHouse:
                                Left(true);
                                Up();
                                Down(false);
                                Right(false);
                                break;
                        }
                        break;
                }
            }
        }

        // 오른쪽 버튼 - 파괴
        private void Right(bool isNotFunction)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && penguinFunction.currentItemData.itemType == ItemType.Hammer)
            {
                penguinFunction.CmdSetTrigger("isMining");
                penguinBody.UnInput(isNotFunction);
                penguinFunction.MoveControllerOffFunction();
                interactionButton.UpdateDurability(-damageAmount);

                // 건물 파괴 확인
                if (interactionButton.GetCurrentDurability() <= 0 /*&& !isBuildingDestroyed*/)
                {
                    selectIcon.SetActive(false);
                    openSelectIcon = OpenSelectIcon.Off;
                    //isBuildingDestroyed = true;
                    BuildingManager.Instance.DestroyBuilding(buildingRoot);
                    interactionButton.Dropitem();
                    inventoryHandler.currentState = InventoryHandlerState.Default;
                    OffWindow();
                }
            }
        }

        // 왼쪽 버튼 - 사용
        private void Left(bool isNotFunction)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && inventoryHandler.currentState == InventoryHandlerState.BuildingUI)
            {
                penguinBody.UnInput(isNotFunction);
                interactionButton.Use();
                OffWindow();
            }
        }

        // 아래 버튼 - 취소
        public void Down(bool inOut)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Escape))
            {
                penguinBody.UnInput(inOut);
                inventoryHandler.currentState = InventoryHandlerState.Default;
                if (checkPlayerInHouse == CheckPlayerInHouse.InHouse)
                {
                    inventoryHandler.currentState = InventoryHandlerState.InBuilding;
                }
                OffWindow();
                penguinBody.isOpenUI = inOut;
            }
        }

        // 윗 버튼 - 수리
        private void Up()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (repairState == State.Yes) // repairState가 Yes일 때만 수리 가능
                {
                    penguinBody.UnInput(true);
                    inventroy.Inventory inventory = penguinBody.GetComponentInChildren<inventroy.Inventory>();
                    ToolWheel toolWheel = penguinBody.GetComponentInChildren<ToolWheel>();

                    interactionButton.Repair(inventory, toolWheel);
                }
                else
                {
                    Debug.Log("<color=red>수리가 불가능합니다. 수리 상태가 No로 설정되어 있습니다.</color>");
                }
            }
        }

        public void OffWindow()
        {
            selectIcon.SetActive(false);
            penguinBody.isOpenUI = false;
            penguinFunction.MoveControllerOffFunction();
            if (checkPlayerInHouse == CheckPlayerInHouse.InHouse)
            {
                penguinBody.UnInput(true);
            }
            openSelectIcon = OpenSelectIcon.Off;
        }

        private void BoolRepairActive()
        {
            if (repairBuilding)
            {
                repairIcon.SetActive(true);
            }
            else
            {
                repairIcon.SetActive(false);
            }
        }
        /// <param name="show">아이콘을 보여줄거면 true 아니라면 false</param>
        public void DestroyIcon(bool show)
        {
            destroyIcon.SetActive(show);
            BoolRepairActive();
        }


        void FindObjectsWithTagInRange()
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position, boxSize / 2, Quaternion.identity);
            isPenguinDetected = false;

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag(BuildingTagAnim.Player))
                {
                    isPenguinDetected = true;
                    return;
                }
            }

            if (!isPenguinDetected && interactionButton != null)
            {
                selectIcon.SetActive(false);
                //inventoryHandler.currentState = InventoryHandlerState.Default;
                openSelectIcon = OpenSelectIcon.Off;
                isLooking = false;
            }
        }
        /// <param name="look">플레이어가 바라보고 있다면 true 아니라면 false</param>
        public void CheckPlayerLook(bool look)
        {
            isLooking = look;
        }

    }
}
