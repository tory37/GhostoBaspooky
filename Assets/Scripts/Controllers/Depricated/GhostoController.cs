using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This controls the GBActor Ghost, who is our main character.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RotationVectors))]
[RequireComponent(typeof(Rigidbody))]
public class GhostoController : BaseController
{


    #region Functions

	#endregion

	#region Possession
	public override void PossessionSpecifics()
	{

	}

	public override void ExitPossessionSpecifics()
	{

	}
	#endregion
}
