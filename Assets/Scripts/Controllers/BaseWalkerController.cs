using UnityEngine;
using System.Collections;

public class BaseWalkerController : CharacterController
{

	#region Constructor
	public BaseWalkerController()
	{
		movementType = MovementTypes.Walker;
	}

	#endregion

	#region States

	/// <summary>
	/// Handles ground movement for a walker
	/// </summary>
	private State StateGrounded;
	private void StateGroundedEnter()
	{
		if ( animationChecksDict[PossibleAnimations.Grounded].hasParam )
			Components.animator.SetBool(animationChecksDict[PossibleAnimations.Grounded].paramName, true);

		//Reset the jumps to zero, because this means grounded.
		currentJump = 0;
	}
	private void StateGroundedUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Move(newPosition, input);
		Rotate();

		if ( OnGround() )
		{
			if ( pJump.canJump && GBInput.GetButtonDown("Jump") )
				FiniteStateMachine.Transition(ref currentState, StateJump);
		}
		else
			FiniteStateMachine.Transition(ref currentState, StateInAir);
	}
	private void StateGroundedExit()
	{

	}

	/// <summary>
	/// This is what happens when a walker is in the air
	/// </summary>
	protected State StateInAir;
	protected void StateInAirEnter()
	{
		//Falling or being in air for some other reason, im
		// implementing a rule that you have one less jump than 
		// you normally would.
		if (pJump.canJump && currentJump == 0 )
				currentJump++;
	}
	protected void StateInAirUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Move(newPosition, input);
		Rotate();

		if ( OnGround() )
			FiniteStateMachine.Transition(ref currentState, StateGrounded);
		if ( pJump.canJump && GBInput.GetButtonDown("Jump") )
			FiniteStateMachine.Transition(ref currentState, StateJump);
	}
	protected void StateInAirExit()
	{

	}

	/// <summary>
	/// causes a player to "jump", shooting them vertically in the air
	/// </summary>
	protected State StateJump;
	protected void StateJumpEnter()
	{
		if ( currentJump < pJump.maxConsecutiveJumps )
		{
			Jump();
			currentJump++;
		}
	}
	protected void StateJumpUpdate()
	{
		if ( OnGround() )
			FiniteStateMachine.Transition(ref currentState, StateGrounded);
		else
			FiniteStateMachine.Transition(ref currentState, StateInAir);
	}
	protected void StateJumpExit()
	{

	}

	#endregion

	#region Mono Methods

	protected override void Awake()
	{
		base.Awake();

		StateGrounded = new State( "Grounded", StateGroundedEnter, StateGroundedUpdate, StateGroundedExit );
		StateInAir = new State( "In air", StateInAirEnter, StateInAirUpdate, StateInAirExit );
		StateJump = new State("Jumping", StateJumpEnter, StateJumpUpdate, StateJumpExit);

		currentState = StateGrounded;
		currentState.OnEnter();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

	}

	#endregion

	#region Utility Functions
	protected override void Rotate()
	{
		
		Vector3 rotation = components.rigidBody.rotation.eulerAngles;

		Vector3 targetRotation = Input.GetAxis( "Horizontal" ) > 0 ? components.rotationVectors.positiveXRotation : Input.GetAxis("Horizontal") < 0 ? components.rotationVectors.negativeXRoation : Vector3.zero;

		Quaternion deltaRotation = Quaternion.Euler( new Vector3( Mathf.MoveTowardsAngle( rotation.x, targetRotation.x, components.rotationVectors.rotationVelocity ), 
									    Mathf.MoveTowardsAngle( rotation.y, targetRotation.y, components.rotationVectors.rotationVelocity ), 
									   Mathf.MoveTowardsAngle( rotation.z, targetRotation.z, components.rotationVectors.rotationVelocity ) ) );

		components.rigidBody.MoveRotation( deltaRotation );
	}

	#endregion
}
