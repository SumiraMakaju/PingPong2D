using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PaddleController : MonoBehaviour
{
    public float speed = 8f;
    public bool isAI = false;
    public bool isTopPaddle = false;
    public Transform ballTransform;

    private float clampX = 2.3f;

    void Update()
    {
        float input = 0f;

        if (isAI)
            input = GetAIInput();
        else
            input = GetPlayerInput();

        Vector3 pos = transform.position;
        pos.x += input * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -clampX, clampX);
        transform.position = pos;
    }

    float GetPlayerInput()
    {
        float input = 0f;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (isTopPaddle)
            {
                if (kb.aKey.isPressed) input = -1f;
                if (kb.dKey.isPressed) input = 1f;
            }
            else
            {
                if (kb.leftArrowKey.isPressed) input = -1f;
                if (kb.rightArrowKey.isPressed) input = 1f;
            }
            if (input != 0f) return input;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (isTopPaddle)
        {
            if (Input.GetKey(KeyCode.A)) input = -1f;
            if (Input.GetKey(KeyCode.D)) input = 1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow)) input = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) input = 1f;
        }
#endif

        return input;
    }

    float GetAIInput()
    {
        if (ballTransform == null) return 0f;
        float diff = ballTransform.position.x - transform.position.x;
        return Mathf.Clamp(diff, -1f, 1f);
    }

    public void SetAI(bool aiOn)
    {
        isAI = aiOn;
    }
}
