using UnityEngine;
using System.Collections;

public class DeadZone : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
		if ( other.transform.HasTag( TagSystem.Player ) )
        {
			if ( !other.transform.HasTag( TagSystem.Player ) )
			{
				GameObject.Destroy( other.gameObject );
			}
            else if (other.transform.GetComponent<GBActor>() != GameMaster.instance.player)
            {
                PossessionSystem.instance.ExitPossession();
                GameMaster.instance.player.transform.position = GameMaster.instance.startPosition;
            }
            else
            {
                other.transform.position = GameMaster.instance.startPosition;
            }
        }
    }
}
