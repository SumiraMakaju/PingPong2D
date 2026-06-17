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

    private int activeTouchId = -1;
    private bool isDragging = false;

    void Update()
    {
        Vector3 pos = transform.position;

        if (isAI)
        {
            float input = GetAIInput();
            pos.x += input * speed * Time.deltaTime;
        }
        else
        {
            if (UpdateDragInput(ref pos))
            {
               
            }
            else
            {
                float input = GetKeyboardInput();
                pos.x += input * speed * Time.deltaTime;
            }
        }

        pos.x = Mathf.Clamp(pos.x, -clampX, clampX);
        transform.position = pos;
    }

    private bool IsPointerOverUI(int pointerId)
    {
        if (UnityEngine.EventSystems.EventSystem.current == null) return false;
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    private bool UpdateDragInput(ref Vector3 pos)
    {
        Camera cam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();
        if (cam == null) return false;

        bool isPointerDown = false;
        Vector2 pointerScreenPos = Vector2.zero;
        bool startedThisFrame = false;
        bool endedThisFrame = false;
        int pointerId = -1;

#if ENABLE_INPUT_SYSTEM
        var touchscreen = Touchscreen.current;
        if (touchscreen != null)
        {
            foreach (var touch in touchscreen.touches)
            {
                int tId = touch.touchId.ReadValue();
                if (isDragging && tId == activeTouchId)
                {
                    var phase = touch.phase.ReadValue();
                    if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                    {
                        endedThisFrame = true;
                    }
                    else
                    {
                        isPointerDown = true;
                        pointerScreenPos = touch.position.ReadValue();
                    }
                    break;
                }
                else if (!isDragging && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    isPointerDown = true;
                    pointerScreenPos = touch.position.ReadValue();
                    startedThisFrame = true;
                    pointerId = tId;
                    break;
                }
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (!isPointerDown && !endedThisFrame && !startedThisFrame && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                int tId = touch.fingerId;
                if (isDragging && tId == activeTouchId)
                {
                    if (touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled)
                    {
                        endedThisFrame = true;
                    }
                    else
                    {
                        isPointerDown = true;
                        pointerScreenPos = touch.position;
                    }
                    break;
                }
                else if (!isDragging && touch.phase == UnityEngine.TouchPhase.Began)
                {
                    isPointerDown = true;
                    pointerScreenPos = touch.position;
                    startedThisFrame = true;
                    pointerId = tId;
                    break;
                }
            }
        }
#endif

        if (!isPointerDown && !endedThisFrame && !startedThisFrame)
        {
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse != null)
            {
                if (isDragging && activeTouchId == 999)
                {
                    if (!mouse.leftButton.isPressed)
                    {
                        endedThisFrame = true;
                    }
                    else
                    {
                        isPointerDown = true;
                        pointerScreenPos = mouse.position.ReadValue();
                    }
                }
                else if (!isDragging && mouse.leftButton.wasPressedThisFrame)
                {
                    isPointerDown = true;
                    pointerScreenPos = mouse.position.ReadValue();
                    startedThisFrame = true;
                    pointerId = 999;
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (!isPointerDown && !endedThisFrame && !startedThisFrame)
            {
                if (isDragging && activeTouchId == 999)
                {
                    if (!Input.GetMouseButton(0))
                    {
                        endedThisFrame = true;
                    }
                    else
                    {
                        isPointerDown = true;
                        pointerScreenPos = Input.mousePosition;
                    }
                }
                else if (!isDragging && Input.GetMouseButtonDown(0))
                {
                    isPointerDown = true;
                    pointerScreenPos = Input.mousePosition;
                    startedThisFrame = true;
                    pointerId = 999;
                }
            }
#endif
        }

        
        if (isDragging)
        {
            if (endedThisFrame)
            {
                isDragging = false;
                activeTouchId = -1;
                return false;
            }

            if (isPointerDown)
            {
                Vector3 screenPos = new Vector3(pointerScreenPos.x, pointerScreenPos.y, Mathf.Abs(transform.position.z - cam.transform.position.z));
                pos.x = cam.ScreenToWorldPoint(screenPos).x;
                return true;
            }
        }
        else if (startedThisFrame)
        {
            float normalizedY = pointerScreenPos.y / Screen.height;
            if ((isTopPaddle && normalizedY > 0.5f) || (!isTopPaddle && normalizedY <= 0.5f))
            {
               
                if (IsPointerOverUI(pointerId == 999 ? -1 : pointerId))
                {
                    return false;
                }

                isDragging = true;
                activeTouchId = pointerId;
                Vector3 screenPos = new Vector3(pointerScreenPos.x, pointerScreenPos.y, Mathf.Abs(transform.position.z - cam.transform.position.z));
                pos.x = cam.ScreenToWorldPoint(screenPos).x;
                return true;
            }
        }

        return false;
    }

    float GetKeyboardInput()
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
