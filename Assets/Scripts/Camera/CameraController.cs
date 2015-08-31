using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	#region Public Interface
	[Tooltip("The vector 3 added to the position of the player, so the camera sits correctly.")]
    public float cameraYOffset;

    [Tooltip("The speed at which the camera follows the player.")]
    public float moveTowardsSpeed;

    [Tooltip("Every layer that is not the start/stop layer")]
    public LayerMask cameraMoveIgnoreMask;

	[Tooltip("The closest the camera can be to the wall horizontally")]
    public float playerToWallLengthHorizontal;
	[Tooltip("The closest the camera can be to the wall vertically")]
	public float playerToWallLengthVertical;
	[Tooltip("How off the camera should be from the current actor in each direction.")]
	public Vector3 offset;
	#endregion

    // Update is called once per frame
    void Update()
    {
		Vector3 actorPos = GameMaster.instance.currentActor.transform.position;

		//Vector3 deltaActorPos = actorPos - lastPosition;

		Vector3 newPos = transform.position;


		if (!Physics.Raycast(new Ray(actorPos, Vector3.left), playerToWallLengthHorizontal, cameraMoveIgnoreMask) && !Physics.Raycast(new Ray(actorPos, Vector3.right), playerToWallLengthHorizontal, cameraMoveIgnoreMask))
		{
			newPos.x = Mathf.MoveTowards(transform.position.x, actorPos.x, moveTowardsSpeed);
		}

		//Y
		if (!Physics.Raycast(new Ray(transform.position + offset, Vector3.down), playerToWallLengthVertical, cameraMoveIgnoreMask) && !Physics.Raycast(new Ray(transform.position + offset, Vector3.up), playerToWallLengthVertical, cameraMoveIgnoreMask))
		{
			newPos.y = Mathf.MoveTowards(transform.position.y, actorPos.y + offset.y, moveTowardsSpeed);
		}


		//float newYPos;
		//if (!Physics.Raycast(new Ray(SceneController.Instance.currentActor.transform.position - new Vector3(0f, 5, 0f), Vector3.up), playerToWallLengthVertical, cameraMoveIgnoreMask) && !Physics.Raycast(new Ray(SceneController.Instance.currentActor.transform.position + new Vector3(0f, 5, 0f), Vector3.down), playerToWallLengthVertical, cameraMoveIgnoreMask))
		//{
		//	newYPos = Mathf.MoveTowards(transform.position.y, SceneController.Instance.currentActor.transform.position.y + cameraYOffset, moveTowardsSpeed);
		//}
		//else
		//{
		//	newYPos = transform.position.y;
		//}

		transform.position = newPos;

  //      Debug.DrawRay(actorPos, Vector3.left * (playerToWallLengthHorizontal), Color.cyan);
		//Debug.DrawRay(actorPos, Vector3.right * (playerToWallLengthHorizontal), Color.cyan);
		//Debug.DrawRay(actorPos, Vector3.up * (playerToWallLengthVertical), Color.cyan);
		//Debug.DrawRay(actorPos, Vector3.down * (playerToWallLengthVertical), Color.cyan);
    }
}
