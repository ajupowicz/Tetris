using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

 
public class SceneLoader : MonoBehaviour
{

    [Header("Game Over UI Panels")]
    public GameObject player1GameOverPanel;
    public GameObject player2GameOverPanel;


    [Header("Score Display")]
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    private int player1Score = 0;
    private int player2Score = 0;

    void Start()
    {
        if (!SceneManager.GetSceneByName("GameLogicScene").isLoaded)
        {
            SceneManager.LoadSceneAsync("GameLogicScene", LoadSceneMode.Additive).completed += (op) =>
            {
                OnSceneLoaded();
            };
        }
    }

    void OnSceneLoaded()
    {

        var canvas = FindObjectOfType<Canvas>();
        var cam = Camera.main;

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = cam;
        }

        var boards = FindObjectsOfType<GameBoard>();
        foreach (var board in boards)
        {
            board.OnGameOverTriggered += HandleGameOver;
            board.OnGameOverTriggered += HandleGameOver;
            board.OnScoreEarned += HandleScoreEarned;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("PresentationScene"));

        BlockSpawner spawner = FindObjectOfType<BlockSpawner>();
        ManagerUI ui = FindObjectOfType<ManagerUI>();

        if (spawner != null && ui != null)
        {
            ui.SetBlockSpawner(spawner);
        }
    }

    void HandleGameOver(string playerTag)
    {
        Debug.Log($"HandleGameOver called for: {playerTag}");

        if (playerTag == "Player1" && player1GameOverPanel != null)
        {
            player1GameOverPanel.SetActive(true);
            Debug.Log("Player1 Game Over panel activated.");
        }
        else if (playerTag == "Player2" && player2GameOverPanel != null)
        {
            player2GameOverPanel.SetActive(true);
            Debug.Log("Player2 Game Over panel activated.");
        }
        else
        {
            Debug.LogWarning("No Game Over panel assigned for " + playerTag);
        }
    }
    void HandleScoreEarned(string playerTag, int points)
    {
        if (playerTag == "Player1")
        {
            player1Score += points;
            if (player1ScoreText != null)
                player1ScoreText.text = player1Score.ToString();
        }
        else if (playerTag == "Player2")
        {
            player2Score += points;
            if (player2ScoreText != null)
                player2ScoreText.text = player2Score.ToString();
        }
    }

}


