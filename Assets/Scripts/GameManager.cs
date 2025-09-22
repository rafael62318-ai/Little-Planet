using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public AudioManager audioManager;
    private HomeBase homeBase;
    private bool gameEnded = false;

    // 💡 Inspector에서 설정할 씬 이름 변수 추가
    public string winSceneName = "ClearScene"; 
    public string loseSceneName = "GameOverScene";


    void Start()
    {
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        homeBase = FindObjectOfType<HomeBase>();
        if (homeBase == null)
        {
            Debug.LogError("씬에 HomeBase 스크립트를 가진 오브젝트가 없습니다! HomeBase 스크립트를 Planet Model 오브젝트에 추가해주세요.");
        }
    }

    void Update()
    {
        if (gameEnded) return; // 게임이 이미 끝났다면 더 이상 업데이트하지 않음

        // 1. 게임 패배 조건 확인: 본진 체력이 0이 되었을 때 (에너지 100% 미만)
        if (homeBase != null && homeBase.currentHealth <= 0 && homeBase.currentEnergy < homeBase.maxEnergy)
        {
            EndGame(GameResult.Lose);
            return; // 패배했으므로 함수 종료
        }
        
        // 2. 게임 승리 조건 확인
        // 본진의 에너지가 100%가 되었을 때
        if (homeBase != null && homeBase.currentEnergy >= homeBase.maxEnergy)
        {
            EndGame(GameResult.Win);
        }
        // 또는 모든 적이 처치되었을 때
        else if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            EndGame(GameResult.Win);
        }
    }

    // 게임 종료 로직을 실행하는 메서드
    public void EndGame(GameResult result)
    {
        if (gameEnded) return;

        gameEnded = true; // 게임이 끝났음을 표시

        // ⚠️ 씬 로드 전에 시간을 다시 정상화 (매우 중요)
        Time.timeScale = 1f;

        // AudioManager를 통해 결과 BGM을 재생합니다.
        if (audioManager != null)
        {
            audioManager.PlayEndGameBGM(result);
        }

        Debug.Log("게임 종료! 결과: " + result);
        
        // 🏆 결과에 따라 씬을 로드합니다.
        if (result == GameResult.Win)
        {
            SceneManager.LoadScene("GameClear Scence");
        }
        else // GameResult.Lose
        {
            SceneManager.LoadScene("GameOver Scence");
        }
    }
}