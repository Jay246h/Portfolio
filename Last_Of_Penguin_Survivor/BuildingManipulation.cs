namespace Lop.Survivor.inBuilding
{
    using UnityEngine;
    using UnityEngine.UI;

    public class BuildingManipulation : MonoBehaviour
    {
        private GameObject selectedObject; // 선택된 오브젝트
        PreviewSystem previewSystem; // 프리뷰 시스템
        private SpriteRenderer spriteRenderer;  // 겹쳐질 때 색깔 변경
        private bool isMoving = false; // 이동 중인지 여부를 나타내는 플래그
        private bool isRotating = false; // 회전 중인지 여부를 나타내는 플래그
        private float gridSize = 1f; // 그리드 한 칸의 크기
        private Vector3 targetPosition; // 이동할 목표 위치

        public GameObject furniturePopup;
        public GameObject uiPanel; // UI 패널 (이동, 회전, 고정 버튼 포함)
        public Button moveButton; // 이동 버튼
        public Button rotateButton; // 회전 버튼
        public Button fixButton; // 고정 버튼

        void Start()
        {
            // UI 버튼에 이벤트 추가
            moveButton.onClick.AddListener(OnMoveButtonClicked);
            rotateButton.onClick.AddListener(OnRotateButtonClicked);
            fixButton.onClick.AddListener(OnFixButtonClicked);
            uiPanel.SetActive(false); // UI 패널 숨김
        }

        void Update()
        {
            GameObject clickedBuilding = GetClickedBuilding();
            if (clickedBuilding != null && selectedObject == null)
            {
                selectedObject = clickedBuilding;
                uiPanel.SetActive(true); // UI 패널 보이기
            }

            if (selectedObject != null)
            {
                uiPanel.transform.position = selectedObject.transform.position + new Vector3(0, 3f, 0);
                if (isMoving)
                {
                    MoveSelectedObject();
                }

                if (isRotating)
                {
                    RotateObject();
                }
            }
        }

        // 마우스로 클릭된 건물 오브젝트 가져오는 함수
        private GameObject GetClickedBuilding()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject.GetComponent<FurniturePopup>() != null)
                    {
                        return hit.collider.gameObject;
                    }
                }
            }
            return null;
        }

        // 오브젝트 이동
        public void OnMoveButtonClicked()
        {
            isMoving = true;
            isRotating = false;
        }

        public void MoveSelectedObject()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                isMoving = true; // 이동 설정
                MoveObject(GetMoveDirection()); // 오브젝트 이동
                selectedObject.transform.position = targetPosition;
            }
        }

        public void MoveObject(Vector3 position)
        {
            targetPosition = SnapToGrid(selectedObject.transform.position + position * gridSize);
        }

        private Vector3 GetMoveDirection()
        {
            if (Input.GetKeyDown(KeyCode.W)) return Vector3.forward;
            if (Input.GetKeyDown(KeyCode.S)) return Vector3.back;
            if (Input.GetKeyDown(KeyCode.A)) return Vector3.left;
            if (Input.GetKeyDown(KeyCode.D)) return Vector3.right;
            return Vector3.zero;
        }

        // 오브젝트 회전
        public void OnRotateButtonClicked()
        {
            isRotating = true;
            isMoving = false;
        }

        private void RotateObject()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedObject.transform.Rotate(Vector3.up, -90);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                selectedObject.transform.Rotate(Vector3.up, 90);
            }
        }

        // 오브젝트 위치 고정
        public void OnFixButtonClicked()
        {
            // 현재 위치에서의 충돌 검사
            if (IsCollidingWithFurniture(selectedObject.transform.position))
            {
                Debug.Log("다른 Furniture 오브젝트와 충돌, 위치를 고정할 수 없습니다.");
                return;
            }

            isMoving = false; // 이동 중 플래그 해제
            isRotating = false; // 회전 중 플래그 해제
            selectedObject = null; // 선택된 오브젝트 초기화
            uiPanel.SetActive(false); // UI 패널 숨김
        }

        // 위치를 가까운 그리드로 맞추기
        private Vector3 SnapToGrid(Vector3 position)
        {
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize,
                position.y,
                Mathf.Round(position.z / gridSize) * gridSize
            );
        }



        // 선택된 오브젝트가 "Furniture" 태그가 있는 다른 오브젝트와 충돌하는지 확인하는 함수
        private bool IsCollidingWithFurniture(Vector3 targetPos)
        {
            // 주어진 위치에 있는 오브젝트를 검색하기 위한 박스 모양의 공간을 정의
            Collider[] colliders = Physics.OverlapBox(
                targetPos, // 검색할 위치
                selectedObject.transform.localScale, // 박스의 지름을 설정
                selectedObject.transform.rotation // 박스의 회전을 설정
            );

            // 충돌 검사에서 "Furniture" 태그가 있는 다른 오브젝트가 있는지 확인
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Furniture" && collider.gameObject != selectedObject)
                {
                    return true; // 다른 Furniture 오브젝트와 충돌
                }

            }
            return false; // 충돌 없음
        }
    }
}
