using UnityEngine;
using System.Collections;

/// <summary>
/// The base script that all attacks (ranged and melee) inherit from
/// </summary>
public abstract class BaseAttack : MonoBehaviour {

    public abstract IEnumerator ExecuteAttack();
}
