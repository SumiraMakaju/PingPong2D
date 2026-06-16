using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public bool isTopGoal = true;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        BallController ball = other.GetComponent<BallController>();
        if (ball == null) ball = other.GetComponentInParent<BallController>();
        if (ball != null && ball.IsImmune()) return;

        if (isTopGoal)
            gameManager.BottomScores();
        else
            gameManager.TopScores();
    }
}
