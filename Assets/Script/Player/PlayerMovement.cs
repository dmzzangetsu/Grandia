using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movespeed = 5f;
    [SerializeField] private InputActionReference moveActionsReference;
    [SerializeField] private InteractingArea interactingArea;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private Rigidbody2D rb;
 
    private Vector2 moveInput;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        TopDownManager.Instance.RegisterPlayerMovement(this);
    }
    

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * movespeed * Time.fixedDeltaTime);
        interactingArea.UpdateInteractionAreaPosition(moveInput);
    }
    void OnDisable()
    {
        moveActionsReference.action.performed -= Move;
        moveActionsReference.action.canceled -= StopMove;
        moveActionsReference.action.Disable();
        animator.SetBool("IsMove", false);
    }

    void OnEnable()
    {
        moveActionsReference.action.Enable();
        moveActionsReference.action.performed += Move;
        moveActionsReference.action.canceled += StopMove;
    }
    void Move(InputAction.CallbackContext context){
        animator.SetBool("IsMove", true);
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX",moveInput.x);
        animator.SetFloat("InputY",moveInput.y);
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void StopMove(InputAction.CallbackContext context)
    {
        animator.SetBool("IsMove", false);
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        animator.SetFloat("LastInputX",moveInput.x);
        animator.SetFloat("LastInputY",moveInput.y);
        moveInput = Vector2.zero;
    }

    public void SetMovement(bool newValue)
    {
        if (newValue)
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }
}
