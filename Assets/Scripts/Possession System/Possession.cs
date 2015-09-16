using UnityEngine;
using System.Collections;

public class Possession : MonoBehaviour
{

	#region Editor Interface

	[SerializeField] private CharacterController GhostoPrefab;

	[SerializeField] private CharacterController currentCharacter;

	#endregion

	#region Public Interface

	public static Possession Instance
	{
		get { return instance; }
	}
	[SerializeField] private static Possession instance;

	/// <summary>
	/// The current character being controlled
	/// </summary>
	public static CharacterController CurrentCharacter
	{
		get { return Instance.currentCharacter; }
	}

	#endregion

	#region Mono Methods

	void Awake()
	{
		instance = this;
	}

	#endregion
}
