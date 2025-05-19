namespace Lop.Survivor.Island.Buildingbase
{
    using System.Collections;
    using System.Collections.Generic;
    using global::Lop.Survivor.Structures;
    using UnityEngine;
    using static global::Lop.Survivor.Island.Buildingbase.TreesInteraction;
    using static UnityEngine.RuleTile.TilingRuleOutput;

    public class Tree : Structure
    {

        [Header("Status")]
        [SerializeField]
        public float currentHealth = 100f;                     // 나무의 현재 체력
        [SerializeField]
        private float maxHealth = 100f;                         // 나무의 최대 체력


        [SerializeField] public int hitCount = 0;                  // 나무 타격 횟수

        public float damageAmount = 10f;                            // 나무가 받는 데미지
        public float detectionRadius;                               // 탐지 범위

        [SerializeField] public bool isPenguinDetected = false;    // 펭귄이 주변에 있는지 확인
        public bool isServer;

        public GameObject tree;

        private NormalTree normalTree;
        private FruitTree fruitTree;
        private DiggingWood diggingWood;
        [SerializeField] public Animator animator;
        private PenguinBody penguin;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(FindPenguinAfterDelay());
        }

        // 1초 후 Penguin 객체를 찾기
        private IEnumerator FindPenguinAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            penguin = FindObjectOfType<PenguinBody>();
            isServer = NetworkObjectManager.Instance.isServer;
        }

        private void Update()
        {
            FindObjectWithTagInRange();
        }

        // DiggingWood 스크립트에서 나무를 때릴 때 호출되는 함수
        protected virtual void ChopTree()
        {

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
        public void DamageTree()
        {
            normalTree.UpdateTreeHealth(-damageAmount);

            if (GetCurrentHealth() <= 0)
            {
                if (isServer) BuildingManager.Instance.DestroyBuilding(tree);
            }
        }

        // 열매 나무 체력관련 상호작용 함수
        public void DamageFuritTree()
        {
            fruitTree.UpdateTreeHealth(-damageAmount);

            // 내구도가 0이 되면 나무 파괴
            if (GetCurrentHealth() <= 0)
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

        protected virtual void UpdateTreeHealth(float amount)
        {

        }

        // 현재 체력 반환 함수
        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        // 아이템 드랍 함수
        public void SetDrop()
        {
            DropItem();
        }


    }
}
