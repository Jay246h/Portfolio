using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HpBarManager : MonoBehaviour
{
    [SerializeField]
    private Slider m_goPrefabs = null; // 프리팹으로 설정된 HP 바 슬라이더

    private List<Transform> m_objectList = new List<Transform>();
    private List<Slider> m_hpBarList = new List<Slider>();

    private Camera m_cam = null;

    private void Start()
    {
        m_cam = Camera.main;

        // "Player" 태그를 가진 객체들 추가
        AddObjectsWithTag("Player");

        // "Enemy" 태그를 가진 객체들 추가
        AddObjectsWithTag("Enemy");
    }

    private void Update()
    {
        for (int i = 0; i < m_objectList.Count; i++)
        {
            Character character = m_objectList[i].GetComponent<Character>();
            if (character != null)
            {
                // HP가 0 이하일 경우 체력 바 제거
                if (character.curHp <= 0)
                {
                    Destroy(m_hpBarList[i].gameObject); // HP 바 제거
                    m_objectList.RemoveAt(i); // 리스트에서 캐릭터 제거
                    m_hpBarList.RemoveAt(i); // 리스트에서 HP 바 제거
                    i--; // 인덱스를 하나 감소시켜 리스트 크기 조정
                    continue;
                }

                // HP 바 위치 업데이트
                m_hpBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + new Vector3(0, 2, 0));

                // HP 값 업데이트
                m_hpBarList[i].value = character.curHp / character.maxHp;
            }
        }
    }

    private void AddObjectsWithTag(string tag)
    {
        GameObject[] t_objects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < t_objects.Length; i++)
        {
            m_objectList.Add(t_objects[i].transform);
            Slider t_hpbar = Instantiate(m_goPrefabs, transform); // 부모를 캔버스로 설정
            m_hpBarList.Add(t_hpbar);
        }
    }
}
