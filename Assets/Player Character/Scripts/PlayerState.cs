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
    public abstract PlayerState OnInput(InputAction.CallbackContext ctx);
    public abstract PlayerState DoState(InputAction.CallbackContext ctx);
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
    public MoveState(PlayerController pController) : base(pController){}
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name.Equals("Move"))
        {
            return this;
        }
        return new IdleState(pController);
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name.Equals("Move"))
        {
            Vector2 axis = ctx.ReadValue<Vector2>();
            Debug.Log($"Player move input: {axis}");
        }
        return this;
    }
    public override PlayerState OnUpdate()
    {
        return this;
    }
}
