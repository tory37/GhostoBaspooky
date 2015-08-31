using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerTreeEnt : BaseController
{


	[Header( "Movement Variables" )]
	[Tooltip( "The speed the player moves across the screen." )]
	public float horizontalMovementSpeed;

	[Header( "Rotation Variables" )]
	[Tooltip( "Testing: Whether or not the player rotates to the front when idle." )]
	public bool rotateIdleToFront;
	[Tooltip( "The rotation of the player when moving to the right." )]
	public Vector3 goingRightRotation;
	[Tooltip( "The rotation of the player when moving to the left." )]
	public Vector3 goingLeftRotation;
	[Tooltip( "The rotation of the player when idle." )]
	public Vector3 idleRotation;
	[Tooltip( "Speed at which the character rotates towards the direction it needs to face" )]
	public float rotateTowardsSpeed;

	private bool facingLeft;

	[Header( "Jump Variables" )]
	[Tooltip( "The max height the player jumps to" )]
	public float jumpHeight;
	[Tooltip( "The time to wait for this character to bend his legs before jumping" )]
	public float timeForLegBendAnimation;

	[Header( "Audio" )]
	public float sfxVolume;
	public AudioClip sfxJump;
	public AudioClip sfxSecondJump;


	[Header( "Wall Checks" )]
	[Tooltip( "The radius of the spherecast to see if the player is up against a wall." )]
	public float againstWallCheckRadius;
	[Tooltip( "The distance of the spherecast to see if the player is up against a wall." )]
	public float againstWallCheckDistance;
	[Tooltip( "Distance from players origin to where the wall check cast needs to start" )]
	public float wallCheckYOffset;

	[Header( "Testing" )]
	[Tooltip( "Testing variable to see if on the ground.  Needs to be private eventually" )]
	public bool isGrounded;
	private bool canSecondJump;

	//Future, make List<IEnumerator> InteruptedByBeingAttackedCouroutines 
	//  then interupt each one when attacked
	//private IEnumerator attackingCoroutine;
	private IEnumerator jumpingCoroutine;


	// Use this for initialization
	void Start()
	{
		facingLeft = false;

		isGrounded = true;
		canSecondJump = false;
	}

	void FixedUpdate()
	{
		bool isAttacking =  anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "Attack" );
		bool isGotHit      =  anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "Hurt" );
		//bool isJumping   =  anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");

		Vector3 direction;
		if ( facingLeft )
			direction = Vector3.left;
		else
			direction = Vector3.right;

		float horizontal = horizontalMovementSpeed * GBInput.GetAxis( "Horizontal" ) * Time.deltaTime;
		//float horizontal = horizontalMovementSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

		if ( !isAttacking && !isGotHit )
			Movement( horizontal, direction );

		Rotation( horizontal, direction );

		if ( !isAttacking && !isGotHit )
			Attacking( direction );

		if ( !isAttacking )
			Jumping();
	}

	#region Horizontal Movement
	void Movement( float horizontal, Vector3 direction )
	{
		Debug.DrawRay( transform.position + new Vector3( 0f, wallCheckYOffset, 0f ), direction * againstWallCheckDistance, Color.blue );
		RaycastHit hit;

		if ( !Physics.SphereCast( new Ray( transform.position + new Vector3( 0f, wallCheckYOffset, 0f ), direction ), againstWallCheckRadius, out hit, againstWallCheckDistance ) )
		{
			GetComponent<Rigidbody>().MovePosition( transform.position + new Vector3( horizontal, 0f, 0f ) );
		}

		anim.SetFloat( "Horizontal", Mathf.Abs( GBInput.GetAxis( "Horizontal" ) ) );
		//anim.SetFloat("Horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
	}
	#endregion

	#region Handle Facing
	void Rotation( float horizontal, Vector3 direction )
	{
		Vector3 rotation = transform.eulerAngles;
		#region Rotation To Front On Idle
		if ( rotateIdleToFront )
		{
			if ( horizontal > 0 )
			{
				transform.eulerAngles = new Vector3( rotation.x, Mathf.MoveTowardsAngle( rotation.y, goingRightRotation.y, rotateTowardsSpeed ), rotation.z );
			}
			else if ( horizontal < 0 )
			{
				transform.eulerAngles = new Vector3( rotation.x, Mathf.MoveTowardsAngle( rotation.y, goingLeftRotation.y, rotateTowardsSpeed ), rotation.z );
			}
			else
			{
				transform.eulerAngles = new Vector3( rotation.x, Mathf.MoveTowardsAngle( rotation.y, idleRotation.y, rotateTowardsSpeed ), rotation.z );
			}
		}
		#endregion
		#region Left Right Rotation
		else
		{
			if ( horizontal < 0 )
			{
				facingLeft = true;
			}
			else if ( horizontal > 0 )
			{
				facingLeft = false;
			}

			if ( facingLeft )
			{
				transform.eulerAngles = new Vector3( rotation.x, Mathf.MoveTowardsAngle( rotation.y, goingLeftRotation.y, rotateTowardsSpeed ), rotation.z );
			}
			else
			{
				transform.eulerAngles = new Vector3( rotation.x, Mathf.MoveTowardsAngle( rotation.y, goingRightRotation.y, rotateTowardsSpeed ), rotation.z );
			}
		}
		#endregion
	}
	#endregion

	#region Attacking
	void Attacking( Vector3 direction )
	{
		if ( GBInput.GetButtonDown("Attack" ))
		{
			anim.SetTrigger( "Attack" );
			//attackingCoroutine = CombatSystem.Instance.MeleeAttackCoroutine( this, GetComponent<AIControllerVariables>().meleeAttacks[0], direction );
			//StartCoroutine( attackingCoroutine );
		}
	}
	#endregion

	#region Jumping
	void Jumping()
	{
		if ( GBInput.GetButtonDown( "Jump" ) )
		{
			if ( isGrounded )
			{
				jumpingCoroutine = WaitForLegBendThenJump();
				StartCoroutine( jumpingCoroutine );
			}
			else if ( canSecondJump )
			{
				GetComponent<Rigidbody>().velocity = new Vector3( Physics.gravity.x, Mathf.Sqrt( -2 * Physics.gravity.y * jumpHeight ), Physics.gravity.z );
				anim.SetTrigger( "Second Jump" );
				AudioSource.PlayClipAtPoint( sfxSecondJump, Camera.main.transform.position, sfxVolume );
				canSecondJump = false;
			}
		}
	}

	IEnumerator WaitForLegBendThenJump()
	{
		anim.SetTrigger( "Jump" );
		yield return new WaitForSeconds( timeForLegBendAnimation );
		//v = sprt((final velocity)^2 + 2 (acceleration) (distance))
		GetComponent<Rigidbody>().velocity += new Vector3( Physics.gravity.x, Mathf.Sqrt( -2 * Physics.gravity.y * jumpHeight ), Physics.gravity.z );
		AudioSource.PlayClipAtPoint( sfxJump, Camera.main.transform.position, sfxVolume );
		canSecondJump = true;
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.transform.HasTag( TagSystem.Jumpable ) )
		{
			isGrounded = true;
			anim.SetBool( "IsGrounded", isGrounded );
			canSecondJump = false;
		}
	}

	void OnTriggerStay( Collider other )
	{
		if ( other.transform.HasTag( TagSystem.Jumpable ) )
		{
			isGrounded = true;
			anim.SetBool( "IsGrounded", isGrounded );
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.transform.HasTag( TagSystem.Jumpable ) )
		{
			isGrounded = false;
			anim.SetBool( "IsGrounded", isGrounded );
		}
	}
	#endregion

	#region Possession
	public override void PossessionSpecifics()
	{
		base.PossessionSpecifics();
	}

	public override void ExitPossessionSpecifics()
	{
		base.ExitPossessionSpecifics();
	}
	#endregion

}
