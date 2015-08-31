using UnityEngine;
using System.Collections;

public class AIBall : BaseAI {

    void Update()
    {
        GetComponent<Renderer>().material.color = new Color(Random.Range(0, 255f), Random.Range(0, 255f), Random.Range(0, 255f));
    }
}
