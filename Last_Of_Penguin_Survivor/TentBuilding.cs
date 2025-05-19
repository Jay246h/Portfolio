using System.Collections;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class TentBuilding : BuildingFrame
    {
        [SerializeField] private Transform ProjectorTransform;
        //public Vector3 cameraPositionOffset = new Vector3(0, 1.5f, -3f); // 카메라 위치 오프셋
        //public Vector3 cameraRotationEuler = new Vector3(10f, 0f, 0f);   // 오일러 각으로 회전 설정
        private Vector3 originalCameraPosition;                          // 카메라 원래 위치 저장
        private Quaternion originalCameraRotation;                       // 카메라 원래 회전 저장

        [Space(10f)]
        private Camera targetCamera;                // 카메라 참조

        [Header("GameObject")]
        [SerializeField] private GameObject player; // 캐릭터 오브젝트 참조


        [Header("float")]
        [SerializeField] private float healAmount = 1f; // 매 초 회복되는 체력 양
        private float healInterval = 1f; // 회복 간격 (초)
        private float healTimer; // 회복 타이머

        [Header("string")]
        // 카메라를 오브젝트 이름으로 할당하기 위한 변수
        [SerializeField] private string cameraObjectName = "Main Camera"; // 카메라 오브젝트 이름

        [Header("Scripts")]
        PenguinBody penguinBody;
        PenguinFunction penguinFunction;
        private InventoryHandler inventoryHandler;
        private HouseManager houseManager;
        private TestFollow followCamera;
        private BuildingSelectManager buildingSelectManager;


        protected override void Start()
        {
            followCamera = Camera.main.GetComponent<TestFollow>();

            buildingSelectManager = GetComponent<BuildingSelectManager>();

            base.Start();

            targetCamera = Camera.main.GetComponent<Camera>();

            if (LopNetworkManager.GetPlayer() != null)
            {
                player = LopNetworkManager.GetPlayer();
                inventoryHandler = player.GetComponentInChildren<InventoryHandler>();
                penguinBody = player.GetComponent<PenguinBody>();
                penguinFunction = player.GetComponent<PenguinFunction>();
                ProjectorTransform = penguinBody.transform.Find("Projector").transform;
            }
            else
            {
                StartCoroutine(Co_GetPlayer());
            }


            healTimer = healInterval;

            // 카메라 원위치 저장
            if (targetCamera != null)
            {
                originalCameraPosition = targetCamera.transform.position;
                originalCameraRotation = targetCamera.transform.rotation;
            }
        }

        private IEnumerator Co_GetPlayer()
        {
            yield return new WaitForSeconds(1f);

            player = LopNetworkManager.GetPlayer();
            inventoryHandler = player.GetComponentInChildren<InventoryHandler>();
            penguinBody = player.GetComponent<PenguinBody>();
            penguinFunction = player.GetComponent<PenguinFunction>();
            ProjectorTransform = penguinBody.transform.Find("Projector").transform;
        }

        private void Update()
        {
            if (isHealing && penguinBody != null)
            {
                healTimer -= Time.deltaTime;

                if (healTimer <= 0)
                {
                    penguinBody.status.status_hp = Mathf.Min(penguinBody.status.status_hp + healAmount, 100f);
                    healTimer = healInterval;
                }
            }
        }

        protected override void OpenTeb()
        {
            if (isHealing)
            {
                inventoryHandler.currentState = InventoryHandlerState.Default;
                StopHealing();
                penguinFunction.MoveControllerOffFunction();
                penguinBody.isBuildingEnter = false;
            }
            else
            {
                inventoryHandler.currentState = InventoryHandlerState.InBuilding;
                StartHealing();
                penguinBody.isBuildingEnter = true;
            }
        }
    }
}
