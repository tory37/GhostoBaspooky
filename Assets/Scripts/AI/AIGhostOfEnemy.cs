using UnityEngine;
using System.Collections;

public class AIGhostOfEnemy : BasePossessableAI {

    [Tooltip("The speed at which the ghost will float off the screen")]
        public float floatSpeed;
	
	// Update is called once per frame
	void Update () {
	    transform.Translate(new Vector3(0f, floatSpeed * Time.deltaTime, 0f));
	}
}
