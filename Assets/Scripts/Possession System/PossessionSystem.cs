using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessionSystem : MonoBehaviour
{

	/// <summary>
	/// Singleton Instance of the possession system in this scene.
	/// </summary>
	public static PossessionSystem Instance { get { return instance; } }
	private static PossessionSystem instance;

	#region Public Interface
	[Header( "Possession Variables" )]
	[Tooltip( "The players 'Suck Range' (Kirby)." )]
	public float posessionSuckRange;
	[Tooltip( "The distance of the cast that reaches towards the target when in range" )]
	float possessionCastDistance;
	[Tooltip( "The vertical offset from the common origin of character models.  This origin is expected to be at the very bottom." )]
	public Vector3 castYOffset;
	[Tooltip( "The time it takes for the player to possess the target" )]
	public float possessTime;
	[Tooltip( "The amount possessionCastDistance increments, based on possessTime" )]
	float possessionCastDistanceIncrement;

	[Header( "Audio" )]
	public float sfxVolumes;
	[Tooltip( "Sound when possessing" )]
	public AudioClip sfxPossess;
	[Tooltip( "Sound when exit possession" )]
	public AudioClip sfxExitPossession;

	[Header( "Testing" )]
	[Tooltip( "MOVE. This is only relevant for the ghost (player), if the animation forces the model to rotate 180 degrees. " )]
	public bool backwardsAnimation;
	#endregion

	// The target AI that is currently in range, may be null
	private BaseAI gettingPossessedTarget;

	private List<BaseAI> targetsEnabledAIs;

	// Use this for initialization
	void Start()
	{
		instance = this;

		possessionCastDistance = 0f;
		possessionCastDistanceIncrement = 0f;

		targetsEnabledAIs = new List<BaseAI>();
	}

	void Update()
	{
		Vector3 direction;
		//Debug.Log(Vector3.Angle(currentActor.transform.forward, Vector3.right));
		if ( !backwardsAnimation )
		{
			if ( Vector3.Angle( GameMaster.Instance.currentActor.transform.forward, Vector3.right ) < 90 )
			{
				direction = Vector3.right;
			}
			else
			{
				direction = Vector3.left;
			}
			Debug.DrawRay( GameMaster.Instance.currentActor.transform.position + castYOffset, direction * posessionSuckRange, Color.red );
			//Debug.Log(direction);
		}
		else
		{
			if ( Vector3.Angle( -GameMaster.Instance.currentActor.transform.forward, Vector3.right ) < 90 )
			{
				direction = Vector3.right;
			}
			else
			{
				direction = Vector3.left;
			}
			Debug.DrawRay( GameMaster.Instance.currentActor.transform.position + castYOffset, direction * posessionSuckRange, Color.red );
			//Debug.Log(direction);
		}

		//Resets the increment to 0 when the button stops being pressed.
		if ( GBInput.GetButtonUp( "Possess" ) )
		{
			possessionCastDistance = 0f;
			if ( gettingPossessedTarget != null )
			{
				Debug.Log( gettingPossessedTarget );
				gettingPossessedTarget.GetComponent<BasePossessableAI>().StopGettingPossessed();
				gettingPossessedTarget = null;
			}
			for ( int i = 0; i < targetsEnabledAIs.Count; i++ )
			{
				targetsEnabledAIs[i].enabled = true;
			}
			targetsEnabledAIs.Clear();
		}

		//Sets the amount the actual possession raycast increments so that it takes the same amount of time everytime, no matter the distance of the target
		if ( GBInput.GetButtonDown( "Possess" ) )
		{
			RaycastHit hit;
			if ( Physics.Raycast( new Ray( GameMaster.Instance.currentActor.transform.position + castYOffset, direction ), out hit, posessionSuckRange ) )
			{
				if ( hit.collider.GetComponent<BaseController>() )
				{
					targetsEnabledAIs = new List<BaseAI>( hit.collider.GetComponents<BaseAI>() );
					for ( int i = 0; i < targetsEnabledAIs.Count; i++ )
					{
						if ( !targetsEnabledAIs[i].enabled )
						{
							targetsEnabledAIs.RemoveAt( i );
							i++;
						}
					}
					for ( int i = 0; i < targetsEnabledAIs.Count; i++ )
					{
						targetsEnabledAIs[i].enabled = false;
					}

					Debug.Log( "Has base controller.", hit.collider );
					possessionCastDistanceIncrement = hit.distance / possessTime;
					//DEBUG:
					Debug.Log( "Increment: " + possessionCastDistanceIncrement );
				}
			}
		}
		if ( GBInput.GetButton( "Possess" ) )
		{
			RaycastHit hit;
			//If the actual possession raycast hits the target, possession occurs
			if ( Physics.Raycast( new Ray( GameMaster.Instance.currentActor.transform.position + castYOffset, direction ), out hit, possessionCastDistance ) )
			{
				if ( hit.collider.GetComponent<BaseController>() )
				{
					Possess( hit.collider.GetComponent<BaseController>() );
				}
			}
			//If the actual possession raycast does not hit the target, extend it further towards the target
			else if ( Physics.Raycast( new Ray( GameMaster.Instance.currentActor.transform.position + castYOffset, direction ), out hit, posessionSuckRange ) )
			{
				if ( hit.collider.GetComponent<BaseController>() )
				{
					possessionCastDistance += possessionCastDistanceIncrement;
					Debug.DrawRay( GameMaster.Instance.currentActor.transform.position + castYOffset, direction * possessionCastDistance, Color.green );
				}
			}
		}

		if ( GBInput.GetButtonDown( "Exit Possess" ) )
		{
			ExitPossession();
		}
	}

	void Possess( BaseController target )
	{
		//targetsEnabledAIs.Clear();

		//gettingPossessedTarget = null;

		////Player possession sound
		//AudioSource.PlayClipAtPoint( sfxPossess, Camera.main.transform.position, sfxVolumes );

		////Clear the events for the actor being exited
		//GameMaster.Instance.ClearActorSpecificEvents();
		////Exit this actor
		//GameMaster.Instance.currentActor.ExitPossessionSpecifics();

		////Set new actor
		//GameMaster.Instance.currentActor = target;
		////Possess this actor
		//GameMaster.Instance.currentActor.PossessionSpecifics();
		////Set the events for the new actor being controlled
		//GameMaster.Instance.SetActorSpecificEvents();
	}

	public void ExitPossession()
	{
		//if ( GameMaster.Instance.currentActor != GameMaster.Instance.player )
		//{
		//	//Play exit possession sound
		//	AudioSource.PlayClipAtPoint( sfxExitPossession, Camera.main.transform.position, sfxVolumes );

		//	//Move the player object to here
		//	GameMaster.Instance.player.transform.position = GameMaster.Instance.currentActor.transform.position;


		//	//Clear the events for the actor being exited
		//	GameMaster.Instance.ClearActorSpecificEvents();
		//	//Exit this actor
		//	GameMaster.Instance.currentActor.ExitPossessionSpecifics();

		//	//Set new actor
		//	GameMaster.Instance.currentActor = GameMaster.Instance.player;
		//	//Take control of the ghost
		//	GameMaster.Instance.currentActor.PossessionSpecifics();
		//	//Set the events for the new actor being controlled
		//	GameMaster.Instance.SetActorSpecificEvents();
		//}
	}
}
