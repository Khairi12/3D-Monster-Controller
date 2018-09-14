using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    public float walkSpeed = 1;
    public float runSpeed = 2;
    public float idleTime = 60;

    private Animator anim;
    private Vector3 movement;
    private int attackStateHash;
    private int skillStateHash;
    private float horizontalMovement;
    private float verticalMovement;

    private void Start ()
    {
        anim = GetComponent<Animator>();
        movement = Vector3.zero;

        attackStateHash = Animator.StringToHash("Attack");
        skillStateHash = Animator.StringToHash("Skill");
        horizontalMovement = 0;
        verticalMovement = 0;
    }
	
    private void Attack() 
    {
        if (Input.GetMouseButtonDown(0))
            anim.Play(attackStateHash);
    }

    private void UseSkill() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
            anim.Play(skillStateHash);
    }

    private void Run() 
    {
        if (Input.GetKey(KeyCode.LeftShift))
            anim.SetBool("Running", true);
        else
            anim.SetBool("Running", false);
    }

    private void Idle()
    {
        idleTime += Time.deltaTime;

        if (idleTime >= 5)
        {
            idleTime = 0;

            anim.SetInteger("IdleCounter", anim.GetInteger("IdleCounter") + 1);

            if (anim.GetInteger("IdleCounter") > 5)
                anim.SetInteger("IdleCounter", 0);
        }
    }

    private void Move() 
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        anim.SetFloat("HorizontalSpeed", horizontalMovement);
        anim.SetFloat("VerticalSpeed", verticalMovement);

        // stop movement
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Skill")) 
        {
            return;
        }

        if (Input.GetButton("Horizontal") && !Input.GetButton("Vertical") ||
            Input.GetButton("Vertical") && !Input.GetButton("Horizontal")) 
        {
            // move horizontally / vertically
            float speed = anim.GetBool("Running") ? runSpeed : walkSpeed;
            movement = new Vector3(horizontalMovement, 0, verticalMovement);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        else if (Input.GetButton("Horizontal") && Input.GetButton("Vertical")) 
        {
            // move diagonally
            float speed = anim.GetBool("Running") ? runSpeed : walkSpeed;
            float diagonalXSpeed = horizontalMovement * 0.725f;
            float diagonalZSpeed = verticalMovement * 0.725f;

            movement = new Vector3(diagonalXSpeed, 0, diagonalZSpeed);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        else
        {
            // retain facing direction when not moving
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
    }

	private void Update ()
    {
        Attack();
        UseSkill();
        Move();
        Run();
        Idle();
	}
}
