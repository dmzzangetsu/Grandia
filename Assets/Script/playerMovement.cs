using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class playerMovement : MonoBehaviour
{
    [SerializeField] private float movespeed = 5f;
    [SerializeField] private InputActionReference moveActionsReference;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
  

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * movespeed * Time.fixedDeltaTime);
    }
    void OnDisable()
    {
        moveActionsReference.action.performed -= Move;
        moveActionsReference.action.canceled -= StopMove;
        moveActionsReference.action.Disable();
    }

    void OnEnable()
    {
        moveActionsReference.action.Enable();
        moveActionsReference.action.performed += Move;
        moveActionsReference.action.canceled += StopMove;
    }
    void Move(InputAction.CallbackContext context){
        moveInput = context.ReadValue<Vector2>();
    }

    void StopMove(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
}
