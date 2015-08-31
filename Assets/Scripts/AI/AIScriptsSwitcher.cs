using UnityEngine;
using System.Collections;

public class AIScriptsSwitcher : MonoBehaviour
{

	public Transform associatedActor;

	public BaseAIMovement notAggrodMovementScript;
	public BaseAIAggression notAggrodAggressionScript;

	public BaseAIMovement aggrodMovementScript;
	public BaseAIAggression aggrodAggressionScript;

	public GameObject ObjectThatEnteredTrigger { get { return objectThatEnteredTrigger; } set { ObjectThatEnteredTrigger = value; } }
	protected GameObject objectThatEnteredTrigger;

	private Vector3 position;
	private Quaternion rotation;

	void OnEnable()
	{
		position = transform.position;
		rotation = transform.rotation;
	}

	void Update()
	{
		transform.position = position;
		transform.rotation = rotation;
	}

	void OnTriggerEnter( Collider other )
	{
		if ( other.transform.HasTag( TagSystem.Player ) )
		{
			objectThatEnteredTrigger = other.gameObject;

			SwitchToAggrod();
		}
	}

	void OnTriggerExit( Collider other )
	{
		if ( other.transform.HasTag( TagSystem.Player ) )
		{
			objectThatEnteredTrigger = null;

			SwitchToNotAggrod();
		}
	}

	protected virtual void SwitchToAggrod()
	{
		notAggrodMovementScript.enabled = false;
		notAggrodAggressionScript.enabled = false;

		aggrodMovementScript.enabled = true;
		aggrodAggressionScript.enabled = true;
	}

	protected virtual void SwitchToNotAggrod()
	{
		notAggrodMovementScript.enabled = true;
		notAggrodAggressionScript.enabled = true;

		aggrodMovementScript.enabled = false;
		aggrodAggressionScript.enabled = false;
	}

}
