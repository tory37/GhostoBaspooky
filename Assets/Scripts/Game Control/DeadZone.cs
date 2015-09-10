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
            else if (other.transform.GetComponent<GBActor>() != GameMaster.Instance.player)
            {
                PossessionSystem.Instance.ExitPossession();
                GameMaster.Instance.player.transform.position = GameMaster.Instance.startPosition;
            }
            else
            {
                other.transform.position = GameMaster.Instance.startPosition;
            }
        }
    }
}
