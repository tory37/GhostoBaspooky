using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An actor with this aggression script will perform random attacks from its list of attacks.  
/// The attacks can be performed at random time intervals, or on a so many seconds basis
/// </summary>
public class AIAggressionIntervalAttack : BaseAIAggression
{
    #region Public Interface
    [Header("Timing")]
    [Tooltip("Is this attack random using the intervals, or does it happen so many seconds.")]
    public bool useRandomTimeIntervals;
    [Tooltip("The minimum time to wait between random timed attacks.")]
    public float minRandomTimeBetweenAttacks;
    [Tooltip("The maximum time to wait between random timed attacks.")]
    public float maxRandomTimeBetweenAttacks;
    [Tooltip("The time to wait between each attack if it is not random intervals.")]
    public float constantTimeBetweenAttacks;

    [Header("Attacks")]
    [Tooltip("A list of each possible attack the actor can perform.  They will be chosen randomly.")]
    public List<BaseAttack> attacks;
    #endregion

    #region Methods
    void OnEnable()
    {
        StartCoroutine("AttackSequence");
    }

    void OnDisable()
    {
        StopCoroutine("AttackSequence");
    }

    IEnumerator AttackSequence()
    {
        while (true)
        {
            int randomAttackIndex = Random.Range(0, attacks.Count - 1);

            yield return StartCoroutine(attacks[randomAttackIndex].ExecuteAttack());

            float timeBetweenAttacks;
            if (useRandomTimeIntervals)
                timeBetweenAttacks = Random.Range(minRandomTimeBetweenAttacks, maxRandomTimeBetweenAttacks);
            else
                timeBetweenAttacks = constantTimeBetweenAttacks;

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }
    #endregion
}
