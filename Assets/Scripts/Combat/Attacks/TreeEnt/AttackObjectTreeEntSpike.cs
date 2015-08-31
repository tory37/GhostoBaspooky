using UnityEngine;
using System.Collections;

/// <summary>
/// Rotates the spike when doing the spike attack.
/// </summary>
public class AttackObjectTreeEntSpike : MonoBehaviour {

	/// <summary>
	/// The speed at which this spike rotates
	/// </summary>
    private float spikeRotateSpeed;
	/// <summary>
	/// The damage this object deals to an enemy
	/// </summary>
	private int damage;
	/// <summary>
	/// The actor (a tree ent normally) who performed this attack.
	/// </summary>
	private GBActor attacker;

	public void SudoConstructor(float spikeRotateSpeed, int damage, GBActor attacker)
	{
		this.spikeRotateSpeed = spikeRotateSpeed;
		this.damage = damage;
		this.attacker = attacker;
	}

    void Update()
    {
        //GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, spikeRotateSpeed * Time.deltaTime, 0f)));
		transform.Rotate( Vector3.up * spikeRotateSpeed * Time.deltaTime );
    }

	void OnTriggerEnter(Collider other)
	{
		if ( CombatScript.CheckIfEnemies( attacker.transform, other.transform ) )
		{
			if ( other.GetComponent<GBActor>() )
				CombatScript.DealDamage( damage, attacker, other.GetComponent<GBActor>() );
			else
				//DEBUG:
				Debug.LogWarning( "There is not a GBActor script attached to the object, but it has tag Enemy or Player.", other.transform );
		}
	}
}
