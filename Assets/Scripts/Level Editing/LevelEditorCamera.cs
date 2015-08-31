using UnityEngine;
using System.Collections;

public class LevelEditorCamera : MonoBehaviour {

	public float moveSpeed;

	public Camera thisCamera;

	// Update is called once per frame
	void Update () {
		float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
		float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
		float forward = Input.GetAxis("Forward") * moveSpeed * Time.deltaTime;

		transform.Translate(horizontal, vertical, 0f);

		thisCamera.orthographicSize = Mathf.Clamp(thisCamera.orthographicSize + forward, .5f, Mathf.Infinity);
	}
}
