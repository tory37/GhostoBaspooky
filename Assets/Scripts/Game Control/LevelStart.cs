using UnityEngine;
using System.Collections;

/// <summary>
/// This script controls the start of every level, where the player
/// gets instantiated then moves through the opening level gate.
/// It also initializes everything that needs to be initialized.
/// Place this on the Game Master object
/// </summary>
public class LevelStart : MonoBehaviour {

	#region Player Vars
	/// <summary>
	/// The prefab for the Ghosto gameobject.
	/// </summary>
	public GameObject playerObject;

	public string playerHorizontalAnimFloatName;
	public string playerGroundedAnimBoolName;

	/// <summary>
	/// The position to instantiate the player gameobject
	/// </summary>
	public Vector3 playerStartingPosition;
	/// <summary>
	/// The roation to instantiate the player gameobject
	/// </summary>
	public Vector3 playerStartRotation;

	/// <summary>
	/// The position of the player that triggers the gate game
	/// object's open and close animation
	/// </summary>
	public Vector3 playerOpenGatePosition;

	/// <summary>
	/// The position of the player gameobject that will trigger 
	/// the end of this state and give control to the user.
	/// </summary>
	public Vector3 playerEndPosition;

	/// <summary>
	/// The speed to move the player
	/// </summary>
	public float playerMoveSpeed;

	#endregion Player Vars

	#region Gate Vars

	/// <summary>
	/// The gate that the player walks through at the 
	/// beginning of the level.
	/// </summary>
	public GameObject startingGate;
	public Animator gateAnim;
	public string gateOpenTriggerName;

	#endregion Gate Vars

	#region Things to enable

	private BaseController      playerController;
	public CameraController     cameraController;
	public PossessionSystem     possessionSystem;
	public GameMaster           gameMaster;
	public GameObject           leftBarrier;

	#endregion


	#region Private Variables

	private Rigidbody rb;
	private Animator playerAnim;
	private bool openedGate;

	#endregion Private Variables

	void Start()
	{
		rb = playerObject.GetComponent<Rigidbody>();

		playerAnim = playerObject.GetComponent<Animator>();

		playerController = playerObject.GetComponent<BaseController>();

		openedGate = false;

		playerAnim.SetFloat(playerHorizontalAnimFloatName, 1);
		playerAnim.SetBool(playerGroundedAnimBoolName, true);
	}

	void Update()
	{
		if ( rb.position.x < playerEndPosition.x )
		{
			if (!openedGate && rb.position.x > playerOpenGatePosition.x )
			{
				gateAnim.SetTrigger(gateOpenTriggerName);
				openedGate = true;
			}
			rb.MovePosition(rb.position + (Vector3.right * playerMoveSpeed * Time.deltaTime));
		}
		else
		{
			End();
		}
	}

	void End()
	{
		playerAnim.SetFloat(playerHorizontalAnimFloatName, 0);
		playerController.enabled = true;
		gameMaster.enabled = true;
		cameraController.enabled = true;
		possessionSystem.enabled = true;
		leftBarrier.SetActive(true);

		Destroy(this);
	}
}
