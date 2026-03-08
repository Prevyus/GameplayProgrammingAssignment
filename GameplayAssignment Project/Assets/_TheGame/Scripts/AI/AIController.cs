using Custom;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{ // THIS CLASS CONTROLS THE ENEMIES
    [Header("Settings")]
    public float attackDistance = 4f;
    public float followDistance = 25f;

    public bool seenPlayer = false;

    [Header("Refferences")]
    public Transform camAnchor;
    public Transform cam;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public AIAnimation anim;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public ObjectHealthComponent healthComponent;
    [HideInInspector] public CharacterModel model;
    public Collider sword;

    [HideInInspector] public bool swordDealsDamage = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<ObjectHealthComponent>();
        model = GetComponent<CharacterModel>();
        anim = GetComponentInChildren<AIAnimation>();
        anim.ctrl = this;
        anim.OnFinishedAttack += OnEndAttack;
        anim.OnStartDamage += StartDamage;
        anim.OnEndDamage += EndDamage;
    }

    private void OnEnable() // ADDS THIS ENEMY TO THE GLOBAL LIST OF ENEMIES
    {
        GameRoot.Instance.aiManager.enemies.Add(this);
    }

    private void Start()
    {
        player = GameRoot.Instance.playerController;
    }

    private void Update()
    {
        Controller();
    }

    private void Controller()
    {
        if (!seenPlayer) // IF THE ENEMY HASNT SEEN THE PLAYER, IT CHECKS ITS FOV TO SEE IF IT APPEARS, ONCE THE ENEMY HAS SEEN THE PLAYER, IT STOPS CHECKING
        {
            Vector3 directionToTarget = (player.transform.position - camAnchor.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            if (angle <= 75) seenPlayer = true;
        }

        float dist = (transform.position - player.transform.position).magnitude;

        // IF BLOCK THAT SHOULD EXECUTE WHEN THE ENEMY IS SUPPOSED TO BE IDLE
        if (dist > followDistance || !seenPlayer || !player.isAlive) // IDLE
        {
            agent.SetDestination(transform.position);

            TurnHeadAndBody(true);

            anim.isIdle = true;
            anim.isWalking = false;

            anim.TargetForwardMovement = 0;
            anim.TargetSidewaysMovement = 0;

            anim.headConstraint.weight = Mathf.Lerp(anim.headConstraint.weight, 1, Time.deltaTime * 10);
        }
        
        // IF BLOCK THAT SHOULD EXECUTE WHEN THE ENEMY IS SUPPOSED TO FOLLOW
        if (dist < followDistance && dist > attackDistance && seenPlayer && player.isAlive) // FOLLOWING
        {
            if (anim.finishedAttack) agent.SetDestination(player.transform.position);
            else agent.SetDestination(transform.position);

            TurnHeadAndBody(true);

            anim.isIdle = false;
            anim.isWalking = true;

            anim.headConstraint.weight = Mathf.Lerp(anim.headConstraint.weight, 1, Time.deltaTime * 10);

            float ver = 0;
            float hor = 0;
            Vector3 toTarget = player.transform.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                ver = 0f;
                hor = 0f;
                return;
            }
            Vector3 localDir = transform.InverseTransformDirection(toTarget.normalized);
            ver = Mathf.Clamp(localDir.z, -1f, 1f);
            hor = Mathf.Clamp(-localDir.x, -1f, 1f);
            anim.TargetForwardMovement = ver;
            anim.TargetSidewaysMovement = hor;
        }

        // IF BLOCK THAT SHOULD EXECUTE WHEN THE ENEMY IS SUPPOSED TO BE ATTACKING
        if (dist < attackDistance && seenPlayer && player.isAlive) // ATTACKING
        {
            agent.SetDestination(transform.position);

            anim.isIdle = true;
            anim.isWalking = false;
            anim.TargetForwardMovement = 0;
            anim.TargetSidewaysMovement = 0;

            anim.headConstraint.weight = Mathf.Lerp(anim.headConstraint.weight, 0, Time.deltaTime * 100);

            anim.attacking = true;

            TurnHeadAndBody(false);
        }
    }

    void OnEndAttack()
    { // WHEN THIS IS EXECUTED, IT MEANS THE ENEMY SHOULD STOP ATTACKING AS SOON AS IT FINISHES PLAYING ITS CURRENT ATTACK
        anim.attacking = false;
    }

    void StartDamage()
    {// GETS CALLED BY ANIMATION EVENTS AT THE POINT IN THE ATTACKING ANIMATION AT WHICH THE SWORD SHOULD START DEALING DAMAGE
        if (!sword.enabled) 
        { 
            sword.enabled = true; 
            swordDealsDamage = true;
        }
    }

    void EndDamage()
    {// GETS CALLED BY ANIMATION EVENTS AT THE POINT IN THE ATTACKING ANIMATION AT WHICH THE SWORD SHOULD STOP DEALING DAMAGE
        if (sword.enabled) 
        { 
            sword.enabled = false; 
            swordDealsDamage = false; 
        }
    }

    public void TurnHeadAndBody(bool turnHead)
    { // THIS TURNS THE HEAD OF THE ENEMY TOWARDS THE PLAYER AND IF THE HEAD TURNS TOO MUCH, IT STARTS TURNING THE BODY
        Vector3 directionToTarget = (player.transform.position - camAnchor.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (angle <= 75 && turnHead)
        {
            seenPlayer = true;
            Quaternion targetRot = Quaternion.LookRotation(player.EyePosition.position - camAnchor.position);

            camAnchor.rotation = Quaternion.Slerp(camAnchor.rotation, targetRot, Time.deltaTime * 7);
        }
        else if (seenPlayer)
        {
            Quaternion targetRot = Quaternion.LookRotation(player.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 7);
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }
        else
        {
            camAnchor.localRotation = Quaternion.Slerp(camAnchor.localRotation, Quaternion.identity, Time.deltaTime * 3);
        }
    }
    
}
