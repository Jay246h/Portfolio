
using Org.BouncyCastle.Security;
using System.Collections;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class Treeinteraction : MonoBehaviour
    {
        [SerializeField] private int hitCount = 0;

        public string targetTag = "Player";
        public const string isHit = nameof(isHit);

        public float damageAmount = 10f;
        public float detectionRadius;

        [SerializeField] private bool isPenguinDetected = false;
        private bool isServer;

        public GameObject tree;

        [SerializeField] private Treeinteractionbutton treeInteractionButton;
        private DiggingWood diggingWood;
        [SerializeField] private Animator animator;


        private Penguin penguinScript;


        private Penguin penguin;
        private void Start()
        {
            StartCoroutine(FindPenguinAfterDelay());
        }

        // 1초 후 Penguin 객체를 찾기
        private IEnumerator FindPenguinAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            penguin = FindObjectOfType<Penguin>();
            isServer = NetworkObjectManager.Instance.isServer;
        }

        private void Update()
        {
            FindObjectWithTagInRange();
        }


        // DiggingWood 스크립트에서 나무를 때릴 때 호출되는 함수
        public void ChopTree()
        {

            if (isPenguinDetected)
            {
                animator.SetTrigger(BuildingTagAnim.isHit); // Chopping 애니메이션 실행
                hitCount++; // 나무 타격 횟수 증가

                InteractWithTree(); // 나무 상호작용 로직 실행
                if (hitCount >= 3)
                {
                    if (isServer) treeInteractionButton.DropItem();
                    hitCount = 0; // 타격 횟수 초기화
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
            treeInteractionButton.UpdateHealth(-damageAmount);

            if (treeInteractionButton.GetCurrentHealth() <= 0)
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
