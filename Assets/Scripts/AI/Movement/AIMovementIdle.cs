using UnityEngine;
using System.Collections;

/// <summary>
/// This actor will not move, only stand idlely.  
/// Needs an idle animation.
/// </summary>
public class AIMovementIdle : BaseAIMovement
{
	#region Public Interface

	[Header( "Animation" )]
	[Tooltip( "The animator of this character." )]
	public Animator anim;
	[Tooltip( "The name of the animation that plays when the character is pacing back and fourth." )]
	public string animIdleFloatName;
	[Tooltip( "The float to set the animator at to get the correct animation at the correct speed for the character pacing." )]
	public float animIdleFloat;

	[Header( "BasePosition" )]
	[Tooltip( "The static position of the character." )]
	public Vector3 basePosition;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		anim.SetFloat( animIdleFloatName, animIdleFloat );
	}
	#endregion
}