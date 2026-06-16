using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI scoreText;
    public BallController ball;
    public PaddleController topPaddle;

    [Header("UI")]
    public GameObject restartButton;
    public GameObject modePanel;
    public TextMeshProUGUI modeButtonText;
    public TextMeshProUGUI controlsHintText;

    [Header("Settings")]
    public int winScore = 5;

    private int topScore = 0;
    private int bottomScore = 0;
    private bool gameOver = false;
    private bool isTwoPlayer = true;

    void Start()
    {
        Button restartBtn = restartButton.GetComponent<Button>();
        if (restartBtn != null)
            restartBtn.onClick.AddListener(Restart);

      
        if (modePanel != null)
        {
            Button modeBtn = modePanel.GetComponent<Button>();
            if (modeBtn != null)
                modeBtn.onClick.AddListener(ToggleMode);
        }

        restartButton.SetActive(false);
        UpdateScoreUI();
        ApplyMode();
    }

    public void TopScores()
    {
        if (gameOver) return;
        topScore++;
        UpdateScoreUI();
        if (!CheckWin()) ball.ResetBall();
    }

    public void BottomScores()
    {
        if (gameOver) return;
        bottomScore++;
        UpdateScoreUI();
        if (!CheckWin()) ball.ResetBall();
    }

    void UpdateScoreUI()
    {
        scoreText.text = topScore + " : " + bottomScore;
    }

    bool CheckWin()
    {
        if (topScore >= winScore)
        {
            scoreText.text = "player 1 WINS!";
            EndGame();
            return true;
        }
        if (bottomScore >= winScore)
        {
            scoreText.text = "player 2 WINS!";
            EndGame();
            return true;
        }
        return false;
    }

    void EndGame()
    {
        gameOver = true;
        ball.StopBall();
        restartButton.SetActive(true);
    }

    public void Restart()
    {
        topScore = 0;
        bottomScore = 0;
        gameOver = false;
        restartButton.SetActive(false);
        UpdateScoreUI();
        ball.ResetBall();
    }

    public void ToggleMode()
    {
        isTwoPlayer = !isTwoPlayer;
        ApplyMode();
       
        Restart();
    }

    void ApplyMode()
    {
        if (topPaddle != null)
        {
            topPaddle.SetAI(!isTwoPlayer);
        }

        if (modeButtonText != null)
        {
            modeButtonText.text = isTwoPlayer ? "2P" : "1P";
        }

        if (controlsHintText != null)
        {
            controlsHintText.text = isTwoPlayer
                ? "Top: A/D | Bottom: Arrow Keys"
                : "Bottom: Arrow Keys";
        }
    }
}
