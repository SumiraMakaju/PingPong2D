using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 6f;
    public float speedIncrement = 0.3f;

    private Rigidbody2D rb;
    private float currentSpeed;
    private bool immune = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = initialSpeed;
        StartCoroutine(LaunchWithDelay());
    }

    System.Collections.IEnumerator LaunchWithDelay()
    {
        immune = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        immune = false;
        LaunchBall();
    }

    void LaunchBall()
    {
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.value > 0.5f ? 1f : -1f;
        rb.linearVelocity = new Vector2(x, y).normalized * currentSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Paddle") ||
            (collision.transform.parent != null &&
             collision.transform.parent.CompareTag("Paddle")))
        {
            currentSpeed += speedIncrement;
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
    }

    public bool IsImmune() => immune;

    public void ResetBall()
    {
        StopAllCoroutines();
        transform.position = Vector2.zero;
        currentSpeed = initialSpeed;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(LaunchWithDelay());
    }

    public void StopBall()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        immune = true;
    }
}
