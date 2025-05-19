namespace Lop.Survivor.Island.Buildingbase
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    namespace Lop.Survivor.Island.Basicbuildingscript
    {
        public class TreesBone : MonoBehaviour
        {
            private NormalTree normalTree;
            private FruitTree fruitTree;

            void Start()
            {
                normalTree = GetComponentInParent<NormalTree>();
                fruitTree = GetComponentInParent<FruitTree>();
            }

            // Trees 스크립트에 있는 ChopTree 함수를 불러오는 함수
            public void NormalChop()
            {
                normalTree.TriggerChopTree();
            }

            public void FruitChop()
            {
                fruitTree.TriggerChopFruitTree();
            }
        }

    }
}
