using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class FuritTreeBone : MonoBehaviour
    {
        private FuritTreeInteraction treeCInteraction;

        private void Start()
        {
            treeCInteraction = GetComponentInParent<FuritTreeInteraction>();
        }

        /// treeCInteraction 스크립트에 ChopFuritTree 함수를 불러오는 함수
        public void FuritChop()
        {
            treeCInteraction.ChopFuritTree();
        }


    }


}
