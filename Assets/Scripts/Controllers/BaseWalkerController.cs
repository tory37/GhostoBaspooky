using UnityEngine;
using System.Collections;

public class BaseWalkerController : CharacterController {

	public BaseWalkerController()
	{
		movementType = MovementTypes.Walker;
	}

	/// <summary>
	/// Handles ground movement for a walker
	/// </summary>
	private State StateWalkerGrounded;
	private void StateWalkerGroundedEnter()
	{
		//if ( animationChecksDict[PossibleAnimations.Grounded].hasParam )
		//Components.animator.SetBool(animationChecksDict[PossibleAnimations.Grounded].paramName, true);

		//Reset the jumps to zero, because this means grounded.
		currentJump = 0;
	}
	private void StateWalkerGroundedUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Movement(newPosition, input);

		if ( OnGround() )
		{
			if ( pJump.canJump && GBInput.GetButtonDown("Jump") )
				FiniteStateMachine.Transition(ref currentState, StateJump);
		}
		else
			FiniteStateMachine.Transition(ref currentState, StateWalkerInAir);
	}
	private void StateWalkerGroundedExit()
	{

	}

	/// <summary>
	/// This is what happens when a walker is in the air
	/// </summary>
	protected State StateWalkerInAir;
	protected void StateWalkerInAirEnter()
	{
		//Falling or being in air for some other reason, im
		// implementing a rule that you have one less jump than 
		// you normally would.
		if (pJump.canJump && currentJump == 0 )
				currentJump++;
	}
	protected void StateWalkerInAirUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Movement(newPosition, input);

		if ( OnGround() )
			FiniteStateMachine.Transition(ref currentState, StateWalkerGrounded);
		if ( pJump.canJump && GBInput.GetButtonDown("Jump") )
			FiniteStateMachine.Transition(ref currentState, StateJump);
	}
	protected void StateWalkerInAirExit()
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
			FiniteStateMachine.Transition(ref currentState, StateWalkerGrounded);
		else
			FiniteStateMachine.Transition(ref currentState, StateWalkerInAir);
	}
	protected void StateJumpExit()
	{

	}

	protected override void Awake()
	{
		base.Awake();

	}

	protected override void OnEnable()
	{
		base.OnEnable();

	}
}
