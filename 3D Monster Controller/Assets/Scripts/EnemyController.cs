using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum MonsterMoveState { IDLE, CHASING, SEEKING }

public class EnemyController : MonoBehaviour {

    [HideInInspector] public bool movementEnabled = true;

    public float defaultSpeed = 3f;         // default speed
    public float chaseSpeed = 6f;           // chase speed

    public float idleDuration = 1;
    public float stopDistance = 1;

    public float fovRange = 50f;            // max visible range
    public float fovAngle = 160f;           // max visible FOV

    private MonsterMoveState moveState;     // current state of movement
    private NavMeshAgent navagent;          // monster navigation agent
    private Transform playerTransform;      // player transform for player position
    private Vector3 destination;            // current destination

    private float outgoingStopDist;         // stop distance after calculations
    private float outGoingIdleTime;         // amount of time left to idle
    private float outgoingRange;            // visible range after calculations

    public void SetDestination(Vector3 newDestination) {
        destination = newDestination;
        navagent.SetDestination(destination);
    }

    private void Awake() {
        navagent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Start() {
        moveState = MonsterMoveState.IDLE;
        navagent.speed = defaultSpeed;
        outgoingStopDist = stopDistance;
        outGoingIdleTime = idleDuration;
        outgoingRange = fovRange;
    }

    private bool IsPlayerVisible(Vector3 transformPos, Vector3 playerPos, float range) {
        RaycastHit hit;
        float curAngle = Vector3.Angle(playerTransform.position - transform.position, transform.forward);

        if (Vector3.Distance(transformPos, playerPos) < range) {
            if (Physics.Raycast(transformPos, playerPos - transformPos, out hit)) {
                if (hit.collider.tag == "Player") {
                    if (curAngle < fovAngle * 0.5f) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void Idle() {
        outGoingIdleTime -= Time.deltaTime;

        if (outGoingIdleTime <= 0f) {
            navagent.isStopped = false;
            moveState = MonsterMoveState.SEEKING;
            outGoingIdleTime = idleDuration;
        }
        else if (IsPlayerVisible(transform.position, playerTransform.position, outgoingRange)) {
            navagent.isStopped = false;
            navagent.speed = chaseSpeed;
            moveState = MonsterMoveState.CHASING;
            outGoingIdleTime = idleDuration;
        }
    }

    private void Seek() {
        if (IsPlayerVisible(transform.position, playerTransform.position, outgoingRange)) {
            navagent.speed = chaseSpeed;
            moveState = MonsterMoveState.CHASING;
        }
        else if (Vector3.Distance(transform.position, destination) < outgoingStopDist) {
            navagent.isStopped = true;
            moveState = MonsterMoveState.IDLE;
        }
    }

    private void Chase() {
        if (IsPlayerVisible(transform.position, playerTransform.position, outgoingRange)) {
            SetDestination(playerTransform.position);
        }
        else if (Vector3.Distance(transform.position, destination) > outgoingRange) {
            navagent.speed = defaultSpeed;
            navagent.isStopped = true;
            moveState = MonsterMoveState.IDLE;
        }
    }

    private void Update() {
        if (!movementEnabled)
            return;

        if (moveState == MonsterMoveState.SEEKING)
            Seek();
        else if (moveState == MonsterMoveState.CHASING)
            Chase();
        else if (moveState == MonsterMoveState.IDLE)
            Idle();
    }
}
