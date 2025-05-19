namespace Lop.Survivor.Island.Buildingbase
{
    using System.Collections;
    using System.Collections.Generic;
    using global::Lop.Survivor.Structures;
    using UnityEngine;

    public class TreesInteraction : Structure
    {
        public enum TreeType { Tree, FruitTree }
        public TreeType treeType;

        [Header("Status")]
        [SerializeField]
        private float currentHealth = 100f;                     // 나무의 현재 체력
        [SerializeField]
        private float maxHealth = 100f;                         // 나무의 최대 체력

        //public bool FruitTree => dropItemData.Any(item => item.itemID == "Fruit");

        protected override void Start()
        {
            base.Start();
        }

        // 나무 체력 업데이트 메서드 
        public void UpdateTreeHealth(float amount)
        {
            currentHealth += amount;

            // 체력가 0 이하로 떨어지면 아이템 드랍
            if (currentHealth <= 0 && treeType == TreeType.Tree)
            {
                DropItem(); // 아이템 드랍
            }

            else if (currentHealth <= 0 && treeType == TreeType.FruitTree)
            {
                DropFurit();
            }

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
