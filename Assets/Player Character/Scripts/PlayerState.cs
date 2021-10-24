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

    public IdleState(PlayerController pController) : base(pController)
    {
        pController.Ragdoll.Animator.SetBool("isRunning", false);
    }
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name.Equals("Move"))
        {
            return new MoveState(pController);
        }
        else if (ctx.action.name.Equals("Snore"))
        {
            return new SnoreState(pController);
        }
        return this;
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        return this;
    }

    public override PlayerState OnUpdate()
    {
        if (!pController.IsGrounded)
        {
            return new MoveState(pController);
        }
        idleTime += Time.deltaTime;
        return this;
    }
}

public class MoveState : PlayerState
{
    private Vector3 move;
    private float turnSmoothVelocity;

    public MoveState(PlayerController pController) : base(pController)
    {
        pController.Ragdoll.Animator.SetBool("isRunning", true);
    }
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name.Equals("Move"))
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            this.move.x = move.x;
            this.move.z = move.y;
            return this;
        }
        else if (ctx.action.name.Equals("Snore"))
        {
            return new SnoreState(pController);
        }
        return new IdleState(pController);
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        return this;
    }

    public override PlayerState OnUpdate()
    {
        if (move == Vector3.zero && pController.IsGrounded)
        {
            pController.CharacterController.Move(new Vector3(0, -2f, 0));
            return new IdleState(pController);
        }
        var targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        var angle = Mathf.SmoothDampAngle(pController.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, pController.TurnSmoothTime);
        pController.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        pController.CharacterController.Move(moveDir * pController.MoveSpeed * Time.deltaTime);
        pController.CharacterController.Move(new Vector3(0, pController.Gravity) * Time.deltaTime);

        //pController.transform.rotation = Quaternion.LookRotation(move);
        //pController.CharacterController.Move(pController.transform.forward * pController.MoveSpeed * Time.deltaTime);
        return this;
    }
}

public class SnoreState : PlayerState
{
    private bool startedRagdoll = false;
    private bool endRagdoll = false;
    private float ragdollEndingSeconds = 0.0f;

    public SnoreState(PlayerController pController) : base(pController)
    {
        pController.Ragdoll.Animator.SetBool("isRunning", false);
    }
    public override PlayerState OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.action.phase.Equals(InputActionPhase.Started) && ctx.action.name.Equals("Snore"))
        {
            endRagdoll = true;
        }
        return this;
    }
    public override PlayerState DoState(InputAction.CallbackContext ctx)
    {
        if (!startedRagdoll && pController.RagdollSeconds > 0)
        {
            pController.Ragdoll.ActivateRagdoll();
            if (pController.AudioSource)
            {
                pController.AudioSource.loop = true;
                pController.AudioSource.Play();
            }
            startedRagdoll = true;
        }
        return this;
    }

    public override PlayerState OnUpdate()
    {
        if (endRagdoll)
        {
            if (pController.AudioSource)
            {
                pController?.AudioSource.Stop();
            }
            pController.Ragdoll.UnRagdoll();
            if (ragdollEndingSeconds >= Mathf.Min(pController.Ragdoll.RecordingTime, pController.Ragdoll.CurRecordingTime))
            {
                return new IdleState(pController);
            }
            else
            {
                ragdollEndingSeconds += Time.deltaTime;
            }
        }
        return this;
    }
}

