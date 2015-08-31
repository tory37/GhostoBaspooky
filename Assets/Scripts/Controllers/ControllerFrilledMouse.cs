using UnityEngine;
using System.Collections;

public class ControllerFrilledMouse : BaseController
{

	#region Public Interface

	[Header("Anim Variables")]
	public string gotHurtAnimatorStateName;
	public string spitAttackAnimatorStateName;

	[Header( "Movement Variables" )]
	[Tooltip( "The speed the player moves across the screen." )]
	public float movementSpeed;
	public Vector3 MoveDirection { get { return moveDirection; } }
	public float wallWalkRaycastLength;

	[Header( "Attacks" )]
	[Tooltip( "Spirit ball attack, attached to this Ghosto  GBActor" )]
	public AttackFrilledMouseSpit spitAttack;

	[Header( "Taking Damage" )]
	[Tooltip( "The velocity of the player when taking damage." )]
	public float damagingCollisionVelocity;

	[Header( "Colliders" )]
	[Tooltip( "The main collider used for things like wallwalking and stuff." )]
	public CapsuleCollider mainCollider;
	[Tooltip( "The layers to ignore when casting for this character." )]
	public LayerMask raycastHitMask;
	public float groundedCapsulecastDistance;
	public float groundedCapsulecastCenterToSide;
	public Vector3 capsuleCastCenter;

	#endregion

	/// <summary>
	/// The state ghosto is currently in
	/// </summary>
	private State currentState;

	#region State 0: Default
	/// <summary>
	/// Grounded, Not Attacking, Not getting hurt, default state
	/// </summary>
	private State State0;

	private void State0Enter()
	{
		Debug.Log( "Mouse Entering State 0: " + State0.stateDescription );
		anim.SetBool( "IsGrounded", true );
		if ( rb != null )
			rb.useGravity = false;
		else
			Debug.LogWarning( "Mouse does not have rigidbody.", this );
	}
	private void State0Update()
	{
		GroundedMovement();
		Rotation();

		if ( !IsGrounded() )
			FiniteStateMachine.Transition( ref currentState, State1 );
		else if ( GBInput.GetButtonDown( "Attack" ) )
			FiniteStateMachine.Transition( ref currentState, State2 );
		else if ( GBInput.GetButtonDown( "Possess" ) )
			FiniteStateMachine.Transition( ref currentState, State4 );
	}
	private void State0Exit()
	{
		Debug.Log( "Ghosto Exiting State 0: " + State0.stateDescription );
		rb.useGravity = true;
	}
	#endregion

	#region State 1: In air
	/// <summary>
	/// Jump
	/// </summary>
	private State State1;

	private void State1Enter()
	{
		Debug.Log( "Entering State 1: " + State1.stateDescription );
		anim.SetBool( "IsGrounded", false );
	}

	private void State1Update()
	{
		InAirMovement();
		Rotation();

		if ( IsGrounded() )
			FiniteStateMachine.Transition( ref currentState, State0 );
	}
	private void State1Exit()
	{
		Debug.Log( "Exiting State 1: " + State1.stateDescription );
	}
	#endregion

	#region State 2: Attack

	/// <summary>
	/// Double jumped, in air
	/// </summary>
	private State State2;
	private enum Attacks
	{
		None,
		Spit
	}
	private Attacks currentAttack;

	private void State2Enter()
	{
		Debug.Log( "Entering State 2: " + State1.stateDescription );
		anim.SetTrigger( spitAttackAnimatorStateName );
		currentAttack = Attacks.Spit;
	}
	private void State2Update()
	{
		if ( currentAttack == Attacks.Spit && !anim.GetCurrentAnimatorStateInfo( 0 ).IsName( spitAttackAnimatorStateName ) )
		{
			if ( IsGrounded() )
				FiniteStateMachine.Transition( ref currentState, State0 );
			else
				FiniteStateMachine.Transition( ref currentState, State1 );
		}
		else
		{
			if ( IsGrounded() )
				FiniteStateMachine.Transition( ref currentState, State0 );
			else
				FiniteStateMachine.Transition( ref currentState, State1 );
		}
	}
	private void State2Exit()
	{
		Debug.Log( "Exiting State 2: " + State1.stateDescription );
		currentAttack = Attacks.None;
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
		//anim.SetTrigger( "Hurt" );
	}

	private void State3Update()
	{
		//if ( !anim.GetCurrentAnimatorStateInfo(0).IsName(gotHurtAnimatorStateName) )
		//{
		if ( IsGrounded() )
			FiniteStateMachine.Transition( ref currentState, State0 );
		else
			FiniteStateMachine.Transition( ref currentState, State1 );
		//}
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

	private void State4Enter()
	{
		Debug.Log( "Entering State 4: " + State4.stateDescription );
		//Set possessing boolean
	}

	private void State4Update()
	{
		if ( GBInput.GetButtonUp( "Possess" ) )
		{
			if ( IsGrounded() )
				FiniteStateMachine.Transition( ref currentState, State0 );
			else
				FiniteStateMachine.Transition( ref currentState, State2 );
		}
	}

	private void State4Exit()
	{
		//Unset possessing boolean
	}
	#endregion

	#region Private Fields
	private RotationVectors rvs;

	private Vector3 moveDirection;

	private Rigidbody rb;

	private float levelGravity;

	private RaycastHit wallWalkHit;
	#endregion

	#region Mono Methods

	void Awake()
	{
		State0 = new State( "Grounded, Not Attacking, Not getting hurt, default state", State0Enter, State0Update, State0Exit );
		State1 = new State( "In Air", State1Enter, State1Update, State1Exit );
		State2 = new State( "Attacking", State2Enter, State2Update, State2Exit );
		State3 = new State( "Getting hurt", State3Enter, State3Update, State3Exit );
		State4 = new State( "Possessing", State4Enter, State4Update, State4Exit );

		rvs = GetComponent<RotationVectors>();

		rb = GetComponent<Rigidbody>();

		groundedCapsulecastDistance = mainCollider.bounds.extents.y + 0.1f;
		capsuleCastCenter = mainCollider.center;

		moveDirection = Vector3.right;
	}

	void OnEnable()
	{
		currentState = State0;
		currentState.OnEnter();
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
				Debug.LogWarning( "Current state On Update is null" + currentState.stateDescription );
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
					rb.AddForce( -otherBlowbackForce, otherBlowbackForce, 0f, ForceMode.VelocityChange );
				}
				else if ( damageDirection >= 0 )
				{
					rb.AddForce( otherBlowbackForce, otherBlowbackForce, 0f, ForceMode.VelocityChange );
				}
				RecieveDamage( 10 );

				FiniteStateMachine.Transition( ref currentState, State3 );
			}
		}
	}
	#endregion

	#region Functions

	void InAirMovement()
	{
		float horizontal = GBInput.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;

		rb.MovePosition( (horizontal * Vector3.right) + transform.position );

		if ( horizontal < 0 )
			moveDirection = Quaternion.LookRotation( wallWalkHit.normal ).eulerAngles - new Vector3( 90, 0, 0 );
		else if ( horizontal > 0 )
			moveDirection = Quaternion.LookRotation( wallWalkHit.normal ).eulerAngles + new Vector3( 90, 0, 0 );
	}

	void GroundedMovement()
	{
		float horizontal = GBInput.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
		float vertical = GBInput.GetAxis("Vertical") * Time.deltaTime * movementSpeed;

		Debug.DrawRay( mainCollider.bounds.center, -transform.up * wallWalkRaycastLength, Color.black );

		rb.MovePosition( (horizontal * Vector3.right) + (vertical * Vector3.up) + transform.position );

		if ( Physics.Raycast( mainCollider.bounds.center, -transform.up, out wallWalkHit, wallWalkRaycastLength, raycastHitMask ) )
		{
			rb.useGravity = false;
			rb.AddForce( -wallWalkHit.normal * levelGravity, ForceMode.Acceleration );
		}

		if ( horizontal < 0 )
			moveDirection = Quaternion.Euler(0, 0, 90) * wallWalkHit.normal;
		else if ( horizontal > 0 )
			moveDirection = Quaternion.Euler(0, 0, -90) * wallWalkHit.normal;

		//DEBUG:
		//Debug.Log( wallWalkHit.normal );
		Debug.DrawRay( mainCollider.bounds.center, (Vector3.Cross( wallWalkHit.normal, new Vector3( 0, 0, 1 ) )).normalized * wallWalkRaycastLength, Color.blue );
	}

	void Rotation()
	{
		Vector3 rotation = rb.rotation.eulerAngles;

		Vector3 targetRotation = ((moveDirection.x > 0 ? rvs.positiveXRotation : rvs.negativeXRoation) * Mathf.Abs( moveDirection.x )) +
								 ((moveDirection.y > 0 ? rvs.positiveYRotation : rvs.negativeYRotaion) * Mathf.Abs( moveDirection.y )) +
								 ((moveDirection.z > 0 ? rvs.positiveZRotation : rvs.negativeZRotaion) * Mathf.Abs( moveDirection.z ));

		Quaternion deltaRotation = Quaternion.Euler( new Vector3( Mathf.MoveTowardsAngle( rotation.x, targetRotation.x, rvs.rotationVelocity ), Mathf.MoveTowardsAngle( rotation.y, targetRotation.y, rvs.rotationVelocity ), Mathf.MoveTowardsAngle( rotation.z, targetRotation.z, rvs.rotationVelocity ) ) );

		rb.MoveRotation( deltaRotation );
	}

	bool IsGrounded()
	{
		Vector3 mod = new Vector3( groundedCapsulecastCenterToSide, 0f, 0f );
		Vector3 startPoint = mainCollider.center + transform.position;
		Debug.DrawRay( (startPoint + mod), groundedCapsulecastDistance * Vector3.down, Color.red );
		Debug.DrawRay( (startPoint - mod), groundedCapsulecastDistance * Vector3.down, Color.red );
		RaycastHit[] hits = Physics.CapsuleCastAll( startPoint- mod, startPoint+ mod, 1f, Vector3.down, groundedCapsulecastDistance, raycastHitMask );
		if ( hits.Length > 0 )
		{
			for ( int i = 0; i < hits.Length; i++ )
			{
				if ( hits[i].transform.HasTag( TagSystem.Jumpable ) )
				{
					return true;
				}
			}
		}

		return false;
	}

	#endregion

	#region Possession
	public override void PossessionSpecifics()
	{
		this.enabled = true;
	}

	public override void ExitPossessionSpecifics()
	{
		this.enabled = false;
	}
	#endregion

}
