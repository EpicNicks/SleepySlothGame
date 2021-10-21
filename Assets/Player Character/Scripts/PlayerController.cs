using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Ragdoll))]
public class PlayerController : MonoBehaviour
{
    private PlayerState state;
    private bool isRunning = false;

    #region Inspector Fields
    [SerializeField]
    private float moveSpeed = 1.0f;
    [SerializeField]
    private float runModifier = 1.5f;
    public float MoveSpeed { get => moveSpeed * (isRunning ? runModifier : 1.0f); }
    [SerializeField]
    private float ragdollSeconds = 1.0f;
    public float RagdollSeconds => ragdollSeconds;
    [SerializeField]
    private Ragdoll ragdoll;
    public Ragdoll Ragdoll => ragdoll;
    [SerializeField]
    private AudioClip snoreSFX;
    [SerializeField]
    private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;
    [SerializeField]
    private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    #endregion


    private void Awake()
    {
        #region Component null checking
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        if (ragdoll == null)
        {
            ragdoll = GetComponent<Ragdoll>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        #endregion
        if (audioSource != null)
        {
            audioSource.clip = snoreSFX;
        }

        state = new IdleState(this);
    }
    private void Update()
    {
        state = state?.OnUpdate();
    }

    public void OnInput(InputAction.CallbackContext ctx)
    {
        state = state?.OnInput(ctx)?.DoState(ctx);
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        Debug.Log(ctx.action.phase);
        if (ctx.action.phase.Equals(InputActionPhase.Started))
        {
            isRunning = true;
        }
        else if (ctx.action.phase.Equals(InputActionPhase.Canceled))
        {
            isRunning = false;
        }
        Debug.Log("isRunning " + isRunning);
    }
}
