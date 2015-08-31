using UnityEngine;
using System.Collections;

/// <summary>
/// This Finite State Machine class has simple functions to allow for correct implementation of a simple State Machine
/// </summary>
public class FiniteStateMachine : MonoBehaviour {

    /// <summary>
    /// Correctly transitions from one state to another state
    /// </summary>
    /// <param name="currentState">current state</param>
    /// <param name="to">state to transition to</param>
    public static void Transition(ref State currentState, State to)
    {
        currentState.OnExit();
        currentState = to;
        currentState.OnEnter();
    }
}
