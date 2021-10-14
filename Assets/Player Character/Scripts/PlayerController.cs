using System.Collections.Generic;
using System.Linq; 

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private struct JointPos
    {
        public CharacterJoint joint;
        public Vector3 pos;
        public Quaternion rot;
    }

    private PlayerState state;
    private bool isRecording = false;
    private bool isRewinding = false;
    private float curRecordingTime = 0.0f;
    private List<List<JointPos>> recordedJointPositions = new List<List<JointPos>> { };

    #region Inspector Fields
    [SerializeField]
    [Min(0.0f)]
    private float recordingTime = 1.0f;
    [SerializeField]
    private float moveSpeed = 1.0f;
    public float MoveSpeed { get => moveSpeed; }
    [SerializeField]
    private CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private List<Rigidbody> ragdollBodies;
    [SerializeField]
    private List<Collider> ragdollColliders;
    #endregion


    private void Awake()
    {
        state = new IdleState(this);
        DisableRagdoll();

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

    /// <summary>
    /// Doesn't run when timescale is 0, perfect for physics and pausing
    /// </summary>
    private void FixedUpdate()
    {
        if (isRecording && curRecordingTime < recordingTime)
        {
            //add current position and rotation of all character joints to list
            //I use GetComponentsInChildren each time because I want to ensure the references added are up to date and the same; can be optimized later
            recordedJointPositions.Add(
                GetComponentsInChildren<CharacterJoint>()
                .Select(joint => new JointPos { joint = joint, pos = joint.transform.position, rot = joint.transform.rotation })
                .ToList()
            );
            curRecordingTime += Time.fixedDeltaTime;
        }
        if (isRewinding)
        {
            //done rewinding
            if (recordedJointPositions.Count == 0)
            {
                isRewinding = false;
                DisableRagdoll();
            }
            //still rewinding
            else
            {
                List<JointPos> jointPosFrame = recordedJointPositions.Last();
                jointPosFrame.Select(jointPos => { jointPos.joint.transform.position = jointPos.pos; jointPos.joint.transform.rotation = jointPos.rot; return jointPos; });
                recordedJointPositions.RemoveAt(recordedJointPositions.Count - 1);
            }
        }
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        state = state?.OnInput(ctx)?.DoState(ctx);
    }

    public void DisableRagdoll()
    {
        ragdollBodies = ragdollBodies.Select(body => { body.isKinematic = true; return body; }).ToList();
        ragdollColliders = ragdollColliders.Select(col => { col.enabled = true; return col; }).ToList();
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    public void ActivateRagdoll()
    {
        Record();
        ragdollBodies = ragdollBodies.Select(body => { body.isKinematic = false; return body; }).ToList();
        ragdollColliders = ragdollColliders.Select(col => { col.enabled = false; return col; }).ToList();
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void UnRagdoll()
    {
        isRewinding = true;
        isRecording = false;
    }

    private void Record()
    {
        isRecording = true;
        isRewinding = false;
        curRecordingTime = 0.0f;
    }
}
