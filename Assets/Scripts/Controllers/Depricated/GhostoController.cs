using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This controls the GBActor Ghost, who is our main character.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RotationVectors))]
[RequireComponent(typeof(Rigidbody))]
public class GhostoController : BaseController
{
	#region Editor Interface

	/// <summary>
	/// Properties dealing with animations.
	/// </summary>
	[Serializable]
	private class AnimationProperties
	{
		[Tooltip("The name of the animator state when this character takes damage.")]
		public string gotHurtAnimatorStateName;
		[Tooltip("The name of the animator state when this character attacks with a spirit ball.")]
		public string spiritBallAttackAnimatorStateName;
	}

	/// <summary>
	/// Properties dealing with animations.
	/// </summary>
	[Serializable]
	private class MovementProperties
	{
		[Tooltip( "The speed the player moves across the screen." )]
		public float horizontalSpeed;

	
		[Tooltip( "The max height the player jumps to" )]
		public float jumpHeight;

		[Tooltip("The main collider used for things like jumping and stuff.")]
		public CapsuleCollider mainCollider;

		[Tooltip( "The distance of the spherecast past the main collider to check if the player is grounded.")]
		public float groundedCheckDistanceModifier;
		[Tooltip( "The amount to subtract from the width of the main collider to check if the player is grounded.")]
		public float groundedCheckWidthModifier;
		[Tooltip( "The distance of the spherecast past the main collider to see if the player is up against a wall." )]
		public float wallCheckDistanceModifier;
		[Tooltip( "The amount to subtract from the height of the characters main collider to do wall checks.")]
		public float wallCheckHeightModifier;

		[Tooltip( "The instance of RotationVectors for this character.")]
		public RotationVectors rotationVectors;

		[Tooltip( "The Rigidbody attached to this character.")]
		public Rigidbody rigidBody;
	}

	/// <summary>
	/// Properties dealing with animations.
	/// </summary>
	[Serializable]
	private class AttackProperties
	{
		[Tooltip( "Spirit ball attack, attached to this Ghosto  GBActor" )]
		public AttackGhostoSpiritBall spiritBallAttack;

		[Tooltip( "The max distance Ghosto can teleport.")]
		public float teleportMaxDistance;
	}

	/// <summary>
	/// Properties dealing with animations.
	/// </summary>
	[Serializable]
	private class AudioProperties
	{
		[Tooltip("The first jump audio clip")]
		public AudioClip firstJumpClip;
		[Tooltip("The second jump audio clip")]
		public AudioClip secondJumpClip;
		[Tooltip("The audio source the play this character's sound effects from.")]
		public AudioSource source;
	}

	[SerializeField] private AnimationProperties animationProperties;
	[SerializeField] private MovementProperties  movementProperties;
	[SerializeField] private AttackProperties    attackProperties;
	[SerializeField] private AudioProperties     audioProperties;

	[Header( "Possession" )]
	[Tooltip( "Renderer to be toggled when the player possesses and depossesses something." )]
	public Renderer pRenderer;
	
	//public bool isGrounded;
	//private bool inSecondJump;


	[Header( "Taking Damage" )]
	[Tooltip( "The velocity of the player when taking damage." )]
	public float damagingCollisionVelocity;

	[Header("Colliders")]
	[Tooltip( "The layers to ignore when casting for this character." )]
	public LayerMask raycastHitMask;

	#endregion

	#region Private Variables

	private float groundedCheckDistance;
	private float groundedCheckRadius;
	private float wallCheckDistance;
	private float wallCheckRadius;

	private Vector3 moveDirection;

    private bool canSecondJump;
	 
	/// <summary>
	/// The state this character is currently in
	/// </summary>
	private State currentState;

	#endregion

	#region State 0: Default
	/// <summary>
	/// Grounded, Not Attacking, Not getting hurt, default state
	/// </summary>
	private State State0;

    private void State0Enter()
    {
        Debug.Log( "Ghosto Entering State 0: " + State0.stateDescription );
        anim.SetBool( "IsGrounded", true );
    }
    private void State0Update()
    {
        Movement();
        Rotation();

        if ( !IsGrounded() )
            FiniteStateMachine.Transition( ref currentState, State2 );
        if ( GBInput.GetButtonDown( "Jump" ) )
            FiniteStateMachine.Transition( ref currentState, State1 );
        else if ( GBInput.GetButtonDown( "Attack" ) )
            FiniteStateMachine.Transition( ref currentState, State4 );
        else if ( GBInput.GetButtonDown( "Possess" ) )
            FiniteStateMachine.Transition( ref currentState, State5 );      
    }
    private void State0Exit()
    {
        Debug.Log( "Ghosto Exiting State 0: " + State0.stateDescription );
    }
    #endregion

    #region State 1: Jumping
    /// <summary>
    /// Jump
    /// </summary>
    private State State1;

    private void State1Enter()
    {
        Debug.Log( "Entering State 1: " + State1.stateDescription );
        if ( IsGrounded() )
        {
            Jump(audioProperties.firstJumpClip);
            canSecondJump = true;
        }
        else if ( canSecondJump )
        {
            Jump(audioProperties.secondJumpClip);
            canSecondJump = false;
        }
    }

    private void State1Update()
    {
        Movement();
        Rotation();

        if ( IsGrounded() )
            FiniteStateMachine.Transition( ref currentState, State0 );
        else
            FiniteStateMachine.Transition( ref currentState, State2 );
    }
    private void State1Exit()
    {
        Debug.Log( "Exiting State 1: " + State1.stateDescription );
    }
    #endregion

    #region State 2: In Air

    /// <summary>
    /// Double jumped, in air
    /// </summary>
    private State State2;

    private void State2Enter()
    {
        Debug.Log( "Entering State 2: " + State1.stateDescription );
        anim.SetBool( "IsGrounded", false );
    }
    private void State2Update()
    {
        Movement();
        Rotation();

        if ( IsGrounded() )
            FiniteStateMachine.Transition( ref currentState, State0 );
        else if ( GBInput.GetButtonDown( "Jump" ) )
            FiniteStateMachine.Transition( ref currentState, State1 );
        else if ( GBInput.GetButtonDown( "Attack" ) )
            FiniteStateMachine.Transition( ref currentState, State4 );
        else if ( GBInput.GetButtonDown( "Possess" ) )
            FiniteStateMachine.Transition( ref currentState, State5 );
    }
    private void State2Exit()
    {
        Debug.Log( "Exiting State 2: " + State1.stateDescription );
    }
    #endregion

    #region State 3 Got hurt, Waiting to Recover

    /// <summary>
    /// Got hurt, waiting to recover
    /// </summary>
    private State State3;

    private void State3Enter()
    {
        Debug.Log( "Entering State 3: " + State3.stateDescription );
        anim.SetTrigger( "Hurt" );
    }

    private void State3Update()
    {
        if ( !anim.GetCurrentAnimatorStateInfo(0).IsName(animationProperties.gotHurtAnimatorStateName) )
        {
            if ( IsGrounded() )
                FiniteStateMachine.Transition( ref currentState, State0 );
            else 
                FiniteStateMachine.Transition( ref currentState, State2 );
        }
    }

    private void State3Exit()
    {

    }
	#endregion

	#region State 4: Attacking

	/// <summary>
	/// Attacking, waiting to finish
	/// </summary>
	private State State4;
	private enum Attacks
	{
		None,
		SpiritBall,
		Teleport
	}
	private Attacks currentAttack;

	private void State4Enter()
	{
		Debug.Log("Entering State 4: " + State4.stateDescription);
		if ( GBInput.GetAxis("Vertical") < -.5 )
		{
			movementProperties.rigidBody.MovePosition(movementProperties.rigidBody.position + new Vector3(FindTeleportLocation(), 0f, 0f));
			currentAttack = Attacks.Teleport;
		}
		else
		{
			anim.SetTrigger("Spirit Ball Attack");
			currentAttack = Attacks.SpiritBall;
		}
	}

	private void State4Update()
    {
        if (currentAttack == Attacks.Teleport)
        {
            if ( IsGrounded() )
                FiniteStateMachine.Transition( ref currentState, State0 );
            else 
                FiniteStateMachine.Transition( ref currentState, State2 );
        }
        else if ( currentAttack == Attacks.SpiritBall && !anim.GetCurrentAnimatorStateInfo(0).IsName(animationProperties.spiritBallAttackAnimatorStateName) )
        {
            if ( IsGrounded() )
                FiniteStateMachine.Transition( ref currentState, State0 );
            else 
                FiniteStateMachine.Transition( ref currentState, State2 );
        }
		else
		{
			if ( IsGrounded() )
                FiniteStateMachine.Transition( ref currentState, State0 );
            else 
                FiniteStateMachine.Transition( ref currentState, State2 );
		}
    }

    private void State4Exit()
    {
        currentAttack = Attacks.None;
    }
    #endregion

    #region State 5: Possessing

    /// <summary>
    /// When holding down the possess button
    /// </summary>
    private State State5;

    private void State5Enter()
    {
        Debug.Log( "Entering State 5: " + State5.stateDescription );
        //Set possessing boolean
    }

    private void State5Update()
    {
        if (GBInput.GetButtonUp("Possess"))
        {
            if ( IsGrounded() )
                FiniteStateMachine.Transition( ref currentState, State0 );
            else 
                FiniteStateMachine.Transition( ref currentState, State2 );
        }
    }

    private void State5Exit()
    {
        //Unset possessing boolean
    }
    #endregion

	#region Mono Methods

	void Start()
	{
		//Initialize the states
		State0 = new State("Grounded, Not Attacking, Not getting hurt, default state", State0Enter, State0Update, State0Exit);
		State1 = new State("Jump", State1Enter, State1Update, State1Exit);
		State2 = new State("In air", State2Enter, State2Update, State2Exit);
		State3 = new State("Getting hurt", State3Enter, State3Update, State3Exit);
		State4 = new State("Attacking, waiting to finish", State4Enter, State4Update, State4Exit);
		State5 = new State("Holding down possess button", State5Enter, State5Update, State5Exit);

		//Initialize the check variables
		groundedCheckDistance = movementProperties.mainCollider.bounds.extents.y + movementProperties.groundedCheckDistanceModifier;
		groundedCheckRadius   = movementProperties.mainCollider.bounds.extents.x - movementProperties.groundedCheckWidthModifier;
		wallCheckDistance     = movementProperties.mainCollider.bounds.extents.y + movementProperties.wallCheckDistanceModifier;
		wallCheckRadius       = movementProperties.mainCollider.bounds.extents.x - movementProperties.wallCheckHeightModifier;

		moveDirection = Vector3.right;

		//Start in state 0
		currentState = State0;
		currentState.OnEnter();
	}

	void OnEnable()
	{
		if ( State0 != null )
		{
			currentState = State0;
			currentState.OnEnter();
		}
	}

	void FixedUpdate()
	{
		if ( currentState != null )
		{
			if ( currentState.OnUpdate != null )
			{
				currentState.OnUpdate();
			}
			else
				Debug.LogWarning( "Current state On Update is null" );
		}
		else
			Debug.Log( "Current state is null" );
	}

	//On collision, if /collision.transform/ has actor stats, meaning it is an enemy, it will get blown back. 
	void OnCollisionEnter( Collision collision )
	{
		if ( enabled )
		{
			GBActorStats stats = collision.collider.GetComponent<GBActorStats>();

			if ( stats != null && currentState != State3 )
			{
				float damageDirection = transform.position.x - collision.transform.position.x;

				float otherBlowbackForce = stats.BlowbackForce;
				if ( damageDirection < 0 )
				{
					movementProperties.rigidBody.AddForce( -otherBlowbackForce, otherBlowbackForce, 0f, ForceMode.VelocityChange );
				}
				else if ( damageDirection >= 0 )
				{
					movementProperties.rigidBody.AddForce( otherBlowbackForce, otherBlowbackForce, 0f, ForceMode.VelocityChange );
				}
				RecieveDamage( 10 );

				FiniteStateMachine.Transition( ref currentState, State3 );
			}
		}
	}
    #endregion

    #region Functions
    void Movement()
	{
		float horizontal = GBInput.GetAxis("Horizontal") * Time.deltaTime * movementProperties.horizontalSpeed;

		if ( horizontal < 0 )
			moveDirection = Vector3.left;
		else if ( horizontal > 0 )
			moveDirection = Vector3.right;

		if ( !WallCheck() )
		{
			movementProperties.rigidBody.MovePosition( transform.position + new Vector3( horizontal, 0f, 0f ) );
		}

		anim.SetFloat( "Horizontal", Mathf.Abs( horizontal ) );
		//anim.SetFloat("Horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
	}

	private void Rotation()
	{
		Vector3 rotation = movementProperties.rigidBody.rotation.eulerAngles;

		Vector3 targetRotation = ((moveDirection.x > 0 ? movementProperties.rotationVectors.positiveXRotation : movementProperties.rotationVectors.negativeXRoation) * Mathf.Abs( moveDirection.x )) +
								 ((moveDirection.y > 0 ? movementProperties.rotationVectors.positiveYRotation : movementProperties.rotationVectors.negativeYRotaion) * Mathf.Abs( moveDirection.y )) +
								 ((moveDirection.z > 0 ? movementProperties.rotationVectors.positiveZRotation : movementProperties.rotationVectors.negativeZRotaion) * Mathf.Abs( moveDirection.z ));

		Quaternion deltaRotation = Quaternion.Euler( new Vector3( Mathf.MoveTowardsAngle( rotation.x, targetRotation.x, movementProperties.rotationVectors.rotationVelocity ), 
									    Mathf.MoveTowardsAngle( rotation.y, targetRotation.y, movementProperties.rotationVectors.rotationVelocity ), 
									   Mathf.MoveTowardsAngle( rotation.z, targetRotation.z, movementProperties.rotationVectors.rotationVelocity ) ) );

		movementProperties.rigidBody.MoveRotation( deltaRotation );
	}

	private void Jump(AudioClip clip)
	{
		if ( clip != null )
		{
			//v = sprt((final velocity)^2 + 2 (acceleration) (distance))
			movementProperties.rigidBody.SetVelocityY(Mathf.Sqrt(-2 * Physics.gravity.y * movementProperties.jumpHeight));
			audioProperties.source.clip = clip;
			audioProperties.source.Play();
		}
	}

	/// <summary>
	/// Returns true if there is some obstruction in front of ghos
	/// </summary>
	/// <param name="hit"></param>
	/// <returns></returns>
	private bool WallCheck()
	{
		RaycastHit hit;

		Debug.DrawRay(movementProperties.mainCollider.bounds.center, moveDirection * wallCheckDistance, Color.red);
		if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center, moveDirection, out hit, wallCheckDistance, raycastHitMask) )
		{
			float verticalDistance = (movementProperties.mainCollider.bounds.extents.y / 2) - movementProperties.wallCheckHeightModifier;
			Debug.DrawRay(movementProperties.mainCollider.bounds.center - new Vector3(0f, verticalDistance, 0f), moveDirection * wallCheckDistance, Color.red);
			if (!Physics.Raycast(movementProperties.mainCollider.bounds.center - new Vector3(0f, verticalDistance, 0f), moveDirection, out hit, wallCheckDistance, raycastHitMask))
			{
				Debug.DrawRay(movementProperties.mainCollider.bounds.center + new Vector3(0f, verticalDistance, 0f), moveDirection * wallCheckDistance, Color.red);
				if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center + new Vector3(0f, verticalDistance, 0f), moveDirection, out hit, wallCheckDistance, raycastHitMask) )
				{
					return false;
				}
			}
		}

		if ( hit.transform.HasTag( TagSystem.Environment ) )
		{
			return true;
		}

		return false;
	}

	private bool IsGrounded()
	{
		RaycastHit hit;

		Debug.DrawRay(movementProperties.mainCollider.bounds.center, Vector3.down * wallCheckDistance, Color.green);
		if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center, Vector3.down, out hit, wallCheckDistance, raycastHitMask) )
		{
			float horizontalDistance = (movementProperties.mainCollider.bounds.extents.x / 2) - movementProperties.groundedCheckDistanceModifier;
			Debug.DrawRay(movementProperties.mainCollider.bounds.center - new Vector3(horizontalDistance, 0f, 0f), Vector3.down * wallCheckDistance, Color.green);
			if (!Physics.Raycast(movementProperties.mainCollider.bounds.center - new Vector3(horizontalDistance, 0f, 0f), Vector3.down, out hit, wallCheckDistance, raycastHitMask))
			{
				Debug.DrawRay(movementProperties.mainCollider.bounds.center + new Vector3(horizontalDistance, 0f, 0f), Vector3.down * wallCheckDistance, Color.green);
				if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center + new Vector3(horizontalDistance, 0f, 0f), Vector3.down, out hit, wallCheckDistance, raycastHitMask) )
				{
					return false;
				}
			}
		}

		if ( hit.transform.HasTag(TagSystem.Jumpable) )
		{
			return true;
		}
		
		return false;
	}

	private float FindTeleportLocation()
	{
		RaycastHit hit;

		float distance = attackProperties.teleportMaxDistance;

		if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center, moveDirection, out hit, distance, raycastHitMask) )
		{
			if (!Physics.Raycast(movementProperties.mainCollider.bounds.center - new Vector3(0f, movementProperties.wallCheckHeightModifier, 0f), moveDirection, out hit, distance, raycastHitMask))
			{
				if ( !Physics.Raycast(movementProperties.mainCollider.bounds.center + new Vector3(0f, movementProperties.wallCheckHeightModifier, 0f), moveDirection, out hit, distance, raycastHitMask) )
				{
					return attackProperties.teleportMaxDistance - movementProperties.mainCollider.radius;
				}
			}
		}

		return hit.distance - movementProperties.mainCollider.radius;
	}

	#endregion

	#region Possession
	public override void PossessionSpecifics()
	{
		pRenderer.enabled = true;
		movementProperties.rigidBody.useGravity = true;
		GetComponent<CapsuleCollider>().enabled = true;

		this.enabled = true;
	}

	public override void ExitPossessionSpecifics()
	{
		pRenderer.enabled = false;
		movementProperties.rigidBody.useGravity = false;
		GetComponent<CapsuleCollider>().enabled = false;

		this.enabled = false;
	}
	#endregion
}
