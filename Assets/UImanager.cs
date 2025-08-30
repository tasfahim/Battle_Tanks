using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject missionCompletePanel;

    [Header("Default Buttons")]
    public GameObject playButton;
    public GameObject retryButton;
    public GameObject restartFromSaveButton;
    public GameObject nextMissionButton;

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (missionCompletePanel != null) missionCompletePanel.SetActive(false);

        if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            Time.timeScale = 0f;
            FindObjectOfType<TankController>()?.DisableControl();

            if (playButton != null) SelectButton(playButton);
        }
        else
        {
            Time.timeScale = 1f;
            FindObjectOfType<TankController>()?.EnableControl(); // ✅ auto-enable if no menu
        }
    }

    public void StartGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        TankController tc = FindObjectOfType<TankController>();
        if (tc != null) tc.EnableControl(); // ✅ enable tank when play pressed
    }

    public void LoadGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        TankController tc = FindObjectOfType<TankController>();
        if (tc != null) tc.EnableControl(); // ✅ enable tank when load pressed

        FindObjectOfType<SaveSystem>()?.LoadGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game pressed");

#if UNITY_EDITOR
        // ✅ Stop Play Mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // ✅ Close the application in a built game
    Application.Quit();
#endif
    }


    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        TankController tc = FindObjectOfType<TankController>();
        if (tc != null) tc.DisableControl();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (retryButton != null) SelectButton(retryButton);
    }

    public void ShowMissionComplete()
    {
        if (missionCompletePanel != null) missionCompletePanel.SetActive(true);
        Time.timeScale = 0f;

        TankController tc = FindObjectOfType<TankController>();
        if (tc != null) tc.DisableControl();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (nextMissionButton != null) SelectButton(nextMissionButton);
    }

    public void Retry()
    {
        Debug.Log("Retry button clicked!");
        Time.timeScale = 1f;

        // ✅ reload and re-enable tank
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartFromSave()
    {
        Debug.Log("Restart from Save clicked!");
        Time.timeScale = 1f;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        PlayerHealth player = FindObjectOfType<PlayerHealth>(true);
        if (player != null) player.gameObject.SetActive(true);

        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>(true);
        foreach (var e in enemies) e.gameObject.SetActive(true);

        TankController tc = FindObjectOfType<TankController>(true);
        if (tc != null) tc.EnableControl(); // ✅ ensure tank re-enabled

        SaveSystem saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem != null) saveSystem.LoadGame();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        // Reactivate the main menu panel inside this scene
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        // Hide other panels
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (missionCompletePanel != null) missionCompletePanel.SetActive(false);

        Time.timeScale = 0f;

        TankController tc = FindObjectOfType<TankController>();
        if (tc != null) tc.DisableControl();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
