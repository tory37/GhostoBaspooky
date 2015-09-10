using UnityEngine;
using System;

public class SquidController : BaseController {


	#region Editor Interface

	[Serializable]
	public class ComponentProperties
	{
		public Rigidbody rigidBody;
		public RotationVectors rotationVectors;
		public Collider mainCollider;
		public AudioSource audioSource;
	}

	[Serializable]
	public class MovementProperties
	{
		public float hoverMaxHeight;
		public float moveSpeed;
		public float jumpHeight;

		[Tooltip( "The amount to subtract from the width of the main collider to check if the player is grounded.")]
		public float groundedCheckWidthModifier;
		[Tooltip( "The distance of the spherecast past the main collider to see if the player is up against a wall." )]
		public float wallCheckDistanceModifier;
		[Tooltip( "The amount to subtract from the height of the characters main collider to do wall checks.")]
		public float wallCheckHeightModifier;

		public LayerMask raycastHitMask;
	}

	[Serializable]
	public class AnimationProperties
	{
		public Animator animator;
		public string moveAnimationStateName;
	}

	[Serializable]
	public class AudioProperties
	{
		public AudioClip jumpClip;
	}

	public ComponentProperties pComponents;
	public MovementProperties  pMovement;
	public AnimationProperties pAnimation;
	public AudioProperties     pAudio;

	#endregion Editor Interface

	#region Private Fields

	private float groundedCheckRadius;
	private float wallCheckDistance;
	private float wallCheckRadius;

	private Vector3 moveDirection;

	private State currentState;

	#endregion

	#region State Default

	private State StateDefault;

	private void StateDefaultEnter()
	{
		pComponents.rigidBody.useGravity = false;
	}
	private void StateDefaultUpdate()
	{
		HorizontalMovement();
		VerticalMovement();
		Rotation();

		if ( !IsGrounded() )
			FiniteStateMachine.Transition(ref currentState, StateInAir);
		else if ( GBInput.GetButtonDown("Jump") )
			FiniteStateMachine.Transition(ref currentState, StateJumping);

	}
	private void StateDefaultExit()
	{

	}

	#endregion State Default

	#region State In Air

	private State StateInAir;

	private void StateInAirEnter()
	{
		pComponents.rigidBody.useGravity = true;
	}
	private void StateInAirUpdate()
	{
		HorizontalMovement();
		Rotation();

		if ( IsGrounded() )
			FiniteStateMachine.Transition(ref currentState, StateDefault);
	}
	private void StateInAirExit()
	{

	}

	#endregion

	#region State Jumping

	private State StateJumping;

	private void StateJumpingEnter()
	{
		
	}
	private void StateJumpingUpdate()
	{
		HorizontalMovement();
        Rotation();

        if ( IsGrounded() )
            FiniteStateMachine.Transition( ref currentState, StateDefault );
        else
            FiniteStateMachine.Transition( ref currentState, StateInAir );
	}
	private void StateJumpingExit()
	{

	}

	#endregion

	#region Mono Methods

	void Start()
	{
		//Initialize the states
		StateDefault = new State("Grounded, Not Attacking, Not getting hurt, default state", StateDefaultEnter, StateDefaultUpdate, StateDefaultExit);
		StateInAir   = new State("Not grounded, not attacking, not getting hurt", StateInAirEnter, StateInAirUpdate, StateInAirExit);
		StateJumping = new State("Pressed jump button.", StateJumpingEnter, StateJumpingUpdate, StateJumpingExit);

		//Initialize the check variables
		groundedCheckRadius   = pComponents.mainCollider.bounds.extents.x - pMovement.groundedCheckWidthModifier;
		wallCheckDistance     = pComponents.mainCollider.bounds.extents.y + pMovement.wallCheckDistanceModifier;
		wallCheckRadius       = pComponents.mainCollider.bounds.extents.x - pMovement.wallCheckHeightModifier;

		moveDirection = Vector3.right;

		//Start in state 0
		currentState = StateDefault;
		currentState.OnEnter();
	}

	void OnEnable()
	{
		if ( StateDefault != null )
		{
			currentState = StateDefault;
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

	#endregion

	#region Private Methods

	private void HorizontalMovement()
	{
		float horizontalInput = GBInput.GetAxis("Horizontal") * pMovement.moveSpeed * Time.deltaTime;

		if ( horizontalInput < 0 )
			moveDirection = Vector3.left;
		else if ( horizontalInput > 0 )
			moveDirection = Vector3.right;

		if ( !WallCheck() )
		{
			pComponents.rigidBody.MovePosition( transform.position + new Vector3( horizontalInput, 0f, 0f ) );
		}

		pAnimation.animator.SetFloat( "Horizontal", Mathf.Abs( horizontalInput ) );
	}

	private void VerticalMovement()
	{
		float verticalInput = GBInput.GetAxis("Vertical") * pMovement.moveSpeed * Time.deltaTime;

		pComponents.rigidBody.MovePosition( transform.position + new Vector3( 0f, verticalInput, 0f ) );
	}

	/// <summary>
	/// Returns true if there is some obstruction in front of ghos
	/// </summary>
	/// <param name="hit"></param>
	/// <returns></returns>
	private bool WallCheck()
	{
		RaycastHit hit;

		Debug.DrawRay(pComponents.mainCollider.bounds.center, moveDirection * wallCheckDistance, Color.red);
		if ( !Physics.Raycast(pComponents.mainCollider.bounds.center, moveDirection, out hit, wallCheckDistance,  pMovement.raycastHitMask) )
		{
			float verticalDistance = (pComponents.mainCollider.bounds.extents.y / 2) - pMovement.wallCheckHeightModifier;
			Debug.DrawRay(pComponents.mainCollider.bounds.center - new Vector3(0f, verticalDistance, 0f), moveDirection * wallCheckDistance, Color.red);
			if (!Physics.Raycast(pComponents.mainCollider.bounds.center - new Vector3(0f, verticalDistance, 0f), moveDirection, out hit, wallCheckDistance, pMovement.raycastHitMask))
			{
				Debug.DrawRay(pComponents.mainCollider.bounds.center + new Vector3(0f, verticalDistance, 0f), moveDirection * wallCheckDistance, Color.red);
				if ( !Physics.Raycast(pComponents.mainCollider.bounds.center + new Vector3(0f, verticalDistance, 0f), moveDirection, out hit, wallCheckDistance, pMovement.raycastHitMask) )
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

		Debug.DrawRay(pComponents.mainCollider.bounds.center, Vector3.down * pMovement.hoverMaxHeight, Color.green);
		if ( !Physics.Raycast(pComponents.mainCollider.bounds.center, Vector3.down, out hit, pMovement.hoverMaxHeight, pMovement.raycastHitMask) )
		{
			float horizontalDistance = (pComponents.mainCollider.bounds.extents.x / 2) - groundedCheckRadius;
			Debug.DrawRay(pComponents.mainCollider.bounds.center - new Vector3(horizontalDistance, 0f, 0f), Vector3.down * pMovement.hoverMaxHeight, Color.green);
			if (!Physics.Raycast(pComponents.mainCollider.bounds.center - new Vector3(horizontalDistance, 0f, 0f), Vector3.down, out hit, pMovement.hoverMaxHeight, pMovement.raycastHitMask))
			{
				Debug.DrawRay(pComponents.mainCollider.bounds.center + new Vector3(horizontalDistance, 0f, 0f), Vector3.down * pMovement.hoverMaxHeight, Color.green);
				if ( !Physics.Raycast(pComponents.mainCollider.bounds.center + new Vector3(horizontalDistance, 0f, 0f), Vector3.down, out hit, pMovement.hoverMaxHeight, pMovement.raycastHitMask) )
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

	private void Rotation()
	{
		Vector3 rotation = pComponents.rigidBody.rotation.eulerAngles;

		Vector3 targetRotation = ((moveDirection.x > 0 ? pComponents.rotationVectors.positiveXRotation : pComponents.rotationVectors.negativeXRoation) * Mathf.Abs( moveDirection.x )) +
								 ((moveDirection.y > 0 ? pComponents.rotationVectors.positiveYRotation : pComponents.rotationVectors.negativeYRotaion) * Mathf.Abs( moveDirection.y )) +
								 ((moveDirection.z > 0 ? pComponents.rotationVectors.positiveZRotation : pComponents.rotationVectors.negativeZRotaion) * Mathf.Abs( moveDirection.z ));

		Quaternion deltaRotation = Quaternion.Euler( new Vector3( Mathf.MoveTowardsAngle( rotation.x, targetRotation.x, pComponents.rotationVectors.rotationVelocity ), 
									    Mathf.MoveTowardsAngle( rotation.y, targetRotation.y, pComponents.rotationVectors.rotationVelocity ), 
									   Mathf.MoveTowardsAngle( rotation.z, targetRotation.z, pComponents.rotationVectors.rotationVelocity ) ) );

		pComponents.rigidBody.MoveRotation( deltaRotation );
	}

	private void Jump( AudioClip clip )
	{
		//v = sprt((final velocity)^2 + 2 (acceleration) (distance))
		pComponents.rigidBody.SetVelocityY( Mathf.Sqrt( -2 * Physics.gravity.y * pMovement.jumpHeight ) );
		if ( clip != null )
		{
			pComponents.audioSource.clip = clip;
			pComponents.audioSource.Play();
		}
	}

	#endregion
}