using UnityEngine;

// 게임 결과를 정의하는 열거형입니다.
// 이 코드는 public class AudioManager 위에 위치해야 합니다.
public enum GameResult
{
    Win,
    Lose
}

public class AudioManager : MonoBehaviour
{
    // 오디오 소스 컴포넌트 참조 변수
    private AudioSource audioSource;

    [Header("배경음악 설정")]
    [SerializeField] private AudioClip mainBGM;
    
    [Header("게임 결과 음악 설정")]
    [SerializeField] private AudioClip winBGM;
    [SerializeField] private AudioClip loseBGM;

    void Awake()
    {
        // 이 오브젝트에 있는 AudioSource 컴포넌트를 찾아서 할당합니다.
        audioSource = GetComponent<AudioSource>();

        // 만약 AudioSource 컴포넌트가 없다면 경고 메시지를 띄웁니다.
        if (audioSource == null)
        {
            Debug.LogError("AudioManager에 AudioSource 컴포넌트가 없습니다. 추가해주세요!");
        }
    }

    void Start()
    {
        // 게임 시작 시 메인 BGM을 재생합니다.
        PlayMainBGM();
    }

    // 메인 배경음악을 재생하는 메서드
    public void PlayMainBGM()
    {
        if (audioSource != null && mainBGM != null)
        {
            audioSource.clip = mainBGM;
            audioSource.loop = true; // 메인 BGM은 반복 재생합니다.
            audioSource.Play();
        }
    }

    // 게임 종료 시 결과를 받아 다른 음악을 재생하는 메서드
    public void PlayEndGameBGM(GameResult result)
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // 현재 재생 중인 음악을 멈춥니다.
            audioSource.loop = false; // 결과 음악은 한 번만 재생합니다.

            if (result == GameResult.Win)
            {
                if (winBGM != null)
                {
                    audioSource.clip = winBGM;
                    audioSource.Play();
                }
            }
            else if (result == GameResult.Lose)
            {
                if (loseBGM != null)
                {
                    audioSource.clip = loseBGM;
                    audioSource.Play();
                }
            }
        }
    }
}