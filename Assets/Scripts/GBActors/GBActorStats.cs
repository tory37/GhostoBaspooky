using UnityEngine;
using System.Collections;

public class GBActorStats : GBActor
{

	public string ActorName { get { return actorName; } }
	[SerializeField] private string actorName;

	[Tooltip("Health is measured in number of hits a standard character deals.  Ghosto's attack deals 1 damage, boxes break at 1, basic enemies die at 1, etc.  -1 means invincible.")]
	[SerializeField] private int maxHealth;

	public int CurrentHealth { get { return currentHealth; } }
	[SerializeField] private int currentHealth;

	public float BlowbackForce
	{
		get { return blowbackForce; }
		#if UNITY_EDITOR
		set { blowbackForce = value; }
		#endif
	}
	[Tooltip( "The force applied to an enemy when he collides with this character." )]
	[SerializeField] private float blowbackForce;

	void Start()
	{
		currentHealth = maxHealth;
	}

	public void decreaseHealth( int subtractedHealth )
	{
		if ( maxHealth != -1 )
		{
			currentHealth -= subtractedHealth;

			if ( currentHealth <= 0 )
			{
				//Death stuff
			}
		}
	}

	public void IncreaseHealth( int addedHealth )
	{
		currentHealth += addedHealth;

		if ( currentHealth > maxHealth )
			currentHealth = maxHealth;
	}
}
