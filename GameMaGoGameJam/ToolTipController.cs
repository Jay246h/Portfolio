using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ToolTip toolTip;   // 툴팁을 나타낼 UI 오브젝트
    private Item item;         // 툴팁을 표시할 Item 객체를 직접 할당

    public void ItemAdd(Item newitem)
    {
        item = newitem;
    }

    // 마우스가 아이템 위에 올려졌을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            toolTip.gameObject.SetActive(true);  // 툴팁 활성화
            toolTip.SetupTooltip(item.itemName, item.Description);  // 툴팁 설정
        }
    }

    // 마우스가 아이템에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);  // 툴팁 비활성화
    }
}
