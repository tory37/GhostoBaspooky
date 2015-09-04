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

	public enum PossibleAnimations
	{
		HorizontalMovement = 0,
		VerticalMovement   = 1,
		TookDamage         = 2,
		FirstJump          = 3,
		SecondJump         = 4,
		Grounded           = 5
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
		public float groundedDistance;
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
		public List<AnimParamInfo> animationChecks = new List<AnimParamInfo>
		{
			new AnimParamInfo { paramDescription = "Horizontal Movement", paramName = "", hasParam = false },
			new AnimParamInfo { paramDescription = "Vertical Movement",   paramName = "", hasParam = false },
			new AnimParamInfo { paramDescription = "Took Damage",         paramName = "", hasParam = false },
			new AnimParamInfo { paramDescription = "First Jump",          paramName = "", hasParam = false },
			new AnimParamInfo { paramDescription = "Second Jump",         paramName = "", hasParam = false },
			new AnimParamInfo { paramDescription = "Grounded",            paramName = "", hasParam = false }
		};
	}

	[SerializeField] protected MovementTypes movementType;
	[SerializeField] protected NeededComponents components;
	[SerializeField] protected MovementProperties pMovement;
	[SerializeField] protected JumpProperties pJump;
	[SerializeField] protected AnimationProperties pAnimation;

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

	#region Protected Fields

	protected Dictionary<PossibleAnimations, AnimParamInfo> animationChecksDict;

	/// <summary>
	/// The current number of times the player has pressed the jump button 
	/// since being grounded
	/// </summary>
	protected int currentJump = 0;

	#endregion

	#region States

	protected State currentState = null;

	protected State StateFlierMovement;
	protected void StateFlierMovementEnter()
	{

	}
	protected void StateFlierMovementUpdate()
	{
		Vector2 input = new Vector2(GBInput.GetAxis("Horizontal"), GBInput.GetAxis("Vertical"));
		float horizontal =  input.x * pMovement.horizontalSpeed * Time.deltaTime;
		float vertical =    input.y * pMovement.verticalSpeed * Time.deltaTime;

		Vector3 newPosition = components.rigidBody.position + new Vector3(horizontal, vertical, 0.0f);

		Movement(newPosition, input);
	}
	protected void StateFlierMovementExit()
	{

	}

	

	#endregion

	#region Mono Methods

	protected virtual void Awake()
	{
		animationChecksDict = new Dictionary<PossibleAnimations, AnimParamInfo>();

		for (int i = 0; i < pAnimation.animationChecks.Count; i++ )
		{
			AnimParamInfo info = pAnimation.animationChecks[i];
			animationChecksDict.Add((PossibleAnimations)i, info);
		}
	}

	protected virtual void OnEnable()
	{
		
	}

	protected void FixedUpdate()
	{
		if ( currentState != null )
			currentState.OnUpdate();
		else
			Debug.LogWarning("Current state is null for " + transform.name, this);
	}

	#endregion

	#region Utility Functions

	protected void Movement(Vector3 newPosition, Vector2 input)
	{
		components.rigidBody.MovePosition(newPosition);

		if (animationChecksDict[PossibleAnimations.HorizontalMovement].hasParam)
			components.animator.SetFloat(pAnimation.horizontalMovement, input.x);

		if (animationChecksDict[PossibleAnimations.VerticalMovement].hasParam)
			components.animator.SetFloat(pAnimation.verticalMovement, input.y);
	}

	/// <summary>
	/// If there is a tile underneath, and the distance between the character's bottom and
	/// the top of the bottom cube is less than what it means to be "Grounded" it returns true
	/// This assumes that cubes are of height 1, and that the character's origin is at the 
	/// bottom of them, as the precendence was set in the design of the game.
	/// </summary>
	/// <returns>Whether the character is grounded or not</returns>
	protected bool OnGround()
	{
		Vector3 position = components.rigidBody.position;
		Debug.Log("Position " + position);
		position = LevelEditor.CubeRound(position.x, position.y);
		Debug.Log("Cube Round Position " + position);
		Vector3 cubeDirectlyUnderPosition = position - Vector3.up;
		Debug.Log("Look Position " + cubeDirectlyUnderPosition);

		LevelCubeObject groundedToCube;
		if ( GameManager.Instance.LevelCubes.TryGetValue(cubeDirectlyUnderPosition, out groundedToCube))
		{
			return true;
		}
		else
			return false;
	}

	protected void Jump()
	{
		//v = sprt((final velocity)^2 + 2 (acceleration) (distance))
		components.rigidBody.SetVelocityY(Mathf.Sqrt(-2 * Physics.gravity.y * pJump.jumpHeight));
	}

	#endregion

	[Serializable]
	public class AnimParamInfo
	{
		public string paramDescription;
		public string paramName;
		public bool hasParam;
		public bool isExpanded; 
	}
}
