namespace Lop.Survivor.Island.Buildingbase
{
    using System.Collections;
    using UnityEngine;

    public class FuritTreeInteraction : MonoBehaviour
    {
        [SerializeField] private int hitCount = 0;                  // 나무 타격 횟수

        public float damageAmount = 10f;                            // 나무가 받는 데미지
        public float detectionRadius;                               // 탐지 범위

        [SerializeField] private bool isPenguinDetected = false;    // 펭귄이 주변에 있는지 확인
        private bool isServer;

        public GameObject tree;

        [SerializeField] private FuritTreeIntercationbutton FurittreeIntercationbutton;
        private DiggingWood diggingWood;
        [SerializeField] private Animator animator;
        //private Penguin penguin; 

        private void Start()
        {
            StartCoroutine(FindPenguinAfterDelay());
        }

        // 1초 후 Penguin 객체를 찾기
        private IEnumerator FindPenguinAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            //penguin = FindObjectOfType<Penguin>();
            isServer = NetworkObjectManager.Instance.isServer;
        }

        private void Update()
        {
            FindObjectWithTagInRange();
        }

        // DiggingWood 스크립트에서 나무를 때릴 때 호출되는 함수
        public void ChopFuritTree()
        {
            if (isPenguinDetected)
            {
                animator.SetTrigger(BuildingTagAnim.isHit); // Chopping 애니메이션 실행
                hitCount++; // 나무 타격 횟수 증가

                InteractWithTree(); // 나무 상호작용 로직 실행

                // 과일이 아닌 아이템일 경우, 3번 타격 시 일반 아이템 드롭
                if (/*!treeCIntercationbutton.FruitTree && */hitCount >= 3)
                {
                    if (isServer) FurittreeIntercationbutton.DropItem(); // 일반 아이템 드롭
                    hitCount = 0; // 나무 타격 횟수 초기화
                }
            }
        }



        // 나무가 도끼에 닿았을 때 실행되는 함수
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<DiggingWood>())
            {
                diggingWood = other.gameObject.GetComponent<DiggingWood>();
                diggingWood.OffFelling();
            }
        }

        // 나무 체력관련 상호작용 함수
        private void InteractWithTree()
        {
            FurittreeIntercationbutton.UpdateHealth(-damageAmount);

            // 내구도가 0이 되면 나무 파괴
            if (FurittreeIntercationbutton.GetCurrentHealth() <= 0)
            {
                if (isServer) BuildingManager.Instance.DestroyBuilding(tree);
            }
        }

        // 플레이어를 태그로 찾는 함수
        private void FindObjectWithTagInRange()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            isPenguinDetected = false;

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag(BuildingTagAnim.Player))
                {
                    isPenguinDetected = true;
                    break;
                }
            }
        }
    }
}
