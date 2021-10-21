using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private struct JointPos
    {
        public CharacterJoint joint;
        public Vector3 pos;
        public Quaternion rot;
    }

    private bool isRecording = false;
    private bool isRewinding = false;
    private float curRecordingTime = 0.0f;
    private List<List<JointPos>> recordedJointPositions = new List<List<JointPos>> { };

    [SerializeField]
    [Min(0.0f)]
    private float recordingTime = 1.0f;

    [SerializeField]
    private Animator animator;
    public Animator Animator => animator;
    [SerializeField]
    private List<Rigidbody> ragdollBodies;
    [SerializeField]
    private List<Collider> ragdollColliders;

    void Awake()
    {
        DisableRagdoll();
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

    private void DisableRagdoll()
    {
        ragdollBodies = ragdollBodies.Select(body => { body.isKinematic = true; return body; }).ToList();
        ragdollColliders = ragdollColliders.Select(col => { col.enabled = true; return col; }).ToList();
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    private void Record()
    {
        isRecording = true;
        isRewinding = false;
        curRecordingTime = 0.0f;
    }
}
