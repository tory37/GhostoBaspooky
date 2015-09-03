using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {

	#region Enums
	
	public enum MovementTypes
	{
		/// <summary>
		/// Stays on the ground and walks/runs left and right
		/// </summary>
		Walker,
		/// <summary>
		/// Someone who has free vertical and horizontal range,
		/// though this can be limited by a 0 speed in either direction
		/// </summary>
		Flier,
		/// <summary>
		/// Someone who is limited to a distance from the ground, 
		/// but never touches it/ they float
		/// </summary>
		Hoverer,
		/// <summary>
		/// Someone who uses a jetpack, so has a hybrid of hovering and flying,
		/// and moves upward when using a boost, etc
		/// </summary>
		Jetpack
	}

	#endregion

	#region Editor Interface

	[Serializable]
	public class NeededComponents
	{
		public Rigidbody rigidBody;
		public Animator animator;
		public RotationVectors rotationVectors;
	}

	[Serializable]
	public class MovementProperties
	{
		public float horizontalSpeed;
		public float verticalSpeed;
	}

	[Serializable]
	public class JumpProperties
	{
		public float jumpHeight;
		public bool canJump;
		public int maxConsecutiveJumps;
	}

	[Serializable]
	public class AnimationProperties
	{
		public string horizontalMovement;
		public string verticalMovement;
		public string gotHurt;
		public string firstJump;
		public string secondJump;
		public List<AnimParamToBool> animationChecks = new List<AnimParamToBool>
		{
			new AnimParamToBool { paramDescription = "Horizontal Movement", paramName = "", hasParam = false },
			new AnimParamToBool { paramDescription = "Vertical Movement",   paramName = "", hasParam = false },
			new AnimParamToBool { paramDescription = "Got Hurt",            paramName = "", hasParam = false },
			new AnimParamToBool { paramDescription = "First Jump",          paramName = "", hasParam = false },
			new AnimParamToBool { paramDescription = "Second Jump",         paramName = "", hasParam = false }
		};
	}

	[SerializeField] private MovementTypes movementType;
	[SerializeField] private NeededComponents components;
	[SerializeField] private MovementProperties pMovement;
	[SerializeField] private JumpProperties pJump;
	[SerializeField] private AnimationProperties pAnimation;

	#endregion

	#region Public Interface

	public MovementTypes MovementType
	{
		get { return movementType; }
		#if UNITY_EDITOR
		set { movementType = value; }
		#endif
	}

	public NeededComponents Components
	{
		get { return components; }
		#if UNITY_EDITOR
		set { components = value; }
		#endif
	}
	public bool isComponentsExpanded;

	public MovementProperties PMovement
	{
		get { return pMovement; }
		#if UNITY_EDITOR
		set { pMovement = value; }
		#endif
	}
	public bool isPMovementExpanded;

	public JumpProperties PJump
	{
		get { return pJump; }
		#if UNITY_EDITOR
		set { pJump = value; }
		#endif
	}
	public bool isPJumpExpanded;

	public AnimationProperties PAnimation
	{
		get { return pAnimation; }
		#if UNITY_EDITOR
		set { pAnimation = value; }
		#endif
	}
	public bool isPAnimationExpanded;

	#endregion

	#region States

	private State currentState = null;

	private State StateDefault;

	//Default state, handles regular movement
	private void StateDefaultEnter()
	{

	}
	private void StateDefaultUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Movement(newPosition, input);

		if ( Grounded() )
		{
			if ( pJump.canJump && GBInput.GetButtonDown("Jump") )
				FiniteStateMachine.Transition(ref currentState, StateJump);
		}
		else
			FiniteStateMachine.Transition(ref currentState, StateInAir);
	}
	private void StateDefaultExit()
	{

	}

	//When jumping
	private State StateJump;

	private void StateJumpEnter()
	{

	}
	private void StateJumpUpdate()
	{

	}
	private void StateJumpExit()
	{

	}

	//When your in the air
	private State StateInAir;

	private void StateInAirEnter()
	{

	}
	private void StateInAirUpdate()
	{

	}
	private void StateInAirExit()
	{

	}


	#endregion

	#region Private Methods

	private void Movement(Vector3 newPosition, Vector2 input)
	{
		components.rigidBody.MovePosition(newPosition);
		components.animator.SetFloat(pAnimation.horizontalMovement, input.x);
		components.animator.SetFloat(pAnimation.verticalMovement, input.y);
	}

	private bool Grounded()
	{
		Vector3 position = components.rigidBody.position;
		position = LevelEditor.CubeRound(position.x, position.y);
		Vector3 cubeDirectlyUnderPosition = position - Vector3.up;
		if ( LevelEditor.Instance.LevelCubes.ContainsKey(cubeDirectlyUnderPosition) )
		{
			return true;
		}
		else
			return false;
	}

	#endregion

	[Serializable]
	public class AnimParamToBool
	{
		public string paramDescription;
		public string paramName;
		public bool hasParam;
	}
}
