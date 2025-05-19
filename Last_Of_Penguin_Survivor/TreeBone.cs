using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class TreeBone : MonoBehaviour
    {
        private Treeinteraction treeinteraction;

        private void Start()
        {
            treeinteraction = GetComponentInParent<Treeinteraction>();
        }

        // treeinteraction 스크립트에 ChopTree 함수를 불러오는 함수
        public void Chop()
        {
            treeinteraction.ChopTree();
        }
    }
}
