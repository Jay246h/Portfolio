using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountingEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10f)]
    private float effectTime;       // 카운팅 되는 시간

    private TextMeshProUGUI effectText;     // 카운팅 효과에 사용되는 텍스트

    private void Awake()
    {
        effectText = GetComponent<TextMeshProUGUI>();
    }

    public void Play(int start, int end, UnityAction action = null)
    {
        StartCoroutine(Process(start, end, action));
    }

    private IEnumerator Process(int start, int end, UnityAction action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / effectTime;

            effectText.text = Mathf.Lerp(start, end, percent).ToString("F0");

            yield return null;
        }

        // action이 null이 아니면 action에 저장되어 있는 메소드 실행
        action?.Invoke();
    }
}
