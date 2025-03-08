using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;  // 게임 오버 UI 패널
    public GameObject gameOverText;   // "GAME OVER" 텍스트
    public GameObject restartButton;  // "Restart" 버튼

    void Start()
    {
        // 게임 시작할 때 UI 요소들을 숨김
        gameOverPanel.SetActive(false);
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
        Time.timeScale = 1f; // 게임 속도 정상화
    }

    public void GameOver()
    {
        // 게임 오버 시 UI 요소들을 활성화
        gameOverPanel.SetActive(true);
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0f; // 게임 멈춤

        Cursor.lockState = CursorLockMode.None; // 마우스 커서 해제
        Cursor.visible = true; // 마우스 보이게 하기
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 씬 다시 로드
        Time.timeScale = 1f; // 게임 재개

        Cursor.lockState = CursorLockMode.Locked; // 마우스 다시 고정
        Cursor.visible = false; // 마우스 숨기기
    }

}
