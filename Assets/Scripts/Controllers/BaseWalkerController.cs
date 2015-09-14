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
	private State StateGrounded = new State (
		delegate ()
		{
			if ( animationChecksDict[PossibleAnimations.Grounded].hasParam )
				Components.animator.SetBool(animationChecksDict[PossibleAnimations.Grounded].paramName, true);

			//Reset the jumps to zero, because this means grounded.
			currentJump = 0;
		},
		null,
		delegate()
		{
			Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
			float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
			float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

			Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

			Move(newPosition, input);
			Rotate();
		},
		null,
		new Transition[2]
		{
			new Transition (
				delegate () { return OnGround() && pJump.canJump && GBInput.GetButtonDown("Jump"); },
				StateJump
			),
			new Transition (
				delegate () { return !OnGround(); },
				StateInAir
			)
		}
	);

	private State StateInAir = new State (
		//Enter
		delegate ()
		{
			//Falling or being in air for some other reason, im
			// implementing a rule that you have one less jump than 
			// you normally would.
			if (pJump.canJump && currentJump == 0 )
					currentJump++;
		},
		//Update
		null,
		//FixedUpdate
		delegate()
		{
			Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
			float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
			float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

			Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

			Move(newPosition, input);
			Rotate();
		},
		//LateUpdate
		null,
		//Exit
		delegate () { },
		//Transitions
		new Transition[2]
		{
			new Transition (
				delegate () { return OnGround(); },
				StateGrounded
			),
			new Transition (
				delegate () { return pJump.canJump && GBInput.GetButtonDown("Jump"); },
				StateJump
			)
		}
	);

	private State StateJump = new State (
		//Enter
		delegate ()
		{
			if ( currentJump < pJump.maxConsecutiveJumps )
			{
				Jump();
				currentJump++;
			}
		},
		//Update
		null,
		//FixedUpdate
		null,
		//LateUpdate
		null,
		//Exit
		delegate () { },
		//Transitions
		new Transition[]
		{
			new Transition (
				delegate () { return OnGround(); },
				StateGrounded
			),
			new Transition (
				delegate () { return !OnGround(); },
				StateInAir
			)
		}
	);

	#endregion

	#region Mono Methods

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		currentState = StateGrounded;

		base.Start();
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
