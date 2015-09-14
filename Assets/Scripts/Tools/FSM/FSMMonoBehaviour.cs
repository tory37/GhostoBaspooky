using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FSMMonoBehaviour : MonoBehaviour
{
	protected State currentState;

	protected State[] states;

	protected void Update()
	{
		if ( currentState.OnUpdate != null )
		{
			currentState.OnUpdate();
		}
	}

	protected void FixedUpdate()
	{
		if ( currentState.OnFixedUpdate != null )
		{
			currentState.OnFixedUpdate();
		}
	}

	protected void LateUpdate()
	{
		for ( int i = 0; i < currentState.Transitions.Length; i++ )
		{
			if ( currentState.Transitions[i].Condition() )
				Transition(currentState.Transitions[i].ToState);
		}
	}

	protected void Transition( State toState )
	{
		currentState.OnExit();
		currentState = toState;
		currentState.OnEnter();
	}
}

public class Transition
{
	public Func<bool> Condition { get; private set; }
	public State ToState { get; private set; }

	public Transition( Func<bool> condition, int toStateIndex = 0 )
	{
		this.Condition = condition;
		this.ToState = states[toStateIndex];
	}
}

public class State
{
	public Action OnEnter       { get; private set; }
	public Action OnUpdate      { get; private set; }
	public Action OnFixedUpdate { get; private set; }
	public Action OnLateUpdate  { get; private set; }
	public Action OnExit        { get; private set; }

	public Transition[] Transitions { get; private set; }

	public State (Action enter, Action update, Action fixedUpdate, Action lateUpdate, Action exit, Transition[] transitions)
	{
		OnEnter       = enter;
		OnUpdate      = update;
		OnFixedUpdate = fixedUpdate;
		OnLateUpdate  = lateUpdate;
		OnExit        = exit;

		Transitions = transitions;
	}
}