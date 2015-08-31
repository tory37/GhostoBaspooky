using UnityEngine;
using System.Collections;

/// <summary>
/// This actor will pace between two points.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RotationVectors))]
[RequireComponent(typeof(Rigidbody))]
public class AIMovementPacer : BaseAIMovement
{
	#region Public Interface

	[Header( "Animation" )]
	[Tooltip( "The animator of this character." )]
	public Animator anim;
	[Tooltip("The name of the animation that plays when the character is pacing back and fourth.")]
	public string animPaceFloatName;
	[Tooltip("The float to set the animator at to get the correct animation at the correct speed for the character pacing.")]
	public float animPaceFloat;

	[Header("Movement")]
	[Tooltip("The negative position the character paces towards.")]
	public Vector3 negativePosition;
	[Tooltip( "The positive position the character paces towards." )]
	public Vector3 positivePosition;


	[Tooltip("The speed the character paces back and fourth at.")]
	public float paceVelocity;

	[Tooltip("Whether or not the character is pacing towards positive direction.  True if going positive, false if going negative.")]
	//Needs to be private eventually
	public bool goingPositive;

	#endregion

	#region Private Fields
    private RotationVectors rvs;

	private Quaternion deltaRotation;

	private Vector3 paceDirection;
	private float distanceBetweenPosAndNeg;

	private Rigidbody rb;
	#endregion

	#region MonoBehaviour Methods

	void Start()
	{
        rvs = GetComponent<RotationVectors>();

		rb = GetComponent<Rigidbody>();

		paceDirection = (positivePosition - negativePosition).normalized;
		distanceBetweenPosAndNeg = Vector3.Distance( positivePosition, negativePosition );

		goingPositive = true;
	}

	void FixedUpdate()
	{
		Movement();

		Rotation();
	}

	#endregion

    #region AI Methods
    void Movement()
	{
		rb.MovePosition( rb.position + (paceDirection * paceVelocity * Time.deltaTime) );
	
		CheckPosition();

		anim.SetFloat( animPaceFloatName, animPaceFloat );
	}

	void CheckPosition()
	{
		Vector3 currentPosition = rb.position;

		if ( Vector3.Distance(currentPosition, negativePosition) > distanceBetweenPosAndNeg )
		{
			paceDirection = ( negativePosition - positivePosition ).normalized;
			goingPositive = false;
		}
		else if ( Vector3.Distance(currentPosition, positivePosition) > distanceBetweenPosAndNeg )
		{
			paceDirection = ( positivePosition - negativePosition ).normalized;
			goingPositive = true;
		}
	}
	
	void Rotation()
	{
        Vector3 rotation = rb.rotation.eulerAngles;

        Vector3 targetRotation = ((paceDirection.x > 0 ? rvs.positiveXRotation : rvs.negativeXRoation) * Mathf.Abs(paceDirection.x)) +
                                 ((paceDirection.y > 0 ? rvs.positiveYRotation : rvs.negativeYRotaion) * Mathf.Abs(paceDirection.y)) +
                                 ((paceDirection.z > 0 ? rvs.positiveZRotation : rvs.negativeZRotaion) * Mathf.Abs(paceDirection.z));

        deltaRotation = Quaternion.Euler(new Vector3(Mathf.MoveTowardsAngle(rotation.x, targetRotation.x, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.y, targetRotation.y, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.z, targetRotation.z, rvs.rotationVelocity)));

        rb.MoveRotation(deltaRotation);
    }
    #endregion
}
