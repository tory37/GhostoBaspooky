using UnityEngine;
using System.Collections;

public class GameFlow : MonoBehaviour {

	public void SwitchScenes(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}
}
