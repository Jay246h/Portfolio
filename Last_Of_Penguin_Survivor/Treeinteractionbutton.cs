namespace Lop.Survivor.Island.Buildingbase
{
    using System.Linq;
    using global::Lop.Survivor.Structures;
    using UnityEngine;
    using UnityEngine.UI;

    public class Treeinteractionbutton : Structure
    {
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private float masHealth = 100f;


        private Treeinteraction treeinteraction;

        protected override void Start()
        {
            base.Start();
        }


        public void UpdateHealth(float amount)
        {

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, masHealth);

            if (currentHealth <= 0)
            {
                DropItem();
            }
        }


        public float GetCurrentHealth()
        {
            return currentHealth;
        }

    }
}
