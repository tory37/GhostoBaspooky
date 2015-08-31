using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour {

	#region Public Interface

	public GameObject startWall;
	public GameObject endWall;

	public List<GameObject> backgroundCanvases;
    private Dictionary<GameObject, float> backgroundYs;

	#endregion

	#region Private Variables

	private float wallToWallDistance;

	private List<float> backgroundMoveSpeedRatios;

	private Transform cameraTransform;
	private float cameraOldX;
	private float cameraDeltaPosition;
	#endregion

	void Start()
	{
        backgroundYs = new Dictionary<GameObject, float>();

        for (int i = 0; i < backgroundCanvases.Count; i++ )
        {
            backgroundYs.Add(backgroundCanvases[i], backgroundCanvases[i].transform.position.y);
        }

		wallToWallDistance = endWall.transform.position.x - startWall.transform.position.x;

		backgroundMoveSpeedRatios = new List<float>();

		for (int i = 0; i < backgroundCanvases.Count; i++)
		{
			float iWidth = backgroundCanvases[i].GetComponent<Renderer>().bounds.size.x;
			//float iWidthModified = iWidth - (2 * (Camera.main.transform.position.x - startWall.transform.position.x));
			float iMoveSpeedRatio = (iWidth / wallToWallDistance); 
			backgroundMoveSpeedRatios.Add(iMoveSpeedRatio);
		}

		cameraTransform = Camera.main.transform;
		cameraOldX = cameraTransform.position.x;
	}

	void Update()
	{
		cameraDeltaPosition = cameraTransform.position.x - cameraOldX;

		for (int i = 0; i < backgroundCanvases.Count; i++)
		{
            GameObject layer = backgroundCanvases[i];
            layer.transform.Translate(Vector3.right * cameraDeltaPosition * backgroundMoveSpeedRatios[i]);
            layer.transform.position = new Vector3(layer.transform.position.x, backgroundYs[layer], layer.transform.position.z);
		}

		cameraOldX = cameraTransform.position.x;
	}
}