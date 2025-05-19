namespace Lop.Survivor.Island.Buildingbase
{
    using System.Linq;
    using global::Lop.Survivor.Structures;
    using UnityEngine;
    using UnityEngine.UI;

    public class FuritTreeIntercationbutton : Structure
    {
        [SerializeField]
        private float currentHealth = 100f;                     // 열매 나무의 현재 체력
        [SerializeField]
        private float maxHealth = 100f;                         // 열매 나무의 최대 체력

        //public bool FruitTree => dropItemData.Any(item => item.itemID == "Fruit");


        protected override void Start()
        {
            base.Start();
        }

        // 나무 체력관련 함수
        public void UpdateHealth(float amount)
        {
            currentHealth += amount;

            // 체력가 0 이하로 떨어지면 아이템 드랍
            if (currentHealth <= 0)
            {
                DropFurit(); // 과일 아이템 드랍
            }
        }
        // 현재 체력 반환 함수
        public float GetCurrentHealth()
        {
            return currentHealth;
        }


    }
}
