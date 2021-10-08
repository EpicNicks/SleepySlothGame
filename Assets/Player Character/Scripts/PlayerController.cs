using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;

    private PlayerState state;

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

    public void Move(InputAction.CallbackContext ctx)
    {
        state = state.OnInput(ctx).DoState(ctx);
    }

    private void Update()
    {
        state = state?.OnUpdate();
    }
}
