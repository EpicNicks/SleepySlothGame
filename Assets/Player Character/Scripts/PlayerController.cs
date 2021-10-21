using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Ragdoll))]
public class PlayerController : MonoBehaviour
{
    private PlayerState state;

    #region Inspector Fields
    [SerializeField]
    private float moveSpeed = 1.0f;
    public float MoveSpeed { get => moveSpeed; }
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
}
