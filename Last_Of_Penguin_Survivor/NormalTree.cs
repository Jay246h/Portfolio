namespace Lop.Survivor.Island.Buildingbase
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NormalTree : Tree
    {


        public void TriggerChopTree()
        {
            ChopTree();
        }

        protected override void ChopTree()
        {
            if (isPenguinDetected)
            {
                animator.SetTrigger(BuildingTagAnim.isHit); // chopping 애니메이션 실행
                hitCount++; // 나무 타격 횟수 증가

                DamageTree();

                if (hitCount >= 3)
                {
                    if (isServer) SetDrop();
                    hitCount = 0; // 타격 횟수 초기화
                }
            }


        }

        protected override void UpdateTreeHealth(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                DropItem();
            }
        }
    }
}
