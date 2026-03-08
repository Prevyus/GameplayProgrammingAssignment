using UnityEngine;
using UnityEngine.Animations.Rigging;
using Custom;

namespace Custom
{
    public class CharacterAnimation : MonoBehaviour
    {// BASE CLASS THAT CONTROLS THE ANIMATION FOR CHARACTERS

        [HideInInspector] public Animator animator;

        [Header("Settings")]
        public float transitionSpeed = 1;
        public bool smoothWeightChanges = true;
        public float armWeightSmoothSpeed = 5;

        [Header("Refferences")]
        public TwoBoneIKConstraint rightHandIK;
        public TwoBoneIKConstraint leftHandIK;

        [Header("Animation Values")]
        public bool isAlive;
        public bool isIdle;
        public bool isWalking;
        public bool isRunning;
        public bool isCrouched;
        public bool isGrounded;
        public bool isLanding;
        public bool rightHandOccupied = false;
        public bool leftHandOccupied = false;
        float targetRightArmWeight = 0;
        float targetLeftArmWeight = 0;

        float ForwardMovement;
        float SidewaysMovement;
        public float TargetForwardMovement;
        public float TargetSidewaysMovement;

        float isPickingUpTriggerTime = 0;
        float isStartingJumpTriggerTime = 0;
        float isStartingDealtDamageTriggerTime = 0;

        int Base;
        int Legs;
        int Arms;
        int Head;

        bool started = false;

        public void Death()
        {
            isAlive = false;
            animator.SetBool("isAlive", false);
            animator.SetBool("PlayedDeathAnim", true);

            animator.SetFloat("ForwardMovement", 0);
            animator.SetFloat("SidewaysMovement", 0);
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouched", false);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isLanding", false);
            animator.SetBool("isPickingUp", false);
            animator.SetBool("isStartingJump", false);
            animator.SetBool("DealtDamage", false);
            rightHandIK.weight = 0;
            leftHandIK.weight = 0;
        }
        public virtual void StartingRefferences()
        {
            bool success = animator;
            if (animator) started = true;
            else return;

            Base = animator.GetLayerIndex("Base Layer");
            Legs = animator.GetLayerIndex("Legs");
            Arms = animator.GetLayerIndex("Arms");
            Head = animator.GetLayerIndex("Head");
        }

        public virtual void Start()
        {
            StartingRefferences();
        }

        public virtual void Update()
        {
            if (!started) StartingRefferences();

            if (!isAlive) return;

            // SETS ALL THE ANIMATOR PARAMETERS BASED ON THE PUBLIC VARIABLES IN THIS CLASS

            ForwardMovement = Mathf.Lerp(ForwardMovement, TargetForwardMovement, Time.deltaTime * transitionSpeed);
            SidewaysMovement = Mathf.Lerp(SidewaysMovement, TargetSidewaysMovement, Time.deltaTime * transitionSpeed);

            isRunning = isRunning && TargetSidewaysMovement == 0 && TargetForwardMovement == 1 && !isCrouched;

            animator.SetFloat("ForwardMovement", ForwardMovement);
            animator.SetFloat("SidewaysMovement", SidewaysMovement);

            animator.SetBool("isIdle", isIdle);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isCrouched", isCrouched);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isLanding", isLanding);

            if (isPickingUpTriggerTime > 0) isPickingUpTriggerTime -= Time.deltaTime;
            else animator.SetBool("isPickingUp", false);

            if (isStartingJumpTriggerTime > 0) isStartingJumpTriggerTime -= Time.deltaTime;
            else animator.SetBool("isStartingJump", false);

            if (isStartingDealtDamageTriggerTime > 0) isStartingDealtDamageTriggerTime -= Time.deltaTime;
            else animator.SetBool("DealtDamage", false);

            if (smoothWeightChanges)
            {
                targetRightArmWeight = rightHandOccupied ? 1 : 0;
                targetLeftArmWeight = leftHandOccupied ? 1 : 0;

                rightHandIK.weight = Mathf.Clamp(rightHandIK.weight + (armWeightSmoothSpeed * Time.deltaTime * (targetRightArmWeight == 0 ? -1 : 1)), 0f, 1);
                leftHandIK.weight = Mathf.Clamp(leftHandIK.weight + (armWeightSmoothSpeed * Time.deltaTime * (targetLeftArmWeight == 0 ? -1 : 1)), 0f, 1);
            }
            else
            {
                rightHandIK.weight = rightHandOccupied ? 1 : 0;
                leftHandIK.weight = leftHandOccupied ? 1 : 0;
            }
        }

        public virtual void OnPickup(GroundItem item) // DISABLED
        {
            //animator.SetBool("isPickingUp", true);
            //isPickingUpTriggerTime = 0.05f;
        }
        public virtual void OnJump()
        {
            animator.SetBool("isStartingJump", true);
            isStartingJumpTriggerTime = 0.05f;
        }
        public virtual void OnDealtDamage()
        {
            animator.SetBool("DealtDamage", true);
            isStartingDealtDamageTriggerTime = 0.05f;
        }

        public virtual void OnLegsAndArmsWeightToZero()
        {
            animator.SetLayerWeight(Legs, 0);
            animator.SetLayerWeight(Arms, 0);
        }

        public virtual void OnPlayedDeathAnim()
        {
            animator.SetBool("PlayedDeathAnim", true);
        }
    }
}

