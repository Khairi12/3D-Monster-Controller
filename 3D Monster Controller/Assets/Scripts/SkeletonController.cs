using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Animator anim;
    private Vector3 movement;
    private int attackStateHash;
    private float horizontalMovement;
    private float verticalMovement;

    private void Start ()
    {
        anim = GetComponent<Animator>();
        movement = Vector3.zero;

        attackStateHash = Animator.StringToHash("Attack");
        horizontalMovement = 0;
        verticalMovement = 0;
    }
	
    private void Attack() 
    {
        // play attack animation
        if (Input.GetKeyDown(KeyCode.Space))
            anim.Play(attackStateHash);
    }

    private void Move() 
    {
        // prevent movement if attacking
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            return;

        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        
        if (Input.GetButton("Horizontal") && !Input.GetButton("Vertical") ||
            Input.GetButton("Vertical") && !Input.GetButton("Horizontal")) 
        {
            // move horizontally / vertically
            movement = new Vector3(horizontalMovement, 0, verticalMovement);
            transform.Translate(movement * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        else if (Input.GetButton("Horizontal") && Input.GetButton("Vertical")) 
        {
            // move diagonally
            float diagonalXSpeed = horizontalMovement * 0.725f;
            float diagonalZSpeed = verticalMovement * 0.725f;

            movement = new Vector3(diagonalXSpeed, 0, diagonalZSpeed);
            transform.Translate(movement * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        else
        {
            // retain facing direction when not moving
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        }
        
        // update state machine vars
        anim.SetFloat("HorizontalSpeed", horizontalMovement);
        anim.SetFloat("VerticalSpeed", verticalMovement);
    }

	private void Update ()
    {
        Attack();
        Move();
	}
}
