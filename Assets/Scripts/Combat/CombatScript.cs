using UnityEngine;
using System.Collections.Generic;

public static class CombatScript {

	public static bool CheckIfEnemies(Transform t1, Transform t2)
	{
		return (t1.HasTag( TagSystem.Player ) && t2.HasTag( TagSystem.Enemy )) || (t1.HasTag( TagSystem.Enemy ) && t2.HasTag( TagSystem.Player )); 
	}

	public static void DealDamage(int damage, GBActor dealer, GBActor reciever)
	{
		reciever.GetComponent<GBActorStats>().decreaseHealth( damage );
	}
}
