using UnityEngine;
using System.Collections;

public class TestEnemy : Character
{
    public float specialMoveMultiplier = 2.0f; // 필살기 배율 (2배)
    public float attackMoveSpeed = 5.0f; // 공격 시 이동 속도

    private Vector3 originalPosition;
    private Transform targetEnemy;
    private bool isAttacking = false;
    private bool returningToOriginalPosition = false;

    void Update()
    {
        if (curHp <= 0)
        {
            gameObject.SetActive(false);
        }

        // 스페이스바를 누르면 공격 실행, 공격 중이거나 원래 위치로 돌아오는 중이면 공격 불가
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !returningToOriginalPosition)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private Transform FindLowestHpEnemy()
    {
        float minHp = Mathf.Infinity;
        Transform lowestHpEnemy = null;
        GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject Charactor in Player)
        {
            Knight enemyComponent = Charactor.GetComponent<Knight>();
            if (enemyComponent != null && enemyComponent.curHp < minHp)
            {
                minHp = enemyComponent.curHp;
                lowestHpEnemy = Charactor.transform;
            }
        }
        return lowestHpEnemy;
    }

    public override void Attack()
    {
        // 적의 공격 로직 구현
    }

    public override void SpecialMove()
    {
        // 적의 필살기 구현
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Knight player = other.GetComponent<Knight>();
            if (player != null)
            {
                DealDamage(player, false);
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true; // 공격 시작
        Transform closestEnemy = FindLowestHpEnemy();

        yield return new WaitForSeconds(0.1f);

        if (closestEnemy != null)
        {
            originalPosition = transform.position;
            targetEnemy = closestEnemy;

            // 적에게 이동
            while (Vector3.Distance(transform.position, targetEnemy.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, Time.deltaTime * attackMoveSpeed);
                yield return null;
            }

            // 적에게 도착 시 데미지 입히기
            Knight knight = targetEnemy.GetComponent<Knight>();
            if (knight != null)
            {
                DealDamage(knight, false);
            }

            // 원래 위치로 돌아가기
            returningToOriginalPosition = true; // 원래 위치로 돌아오는 중
            while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, Time.deltaTime * attackMoveSpeed);
                yield return null;
            }
            returningToOriginalPosition = false; // 위치 복귀 완료
        }

        isAttacking = false; // 공격 종료, 원래 위치로 돌아온 후에만 공격 가능
    }

    private IEnumerator SpecialMoveCoroutine()
    {
        float originalDmg = dmg; // 원래 공격력을 저장
        dmg *= specialMoveMultiplier; // 공격력 2배로 증가

        // 필살기 공격 실행
        yield return StartCoroutine(PerformAttack());

        // 공격력 원상 복구
        dmg = originalDmg;
        gauge = 0; // 게이지 초기화

        // 게이지 UI 업데이트
        GaugeManager.Instance.ResetGauge(this.transform);
    }
}
