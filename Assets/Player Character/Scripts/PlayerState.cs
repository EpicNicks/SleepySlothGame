using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
    protected PlayerController pController;
    public PlayerState(PlayerController pController)
    {
        this.pController = pController;
    }
    /// <summary>
    /// Do on input while in the state
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns>The next state</returns>
    public abstract PlayerState OnInput(InputAction.CallbackContext ctx);
    /// <summary>
    /// Do after processing input input while in the state
    /// </summary>
    /// <param name="ctx">The input action callback context</param>
    /// <returns>The next state</returns>
    public abstract PlayerState DoState(InputAction.CallbackContext ctx);
    /// <summary>
    /// Do every frame while in this state
    /// </summary>
    /// <returns>The next state</returns>
    public abstract PlayerState OnUpdate();
}

public class IdleState : PlayerState
{
    private float idleTime;

    public IdleState(PlayerController pController) : base(pController){}
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name.Equals("Move"))
        {
            return new MoveState(pController);
        }
        return this;
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        return this;
    }

    public override PlayerState OnUpdate()
    {
        idleTime += Time.deltaTime;
        return this;
    }
}

public class MoveState : PlayerState
{
    private Vector3 move;

    public MoveState(PlayerController pController) : base(pController){}
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.action.name);
        if (ctx.action.name.Equals("Move"))
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            this.move = new Vector3(move.x, 0, move.y);
            return this;
        }
        return new IdleState(pController);
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        return this;
    }
    public override PlayerState OnUpdate()
    {
        Debug.Log($"Player move input: {move}");
        if (move == Vector3.zero)
        {
            return new IdleState(pController);
        }
        pController.CharacterController.Move(move * pController.MoveSpeed * Time.deltaTime);
        return this;
    }
}
