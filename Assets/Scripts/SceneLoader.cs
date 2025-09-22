using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요합니다.
using UnityEngine.UI; // UI 컴포넌트에 접근할 경우 필요할 수 있습니다.

public class SceneLoader : MonoBehaviour
{
    // 씬을 불러오는 공개 메서드
    // Inspector에서 버튼 이벤트에 연결할 수 있도록 public으로 선언합니다.
    public void LoadLittlePlanetScene()
    {
        // SceneManager.LoadScene 메서드를 사용하여 "Little-Planet Scence" 씬을 불러옵니다.
        // 씬 이름이 정확히 "LIttle-Planet Scence"인지 확인해 주세요. 
        // 오타가 있으면 씬 로딩에 실패합니다.
        SceneManager.LoadScene("LIttle-Planet Scence");
    }

    // 💡 (추가) StartScene으로 돌아가는 메서드
    public void LoadStartScene()
    {
        // ⚠️ 게임 내에서 메인 씬으로 돌아갈 때는 시간을 정상화(1f)하는 것이 중요합니다.
        // 클리어/게임 오버 씬에서 돌아올 때 Time.timeScale이 0일 수 있습니다.
        Time.timeScale = 1f;

        // StartScene의 이름이 "Start Scence"라고 가정하고 로드합니다.
        SceneManager.LoadScene("Main Scence");
    }

    // Inspector에서 Exit 버튼 이벤트에 연결할 수 있도록 public으로 선언합니다.
    public void QuitGame()
    {
        // ⚠️ 에디터와 빌드 환경에 따라 다르게 작동합니다.

        // 1. 유니티 에디터에서 실행 중일 때: 
        // Play 모드를 종료합니다. (실제 빌드에서는 작동하지 않음)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        // 2. 빌드된 게임(PC, 모바일 등)에서 실행 중일 때: 
        // 애플리케이션을 종료합니다.
        Application.Quit();

        // 디버깅 용: 종료 메시지를 출력합니다. (에디터/빌드 공통)
        Debug.Log("게임이 종료됩니다.");
    }
}