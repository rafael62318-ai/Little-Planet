using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab; // 폭발 파티클 프리팹
    [SerializeField] AudioClip explosionSound;   // 폭발 사운드
    [SerializeField] float destroyDelay = 0.5f;  // 오브젝트 삭제 지연

    AudioSource audioSource;

    void Awake()
    {
        // Enemy 프리팹에 AudioSource가 없으면 자동 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {Debug.Log($"{gameObject.name} triggered with {other.name}"); // 로그 찍기
        if (other.CompareTag("HomeBase"))
        {
            PlayExplosion();
        }
    }

    void PlayExplosion()
    {
        // 1) 폭발 이펙트 생성
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2) 폭발 사운드 재생
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 3) 유닛 제거 (사운드가 자기 AudioSource에서 나오면 딜레이 후 제거)
        Destroy(gameObject, destroyDelay);
    }
}
