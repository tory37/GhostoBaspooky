using UnityEngine;
using System.Collections;

/// <summary>
/// Moves an actor towards the position of an object
/// </summary>
[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (RotationVectors))]
[RequireComponent (typeof (Rigidbody))]
public class AIMovementMoveTowardsObject : BaseAIMovement
{
	#region Public Interface

	[Header( "Animation" )]
	[Tooltip( "The animator of this character." )]
	public Animator anim;
	[Tooltip( "The name of the animation that plays when the character is pacing back and fourth." )]
	public string animPaceFloatName;
	[Tooltip( "The float to set the animator at to get the correct animation at the correct speed for the character pacing." )]
	public float animPaceFloat;

	[Header( "Target" )]
	[Tooltip( "The object this character moves towards." )]
	public GameObject targetObject;

	[Tooltip( "The speed the character moves towards the target object." )]
	public float paceVelocity;

	#endregion

	#region Private Fields
    private RotationVectors rvs;

	private Quaternion deltaRotation;

	private Vector3 paceDirection;

	private Rigidbody rb;
	#endregion

	#region MonoBehaviour Methods

	void Start ()
	{
        rvs = GetComponent<RotationVectors>();

		rb = GetComponent<Rigidbody>();

		paceDirection = ( targetObject.transform.position - transform.position ).normalized;
	}

	void FixedUpdate ()
	{
		Movement();

		Rotation();
	}

	#endregion

    #region AI Methods
    void SetTarget(GameObject target)
    {
        targetObject = target;
    }

    void Movement ()
	{
		float vTimesTime = paceVelocity * Time.deltaTime;
		float deltaMove = (rb.position - targetObject.transform.position).magnitude - Vector3.MoveTowards( rb.position, targetObject.transform.position, vTimesTime ).magnitude;
		float animFloatModifier = deltaMove / vTimesTime;

		rb.MovePosition( rb.position + ( paceDirection * vTimesTime ) );

		anim.SetFloat( animPaceFloatName, animPaceFloat * animFloatModifier);

		paceDirection = ( targetObject.transform.position - transform.position ).normalized;
	}

	void Rotation ()
	{
		Vector3 rotation = rb.rotation.eulerAngles;

        Vector3 targetRotation = ((paceDirection.x > 0 ? rvs.positiveXRotation : rvs.negativeXRoation) * Mathf.Abs(paceDirection.x)) +
                                 ((paceDirection.y > 0 ? rvs.positiveYRotation : rvs.negativeYRotaion) * Mathf.Abs(paceDirection.y)) +
                                 ((paceDirection.z > 0 ? rvs.positiveZRotation : rvs.negativeZRotaion) * Mathf.Abs(paceDirection.z));

        deltaRotation = Quaternion.Euler(new Vector3(Mathf.MoveTowardsAngle(rotation.x, targetRotation.x, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.y, targetRotation.y, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.z, targetRotation.z, rvs.rotationVelocity)));

		rb.MoveRotation( deltaRotation );
    }
    #endregion
}