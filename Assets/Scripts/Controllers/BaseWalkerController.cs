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

	private State StateInAir;

	private State StateJump;

	protected override void Initialize()
	{
		base.Initialize();

		#region State Grounded

		StateGrounded = new State (
			"Grounded",
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
			null
		);

		#endregion
		 
		#region State In Air

		StateInAir = new State (
			"In Air",
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
			delegate () { }
		);

		#endregion

		#region State Jump

		StateJump = new State (
			"Jump",
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
			delegate () { }
		);

		#endregion

		StateGrounded.SetTransitions(
			new Transition[]
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

		StateInAir.SetTransitions(
			new Transition[]
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

		StateJump.SetTransitions(
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

		startState = StateGrounded;
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
