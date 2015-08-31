using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This actor will randomly pace and stand idle between two points at random provided intervals.
/// </summary>
[RequireComponent (typeof (Animator))]
[RequireComponent(typeof(RotationVectors))]
[RequireComponent(typeof(Rigidbody))]
public class AIMovementPaceAndWait : BaseAIMovement
{
	#region Public Interface

	[Header( "Animation" )]
	[Tooltip( "The animator of this character." )]
	public Animator anim;
	[Tooltip( "The name of the animation that plays when the character is pacing back and fourth." )]
	public string animPaceFloatName;
	[Tooltip( "The float to set the animator at to get the correct animation at the correct speed for the character pacing." )]
	public float animPaceFloat;
	[Tooltip( "The name of the animation that plays when the character is pacing back and fourth." )]
	public string animIdleFloatName;
	[Tooltip( "The float to set the animator at to get the correct animation at the correct speed for the character pacing." )]
	public float animIdleFloat;

	[Header( "Movement" )]
	[Tooltip( "The negative position the character paces towards." )]
	public Vector3 negativePosition;
	[Tooltip( "The positive position the character paces towards." )]
	public Vector3 positivePosition;

	[Header( "Times" )]
	//[Tooltip( "The maximum amount of time the character would stay idle." )]
	//public float maxIdleTime;
	//[Tooltip( "The minimum amount of time the character would stay idle." )]
	//public float minIdleTime;
	[Tooltip("The min and max time the character would stay idle.")]
	[MinMaxRange(0, 10)]
	public MinMaxRange minMaxIdleTime;
	//[Tooltip( "The maximum amount of time the character would be pacing." )]
	//public float maxPaceTime;
	//[Tooltip( "The minimum amount of time the character would be pacing." )]
	//public float minPaceTime;
	[Tooltip( "The min and max time the character would be pacing." )]
	[MinMaxRange( 0, 10 )]
	public MinMaxRange minMaxPaceTime;

	[Tooltip( "The speed the character paces back and fourth at." )]
	public float paceVelocity;

	[Tooltip( "Whether or not the character is pacing towards positive direction.  True if going positive, false if going negative." )]
	//Needs to be private eventually
	public bool goingPositive;

	#endregion

	#region Private Fields
    private RotationVectors rvs;

	private Quaternion deltaRotation;

	private Vector3 paceDirection;
	private float distanceBetweenPosAndNeg;

	private Rigidbody rb;

	private bool isMoving;

	#endregion

	#region MonoBehaviour Methods
   
	void Start ()
	{
        rvs = GetComponent<RotationVectors>();

		rb = GetComponent<Rigidbody>();

		paceDirection = ( positivePosition - negativePosition ).normalized;
		distanceBetweenPosAndNeg = Vector3.Distance( positivePosition, negativePosition );

		StartCoroutine( StartIdle() );
	}

	void FixedUpdate ()
	{
		if ( isMoving )
		{
			Movement();
		}
		Rotation();
	}

	#endregion

    #region AI Methods
    void Movement ()
	{
		rb.MovePosition( rb.position + ( paceDirection * paceVelocity * Time.deltaTime ) );

		CheckPosition();

		anim.SetFloat( animPaceFloatName, animPaceFloat );
	}

	void CheckPosition ()
	{
		Vector3 currentPosition = rb.position;

		if ( Vector3.Distance( currentPosition, negativePosition ) > distanceBetweenPosAndNeg )
		{
			paceDirection = ( negativePosition - positivePosition ).normalized;
			goingPositive = false;
		}
		else if ( Vector3.Distance( currentPosition, positivePosition ) > distanceBetweenPosAndNeg )
		{
			paceDirection = ( positivePosition - negativePosition ).normalized;
			goingPositive = true;
		}
	}

	void Rotation ()
	{
        Vector3 rotation = rb.rotation.eulerAngles;

        Vector3 targetRotation = ((paceDirection.x > 0 ? rvs.positiveXRotation : rvs.negativeXRoation) * Mathf.Abs(paceDirection.x)) +
                                 ((paceDirection.y > 0 ? rvs.positiveYRotation : rvs.negativeYRotaion) * Mathf.Abs(paceDirection.y)) +
                                 ((paceDirection.z > 0 ? rvs.positiveZRotation : rvs.negativeZRotaion) * Mathf.Abs(paceDirection.z));

        deltaRotation = Quaternion.Euler(new Vector3(Mathf.MoveTowardsAngle(rotation.x, targetRotation.x, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.y, targetRotation.y, rvs.rotationVelocity), Mathf.MoveTowardsAngle(rotation.z, targetRotation.z, rvs.rotationVelocity)));

        rb.MoveRotation(deltaRotation);
	}

	IEnumerator StartIdle ()
	{
		isMoving = false;

		anim.SetFloat(animIdleFloatName, animIdleFloat);

		float idleTime = UnityEngine.Random.Range( minMaxIdleTime.rangeStart, minMaxIdleTime.rangeEnd );

		yield return new WaitForSeconds( idleTime );

		StartCoroutine( StartMovement() );

	}

	IEnumerator StartMovement()
	{
		isMoving = true;

		bool startGoingPositive = Convert.ToBoolean(UnityEngine.Random.Range( 0, 1 ));
		if (startGoingPositive)
		{
			paceDirection = ( positivePosition - negativePosition ).normalized;
			goingPositive = true;
		}
		else
		{
			paceDirection = ( negativePosition - positivePosition ).normalized;
			goingPositive = false;
		}

		float paceTime = UnityEngine.Random.Range( minMaxPaceTime.rangeStart, minMaxPaceTime.rangeEnd ); 

		yield return new WaitForSeconds( paceTime );

		StartCoroutine( StartIdle() );
    }
    #endregion
}
