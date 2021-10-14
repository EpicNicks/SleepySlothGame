using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerState state;

    #region Inspector Fields
    [SerializeField]
    private float moveSpeed = 1.0f;
    public float MoveSpeed { get => moveSpeed; }
    [SerializeField]
    private CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }
    #endregion


    private void Awake()
    {
        state = new IdleState(this);

        #region Component null checking
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        #endregion
    }
    private void Update()
    {
        state = state?.OnUpdate();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        state = state?.OnInput(ctx)?.DoState(ctx);
    }
}
