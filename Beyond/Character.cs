using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public static class TypeEffectiveness
{
    private static readonly Dictionary<string, Dictionary<string, float>> effectiveness = new Dictionary<string, Dictionary<string, float>>()
    {
        { "Fire", new Dictionary<string, float> { { "Fire", 0.5f }, { "Water", 0.5f }, { "Grass", 2.0f } } },
        { "Water", new Dictionary<string, float> { { "Fire", 2.0f }, { "Water", 0.5f }, { "Grass", 0.5f } } },
        { "Grass", new Dictionary<string, float> { { "Fire", 0.5f }, { "Water", 2.0f }, { "Grass", 0.5f } } }
        // 필요한 속성들 추가
    };

    public static float GetEffectiveness(string attackerType, string defenderType)
    {
        if (effectiveness.ContainsKey(attackerType) && effectiveness[attackerType].ContainsKey(defenderType))
        {
            return effectiveness[attackerType][defenderType];
        }
        return 1.0f; // 기본 효과는 1.0
    }
}

public abstract class Character : MonoBehaviour
{
    [Header("Status")]
    public float maxHp;
    public float curHp;
    public float dmg;
    public string attribute; // 캐릭터의 속성

    [Header("SpecialMove")]
    public float specialMove;
    public bool isSpecialMove = false;

    [Header("UI")]
    public TextMeshProUGUI level;
    public Slider hpSlider;
    //public GaugeManager gaugeManager;

    [Header("Gauge")]
    public int gauge = 0; // 현재 게이지 값



    protected virtual void OnEnable()
    {
        curHp = maxHp;
        UpdateHpUI();
    }

    private void Update()
    {
        // HP 바를 실시간으로 업데이트
        UpdateHpUI();
    }

    public void TakeDamage(float amount, bool isEnemy)
    {
        curHp -= amount;
        curHp = Mathf.Clamp(curHp, 0, maxHp); // HP가 0 이하로 떨어지지 않도록 제한
        UpdateHpUI();

        if (curHp <= 0)
        {
            OnDeath(isEnemy);
        }
    }

    public void DealDamage(Character target, bool isEnemy)
    {
        float effectiveness = TypeEffectiveness.GetEffectiveness(this.attribute, target.attribute);
        float damage = dmg * effectiveness;
        target.TakeDamage(damage, isEnemy);
    }

    public abstract void Attack();


    public abstract void SpecialMove();


    protected virtual void OnDeath(bool isEnemy)
    {
        if (isEnemy)
        {
            Debug.Log("몬스터 파괴");
            StageManager.Instance.EnemyDie(gameObject);
        }
        // 사망 시 처리 로직 (예: 캐릭터 비활성화, 사망 애니메이션 실행 등)
        Debug.Log($"{gameObject.name} has died.");
    }

    private void UpdateHpUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = curHp / maxHp;
        }
    }

    public void IncreaseGauge()
    {
        //if (gaugeManager != null)
        //{
        GaugeManager.Instance.IncreaseGauge(transform);
        //}
    }
}
