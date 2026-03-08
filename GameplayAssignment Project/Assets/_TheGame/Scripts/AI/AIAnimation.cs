using Custom;
using UnityEngine;
using System;
using UnityEngine.Animations.Rigging;

public class AIAnimation : CharacterAnimation
{// THIS CLASS CONTROLS THE ANIMATION OF THE ENEMIES
    [HideInInspector] public AIController ctrl;


    [Header("Animation Values")]
    public bool attacking = false;
    public bool finishedAttack = true;
    public int attackTypesAmount = 3;
    int lastAttack = 1;
    public float maxTimeOnAttack = 5f;
    float maxTimeOnAttackTimer = 0;
    bool startedAttack = false;


    [Header("Refferences")]
    public MultiAimConstraint headConstraint;

    public event Action OnFinishedAttack;
    public event Action OnStartDamage;
    public event Action OnEndDamage;

    public override void StartingRefferences()
    {
        animator = GetComponent<Animator>();
        base.StartingRefferences();
    }

    public override void Start()
    {
        base.Start();

        // START WITH CORRECT VALUES

        rightHandOccupied = false;
        leftHandOccupied = false;

        isAlive = true;
        isIdle = true;
        isWalking = false;
        isRunning = false;
        isCrouched = false;
        isGrounded = true;
        isLanding = true;

        TargetForwardMovement = 0;
        TargetSidewaysMovement = 0;

        finishedAttack = true;
    }

    public override void Update()
    {
        base.Update();

        // THIS CONTROLS WETHER OR NOT THE ENEMY SHOULD ATTACK

        if (!startedAttack && attacking)
        {
            startedAttack = true;
            animator.SetInteger("AttackType", lastAttack);
            maxTimeOnAttackTimer = maxTimeOnAttack;
            finishedAttack = false;
        }
        else if (attacking)
        {
            animator.SetBool("StartedAttack", true);
        }

        if (attacking)
        {
            if (maxTimeOnAttackTimer > 0) maxTimeOnAttackTimer -= Time.deltaTime;
            else FinishedAttack();
        }
        else
        {
            startedAttack = false;
            animator.SetInteger("AttackType", 0);
        }
    }

    int RandomNoRepeat(int minInclusive, int maxExclusive, int old)
    { // JUST A FUNCTION THAT RETURNS A RANDOM NUMBER THAT ISNT THE SAME AS THE ONE BEFORE
        int res = UnityEngine.Random.Range(minInclusive, maxExclusive);
        return res == old ? RandomNoRepeat(minInclusive, maxExclusive, old) : res;
    }

    public void FinishedAttack() // GETS CALLED BY ANIMATION EVENTS AT THE END OF THE ATTACK ANIMATION
    {
        startedAttack = false;
        maxTimeOnAttackTimer = maxTimeOnAttack;
        animator.SetBool("StartedAttack", false);
        OnFinishedAttack?.Invoke();
        lastAttack = RandomNoRepeat(1, attackTypesAmount + 1, lastAttack);
        finishedAttack = true;
    }

    public void StartDamage()
    { // GETS CALLED BY ANIMATION EVENTS AT THE POINT IN THE ATTACKING ANIMATION AT WHICH THE SWORD SHOULD START DEALING DAMAGE
        OnStartDamage?.Invoke();
    }

    public void EndDamage()
    { // GETS CALLED BY ANIMATION EVENTS AT THE POINT IN THE ATTACKING ANIMATION AT WHICH THE SWORD SHOULD STOP DEALING DAMAGE
        OnEndDamage?.Invoke();
    }
}
