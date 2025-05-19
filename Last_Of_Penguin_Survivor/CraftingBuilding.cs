using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Survivor.Island.Buildingbase
{
    public class CraftingBuilding : BuildingFrame
    {
        private CraftingStation craftBox;


        protected override void Start()
        {
            base.Start();
            // 씬에서 첫 번째 CraftBox를 찾습니다.
            craftBox = LopNetworkManager.GetPlayer().GetComponentInChildren<CraftingStation>();

            if (craftBox == null)
            {
                Debug.LogError("CraftBox component not found in the scene.");
            }
        }

        // CraftBox의 제작 관련 기능을 호출하는 함수
        private void ActivateCraftingUI()
        {
            if (craftBox != null)
            {
                // craft_tier를 2로 설정
                craftBox.craft_tier = 2; // 여기에 추가
                // UI 초기화
                craftBox.InitCombineTab();
                craftBox.InitCraftUI();

                // 제작 패널 활성화
                craftBox.craftingPanel.SetActive(true);
                craftBox.inventoryHandler.currentState = InventoryHandlerState.Crafting;

                //Debug.Log("Crafting UI activated and craft_tier set to 2.");
            }
        }

        // 제작 패널 비활성화 하는 함수
        private void DeactivateCraftingUI()
        {
            if (craftBox != null)
            {
                // 제작 패널 비활성화
                craftBox.CraftingOff();

                // 기타 초기화 작업
                craftBox.combineTabIndex = 0;
                craftBox.currentSlotIndex = 0;

                //Debug.Log("Crafting UI deactivated.");
            }
        }

        // OpenTab 함수에서 UI 활성화/비활성화
        protected override void OpenTeb()
        {
            if (craftBox != null)
            {
                if (craftBox.craftingPanel.activeSelf)
                {
                    // 패널이 이미 열려 있다면 비활성화
                    DeactivateCraftingUI();
                }
                else
                {
                    // 패널이 비활성화 되어 있다면 활성화
                    ActivateCraftingUI();
                }
            }
        }
    }
}
