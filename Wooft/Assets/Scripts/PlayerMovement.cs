using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public Rigidbody2D rb;
    protected PlayerAnimation playerAnimation;
    protected float moveH, moveV;

    [SerializeField]
    protected static float moveSpeed = 0.6f;
    [SerializeField]
    public static float walkSpeed = 0.6f;
    [SerializeField]
    public static float runSpeed = 1.0f;
    [SerializeField]
    public static float mirrorSpeed = 0.0f;

    protected Interactable lastInteraction = null;
    public static bool AllowGameInput = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            rb = GetComponent<Rigidbody2D>();
            playerAnimation = FindObjectOfType<PlayerAnimation>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void FixedUpdate()
    {
        Vector2 currentPos = rb.position;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);
        Vector2 movement = inputVector * moveSpeed;
        Vector2 newPos = currentPos + movement * Time.fixedDeltaTime;
        playerAnimation.SetDirection(movement);
        rb.MovePosition(newPos);
    }

    public static void SetSpeed(float targetSpeed)
    {
        moveSpeed = targetSpeed;
    }

    public void SetAsActiveInteractable(Interactable target)
    {
        Debug.Log("Changing from " + lastInteraction.ToString() + " to " + target.ToString());
        lastInteraction = target;
    }

    public void ClearActiveInteractable()
    {
        lastInteraction = null;
    }

    public void Update()
    {
        if (QuitGameInput())
        {
            Application.Quit();
        }
    }

    public bool QuitGameInput()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
