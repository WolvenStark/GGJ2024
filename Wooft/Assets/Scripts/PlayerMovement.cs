using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    protected PlayerAnimation playerAnimation;
    protected float moveH, moveV;

    [SerializeField]
    protected float moveSpeed = 1.0f;

    protected Interactable lastInteraction = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimation = FindObjectOfType<PlayerAnimation>();
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
