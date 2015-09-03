using UnityEngine;
using System.Collections;
using System;

public class CharacterController : MonoBehaviour {

	#region Enums
	
	public enum MovementType
	{
		Walk,
		Hop
	}

	#endregion

	#region Editor Interface

	[Serializable]
	protected class Components
	{
		public Rigidbody rigidBody;
		public Animator animator;
		public RotationVectors rotationVectors;
	}

	[Serializable]
	protected class MovementProperties
	{
		public float horizontalSpeed;
		public float verticalSpeed;
	}

	[Serializable]
	protected class JumpProperties
	{
		public float jumpHeight;
		public bool canJump;
		public bool canDoubleJump;
	}

	[Serializable]
	protected class AnimationProperties
	{
		public string horizontalMovement;
		public string verticalMovement;
		public string gotHurt;
		public string firstJump;
		public string secondJump;
	}

	[SerializeField] private Components components;
	[SerializeField] private MovementProperties pMovement;
	[SerializeField] private JumpProperties pJump;
	[SerializeField] private AnimationProperties pAnimation;

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

}
