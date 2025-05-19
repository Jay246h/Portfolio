using System.Collections;
using Lop.Survivor.inventroy;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Lop.Survivor.Island.Buildingbase
{
    public abstract class BuildingFrame : BuildingBase
    {
        [Header("Slider")]
        [Space(5f)]
        public Slider durabilitySlider;        // 내구도 슬라이더

        [Header("bool")]
        [Space(5f)]
        private bool hasDroppedItems = false;  // 아이템 드롭 방지 플래그
        public bool isHealing;              // 힐 받고 있는지

        [Header("float")]
        [Space(5f)]
        public float currentDurability = 100f; // 초기 내구도 (현재 체력)
        public float maxDurability = 100f;     // 최대 내구도

        [Space(10f)]
        private Camera targetCamera;                // 카메라 참조

        [Header("GameObject")]
        [SerializeField] private GameObject player; // 캐릭터 오브젝트 참조

        [Header("Scripts")]
        [Space(5f)]
        private BuildingMeshSync buildingMeshSync;
        private TestFollow followCamera;
        private BuildingSelectManager buildingSelectManager;
        private PenguinFunction penguinFunction;
        private PenguinBody penguinBody;

        [Header("Camera Setting")]
        [SerializeField] private Transform ProjectorTransform;
        public Vector3 cameraPositionOffset = new Vector3(0, 1.5f, -3f); // 카메라 위치 오프셋
        public Vector3 cameraRotationEuler = new Vector3(10f, 0f, 0f);   // 오일러 각으로 회전 설정
        private Vector3 originalCameraPosition;                          // 카메라 원래 위치 저장
        private Quaternion originalCameraRotation;                       // 카메라 원래 회전 저장


        public bool IsHealing
        {
            get { return isHealing; }
            protected set { isHealing = value; }
        }

        protected override void Start()
        {
            followCamera = Camera.main.GetComponent<TestFollow>();

            buildingSelectManager = GetComponent<BuildingSelectManager>();

            targetCamera = Camera.main.GetComponent<Camera>();

            if (LopNetworkManager.GetPlayer() != null)
            {
                player = LopNetworkManager.GetPlayer();
                penguinFunction = player.GetComponent<PenguinFunction>();
                penguinBody = player.GetComponent<PenguinBody>();
                ProjectorTransform = penguinBody.transform.Find("Projector").transform;
            }
            else
            {
                StartCoroutine(Co_GetPlayer());
            }


            base.Start();
            durabilitySlider.maxValue = maxDurability;
            durabilitySlider.value = currentDurability;
            if (durabilitySlider == null)
            {
                return;
            }
            buildingMeshSync = GetComponent<BuildingMeshSync>();
        }

        private IEnumerator Co_GetPlayer()
        {
            yield return new WaitForSeconds(1f);

            player = LopNetworkManager.GetPlayer();
            penguinBody = player.GetComponent<PenguinBody>();
            //ProjectorTransform = penguin.transform.Find("Projector").transform;
        }

        public void UpdateDurability(float amount)
        {
            if (buildingMeshSync.isServer)
            {
                currentDurability += amount;
                currentDurability = Mathf.Clamp(currentDurability, 0f, maxDurability);

                buildingMeshSync.currentDurability = currentDurability;

                if (currentDurability <= 0 && !hasDroppedItems)
                {
                    hasDroppedItems = true;
                    //DestoryandSetDrop(); // 아이템 드롭
                }
            }
            else
            {
                buildingMeshSync.SendDurabilityMessage(amount);
            }
        }

        public float GetCurrentDurability()
        {
            return currentDurability;
        }

        public void Dropitem()
        {
            DropItem();
        }
        public void Use()
        {
            OpenTeb();
        }

        protected abstract void OpenTeb();

        public void Repair(inventroy.Inventory inventory, ToolWheel toolWheel)
        {
            // inventory나 toolWheel이 null인 경우를 예외 처리
            if (inventory == null || toolWheel == null)
            {
                Debug.LogError("Inventory 또는 ToolWheel이 null입니다. 수리할 수 없습니다.");
                return;
            }

            foreach (var slot in inventory.slotDatas)
            {
                if (slot.slotItemData != null && slot.slotItemData.itemName == ItemCategory.Snow)
                {
                    if (slot.itemCount >= 10)
                    {
                        slot.itemCount -= 10;
                        float repairAmount = 10f;
                        UpdateDurability(repairAmount);
                        return;
                    }
                    else
                    {
                        Debug.Log("<color=red>눈덩이가 부족합니다.</color>");
                        return;
                    }
                }
            }
        }


        private void FindPenguinInventory()
        {
            GameObject penguin = GameObject.FindWithTag(BuildingTagAnim.Player);
            if (penguin == null)
            {
                Debug.LogError("Player 태그를 가진 펭귄 객체를 찾을 수 없습니다.");
                return;
            }

            inventroy.Inventory inventory = penguin.GetComponentInChildren<inventroy.Inventory>();
            ToolWheel toolWheel = penguin.GetComponentInChildren<ToolWheel>();

            if (inventory == null)
            {
                Debug.LogError("Inventory 컴포넌트를 찾을 수 없습니다.");
            }

            if (toolWheel == null)
            {
                Debug.LogError("ToolWheel 컴포넌트를 찾을 수 없습니다.");
            }

            if (inventory != null && toolWheel != null)
            {
                Repair(inventory, toolWheel);
            }
        }

        public void StartHealing()
        {
            followCamera.GetFollowTransform(transform);
            isHealing = true;
            penguinBody.UnInput(false);
            penguinFunction.MoveControllerOffFunction();
            buildingSelectManager.checkPlayerInHouse = BuildingSelectManager.CheckPlayerInHouse.InHouse;
            buildingSelectManager.DestroyIcon(false);

            if (player != null)
            {
                SetPlayerVisibility(false);
                ProjectorTransform.gameObject.SetActive(false);
            }

            if (targetCamera != null)
            {
                targetCamera.transform.position = transform.position + cameraPositionOffset;
                targetCamera.transform.rotation = Quaternion.Euler(cameraRotationEuler);
            }
        }

        public void StopHealing()
        {
            isHealing = false;
            penguinBody.UnInput(true);
            buildingSelectManager.checkPlayerInHouse = BuildingSelectManager.CheckPlayerInHouse.OutHouse;
            buildingSelectManager.DestroyIcon(true);

            if (player != null)
            {
                SetPlayerVisibility(true);
                ProjectorTransform.gameObject.SetActive(true);
            }

            if (targetCamera != null)
            {
                targetCamera.transform.position = originalCameraPosition;
                targetCamera.transform.rotation = originalCameraRotation;
            }
        }


        public void SetPlayerVisibility(bool isVisible)
        {
            Renderer[] renderers = penguinBody.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (!renderer.gameObject.CompareTag("FindMission"))
                    renderer.enabled = isVisible;
            }

            //penguin.GetComponent<PenguinMeshSync>().SetPlayerInBuilding(isVisible);

            penguinBody.GetComponent<Rigidbody>().useGravity = isVisible;

        }

    }
}
