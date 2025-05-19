using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;   // 보스가 아래로 내려가는 속도

    [SerializeField] Transform[] bulletPos;  // 총알 생성 위치 배열
    [SerializeField] Transform tonguePos;
    [SerializeField] GameObject[] dangerLine; // 경고 라인 배열

    [SerializeField] GameObject bulletPrefab; // 총알 프리팹
    [SerializeField] GameObject tongue;       // tongue 객체

    [SerializeField] private float fireInterval = 2f;  // 총알 발사 간격
    [SerializeField] private float tongueInterval = 15f;  // tongue 생성 간격

    private float fireTimer;
    private float tongueTimer;

    Enemy enemy;

    private void Update()
    {
        if (GameManager.instance.isDamageTrue)
            return;

        // 보스가 아래로 내려가는 함수
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (CutSceneManager.instance.isCutScene)
            return;

        // 총알 발사 타이머 업데이트
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            FireRandomBullet();
            fireTimer = 0f;
        }

        // tongue 생성 타이머 업데이트
        tongueTimer += Time.deltaTime;
        if (tongueTimer >= tongueInterval)
        {
            SpawnTongue();
            tongueTimer = 0f;
        }
    }

    private void FireRandomBullet()
    {
        if (bulletPos.Length == 0 || dangerLine.Length == 0 || bulletPos.Length != dangerLine.Length)
        {
            Debug.LogWarning("bulletPos와 dangerLine 배열의 길이가 맞지 않습니다.");
            return;
        }

        // 랜덤 인덱스 선택
        int randomIndex = Random.Range(0, bulletPos.Length);

        // 해당 인덱스의 DangerLine 활성화
        dangerLine[randomIndex].SetActive(true);

        // 잠시 후 DangerLine 비활성화 코루틴 실행
        StartCoroutine(DeactivateDangerLineAfterDelay(dangerLine[randomIndex], 0.5f));

        // 해당 위치에 총알 생성
        Instantiate(bulletPrefab, bulletPos[randomIndex].position, Quaternion.identity);
    }

    private void SpawnTongue()
    {
        if (tongue != null)
        {
            // 현재 보스의 위치에서 tongue 생성
            Destroy(Instantiate(tongue, tonguePos.transform.position + new Vector3(0, -20, 0), Quaternion.identity), 7);
        }
        else
        {
            Debug.LogWarning("tongue 프리팹이 설정되지 않았습니다.");
        }
    }

    private System.Collections.IEnumerator DeactivateDangerLineAfterDelay(GameObject dangerLine, float delay)
    {
        yield return new WaitForSeconds(delay);
        dangerLine.SetActive(false);
    }
}
