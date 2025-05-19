using System.Collections;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class HouseManager : MonoBehaviour
    {
        [SerializeField] private Transform ProjectorTransform;
        public Vector3 cameraPositionOffset = new Vector3(0, 1.5f, -3f); // 카메라 위치 오프셋
        public Vector3 cameraRotationEuler = new Vector3(10f, 0f, 0f);   // 오일러 각으로 회전 설정
        private Vector3 originalCameraPosition;                          // 카메라 원래 위치 저장
        private Quaternion originalCameraRotation;                       // 카메라 원래 회전 저장

        [Space(10f)]
        private Camera targetCamera;                // 카메라 참조

        [Header("GameObject")]
        [SerializeField] private GameObject player; // 캐릭터 오브젝트 참조

        [Header("Scripts")]
        private PenguinBody penguinBody;
        private PenguinFunction penguinFunction;
        private TestFollow followCamera;
        private BuildingSelectManager buildingSelectManager;
        private BuildingFrame interactionButton;



        void Start()
        {
            followCamera = Camera.main.GetComponent<TestFollow>();

            targetCamera = Camera.main.GetComponent<Camera>();

            buildingSelectManager = GetComponent<BuildingSelectManager>();

            if (LopNetworkManager.GetPlayer() != null)
            {
                player = LopNetworkManager.GetPlayer();
                penguinBody = player.GetComponent<PenguinBody>();
                penguinFunction = player.GetComponent<PenguinFunction>();
                ProjectorTransform = penguinBody.transform.Find("Projector").transform;
            }
            else
            {
                StartCoroutine(Co_GetPlayer());
            }

        }

        void Update()
        {

        }

        private IEnumerator Co_GetPlayer()
        {
            yield return new WaitForSeconds(1f);

            player = LopNetworkManager.GetPlayer();
            penguinBody = player.GetComponent<PenguinBody>();
            ProjectorTransform = penguinBody.transform.Find("Projector").transform;
        }

        public void StartHealing()
        {
            followCamera.GetFollowTransform(transform);
            interactionButton.isHealing = true;
            penguinBody.UnInput(false);
            penguinFunction.MoveControllerOffFunction();
            buildingSelectManager.checkPlayerInHouse = BuildingSelectManager.CheckPlayerInHouse.InHouse;
            buildingSelectManager.DestroyIcon(false);

            if (player != null)
            {
                SetPlayerVisibility(false); // 플레이어 비가시화
                ProjectorTransform.gameObject.SetActive(false);
            }

            if (targetCamera != null)
            {
                // 지정된 카메라의 위치와 회전을 오프셋과 각도로 설정
                targetCamera.transform.position = transform.position + cameraPositionOffset;
                targetCamera.transform.rotation = Quaternion.Euler(cameraRotationEuler);
            }
        }

        public void StopHealing()
        {
            interactionButton.isHealing = false;
            penguinBody.UnInput(true);
            buildingSelectManager.checkPlayerInHouse = BuildingSelectManager.CheckPlayerInHouse.OutHouse;
            buildingSelectManager.DestroyIcon(true);

            if (player != null)
            {
                SetPlayerVisibility(true); // 플레이어 가시화
                ProjectorTransform.gameObject.SetActive(true);
            }

            if (targetCamera != null)
            {
                // 지정된 카메라의 원래 위치와 회전으로 복원
                targetCamera.transform.position = originalCameraPosition;
                targetCamera.transform.rotation = originalCameraRotation;
            }
        }

        /// <summary>
        /// 플레이어 렌더러 끄게 해주는 함수
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetPlayerVisibility(bool isVisible)
        {
            Renderer[] renderers = penguinBody.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (!renderer.gameObject.CompareTag("FindMission"))
                    renderer.enabled = isVisible;
            }

            penguinBody.GetComponent<PenguinPrefabSync>().SetPlayerInBuilding(isVisible);

            penguinBody.GetComponent<Rigidbody>().useGravity = isVisible;

        }
    }
}
