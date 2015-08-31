using UnityEngine;
using System.Collections;

/// <summary>
/// This script goes on an object that can be destroyed
/// </summary>
public class DestructibleObject : MonoBehaviour {

	[Tooltip("The amount of damage it takes to destroy this object.")]
	public float health;

	/// <summary>
	/// Adds the required tag at start for safety
	/// </summary>
	void Start()
	{
		transform.AddTag( TagSystem.DestructibleObject );
	}

	/// <summary>
	/// Adds or removes health, depending on the sign of modifier
	/// </summary>
	/// <param name="modifier">adds this number to health of object</param>
	public void ModifyHealth (int modifier)
	{
		health += modifier;

		if (health <= 0)
		{
			Destroy( gameObject );
		}
	}
}
