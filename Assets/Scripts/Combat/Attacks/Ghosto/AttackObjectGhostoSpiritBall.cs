using UnityEngine;
using System.Collections;

/// <summary>
/// Rotates the spike when doing the spike attack.
/// </summary>
public class AttackObjectGhostoSpiritBall : MonoBehaviour
{
	[Tooltip("The particle system that plays when this gets destroyed.")]
	public GameObject burstParticles;

	/// <summary>
	/// The speed at which this ball moves
	/// </summary>
	private float moveSpeed;
	/// <summary>
	/// The direction for the ball to travel
	/// </summary>
	private Vector3 attackDirection;
	/// <summary>
	/// How long the attack continues moving without collision before bursting
	/// </summary>
	private float attackDuration;
	/// <summary>
	/// Thet time to wait before releasing the ball
	/// </summary>
	private float waitToReleaseTime;
	/// <summary>
	/// The damage this object deals to an enemy
	/// </summary>
	private int damage;
	/// <summary>
	/// The actor (a Ghosto normally) who performed this attack.
	/// </summary>
	private GBActor attacker;

	private Coroutine waitToReleaseBall;
	private Coroutine waitForBallDeath;

	private bool isReleased;

	public void SudoConstructor( float moveSpeed, Vector3 direction, float attackDuration, float releaseTime, int damage, GBActor attacker )
	{
		this.moveSpeed = moveSpeed;
		this.attackDirection = direction;
		this.attackDuration = attackDuration;
		this.waitToReleaseTime = releaseTime;
		this.damage = damage;
		this.attacker = attacker;
	}

	void Start()
	{
		waitToReleaseBall = StartCoroutine( WaitToReleaseBall( waitToReleaseTime ) );
		waitForBallDeath = StartCoroutine( WaitForBallDeath( attackDuration ) );
	}

	IEnumerator WaitToReleaseBall(float time)
	{
		isReleased = false;
		yield return new WaitForSeconds( time );
		isReleased = true;
		transform.SetPositionZ( attacker.transform.position.z );
		transform.parent = null;
	}

	IEnumerator WaitForBallDeath(float time)
	{
		yield return new WaitForSeconds( time );
		Burst();
	}

	void Update()
	{
		//GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, spikeRotateSpeed * Time.deltaTime, 0f)));
		if (isReleased)
			transform.Translate(attackDirection * moveSpeed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter( Collider other )
	{
		//Make sure this attack doesnt hit the caster
		if (other.transform == attacker.transform)
			return;

		GBActor otherActor = other.GetComponent<GBActor>();
		if ( otherActor != null )
		{
			if ( CombatScript.CheckIfEnemies( attacker.transform, other.transform ) )
			{
				CombatScript.DealDamage( damage, attacker, otherActor );
			}
		}

		DestructibleObject otherDO = other.GetComponent<DestructibleObject>();
		if (otherDO != null)
		{
			otherDO.ModifyHealth( -damage );
		}

		Burst();
	}

	void Burst()
	{
		if (waitForBallDeath != null)
			StopCoroutine( waitForBallDeath );
		if ( waitToReleaseBall != null )
			StopCoroutine( waitToReleaseBall );

		GameObject burstParts = (GameObject)Instantiate( burstParticles, transform.position, Quaternion.identity );
		StartCoroutine( WaitForBurst( burstParts ) );
		Destroy( this.gameObject );
	}

	IEnumerator WaitForBurst(GameObject burstParts)
	{
		yield return new WaitForSeconds( burstParticles.GetComponent<ParticleSystem>().duration );
		Destroy( burstParts );
	}
}
