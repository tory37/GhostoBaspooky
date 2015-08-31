using UnityEngine;
using System;

/// <summary>
/// A representation of a state, for use with state machines.  Using this class ensures that states will have a Enter, Update, and Exit.  
/// </summary>
public class State  {

    /// <summary>
    /// A description to determine what this state represents
    /// </summary>
    public string stateDescription;

    /// <summary>
    /// This will be called when the state machine switches to this state.  "Initializer".
    /// </summary>
    public Action OnEnter;

    /// <summary>
    /// This should be called every frame, whether thats in update or fixed update
    /// </summary>
    public Action OnUpdate;

    /// <summary>
    /// This should be called when the State machine before the state machine transitions to a new state
    /// </summary>
    public Action OnExit;

    /// <summary>
    /// The constructor for a state
    /// </summary>
    /// <param name="stateDescription"></param>
    /// <param name="Enter"></param>
    /// <param name="Update"></param>
    /// <param name="Exit"></param>
    public State(string stateDescription, Action Enter, Action Update, Action Exit )
    {
        this.stateDescription = stateDescription;

        OnEnter = Enter;

        OnUpdate = Update;

        OnExit = Exit;
    }
}
