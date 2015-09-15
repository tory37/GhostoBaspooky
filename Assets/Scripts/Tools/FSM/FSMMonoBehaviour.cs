using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FSMMonoBehaviour : MonoBehaviour
{
	/// <summary>
	/// This is the state that the machine will always start in.
	/// </summary>
	protected State startState;

	/// <summary>
	/// This is the state that represents which state this machine is
	/// currently in
	/// </summary>
	protected State currentState;

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			Debug.Log("FSM is in state: " + currentState.Description, this);
		}
		if ( currentState.OnUpdate != null )
		{
			currentState.OnUpdate();
		}
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the 
	/// MonoBehaviour is enabled.
	/// </summary>
	private void FixedUpdate()
	{
		if ( currentState.OnFixedUpdate != null )
		{
			currentState.OnFixedUpdate();
		}
	}

	/// <summary>
	/// LateUpdate is called every frame, if the Behaviour is enabled.
	/// LateUpdate is called after all Update functions have been called. 
	/// </summary>
	private void LateUpdate()
	{
		for ( int i = 0; i < currentState.Transitions.Length; i++ )
		{
			if ( currentState.Transitions[i].Condition() )
				Transition(currentState.Transitions[i].ToState);
		}
	}

	/// <summary>
	/// This function transitions between different states.
	/// </summary>
	/// <param name="toState"></param>
	protected void Transition( State toState )
	{
		if (currentState.OnExit != null)
			currentState.OnExit();
		currentState = toState;
		if (currentState.OnEnter != null)
			currentState.OnEnter();
	}

	/// <summary>
	/// Override this function and declare the states.
	/// Lastly, assign the start state.
	/// </summary>
	protected virtual void Initialize(){ }

	protected virtual void Awake()
	{
		Initialize();

		if ( startState != null )
		{
			currentState = startState;
			currentState.OnEnter();
		}
		else
			Debug.LogError( "Start state is null.  It must be assigned to at the end of Initialize." );
	}
}

public class Transition
{
	public Func<bool> Condition { get; private set; }
	public State ToState { get; private set; }

	public Transition( Func<bool> condition, State toState )
	{
		this.Condition = condition;
		this.ToState = toState;
	}
}

public class State
{
	public string Description   { get; private set; }
	public Action OnEnter       { get; private set; }
	public Action OnUpdate      { get; private set; }
	public Action OnFixedUpdate { get; private set; }
	public Action OnLateUpdate  { get; private set; }
	public Action OnExit        { get; private set; }

	public Transition[] Transitions { get; private set; }

	public State (string description, Action enter, Action update, Action fixedUpdate, Action lateUpdate, Action exit)
	{
		Description   = description;
		OnEnter       = enter;
		OnUpdate      = update;
		OnFixedUpdate = fixedUpdate;
		OnLateUpdate  = lateUpdate;
		OnExit        = exit;
	}

	public void SetTransitions(Transition[] transitions)
	{
		Transitions = transitions;
	}
}