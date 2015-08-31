using UnityEngine;
using System.Collections;

/// <summary>
/// Rotates the spike when doing the spike attack.
/// </summary>
public class AttackObjectFrilledMouseSpit : MonoBehaviour
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
	/// The damage this object deals to an enemy
	/// </summary>
	private int damage;
	/// <summary>
	/// The actor (a Ghosto normally) who performed this attack.
	/// </summary>
	private GBActor attacker;

	//private Coroutine waitForSpitDeath;

	private bool isReleased;

	public void SudoConstructor( float moveSpeed, Vector3 direction, float attackDuration, float releaseTime, int damage, GBActor attacker )
	{
		this.moveSpeed = moveSpeed;
		this.attackDirection = direction;
		this.attackDuration = attackDuration;
		this.damage = damage;
		this.attacker = attacker;
	}

	void Start()
	{
		//waitForSpitDeath = StartCoroutine( WaitForSpitDeath( attackDuration ) );
	}

	IEnumerator WaitForSpitDeath(float time)
	{
		yield return new WaitForSeconds( time );
		Destroy( gameObject );
	}

	void Update()
	{
		transform.Translate(attackDirection * moveSpeed * Time.deltaTime, Space.World);
	}

	void OnTriggerEnter( Collider other )
	{
		if (other.transform == attacker.transform)
		{
			return;
		}
		if ( other.GetComponent<GBActor>() )
		{
			if ( CombatScript.CheckIfEnemies( attacker.transform, other.transform ) )
			{
				CombatScript.DealDamage( damage, attacker, other.GetComponent<GBActor>() );
			}
		}

		Destroy( gameObject );
		
	}
}
