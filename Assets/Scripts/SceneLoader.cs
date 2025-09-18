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

    // Start나 Update 메서드는 현재 씬 전환 기능에 필요하지 않으므로 비워둡니다.
    // 필요하다면 추후에 기능을 추가할 수 있습니다.
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}