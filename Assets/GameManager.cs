using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;  // ���� ���� UI �г�
    public GameObject gameOverText;   // "GAME OVER" �ؽ�Ʈ
    public GameObject restartButton;  // "Restart" ��ư

    void Start()
    {
        // ���� ������ �� UI ��ҵ��� ����
        gameOverPanel.SetActive(false);
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
        Time.timeScale = 1f; // ���� �ӵ� ����ȭ
    }

    public void GameOver()
    {
        // ���� ���� �� UI ��ҵ��� Ȱ��ȭ
        gameOverPanel.SetActive(true);
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
        Time.timeScale = 0f; // ���� ����

        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� ����
        Cursor.visible = true; // ���콺 ���̰� �ϱ�
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // �� �ٽ� �ε�
        Time.timeScale = 1f; // ���� �簳

        Cursor.lockState = CursorLockMode.Locked; // ���콺 �ٽ� ����
        Cursor.visible = false; // ���콺 �����
    }

}
