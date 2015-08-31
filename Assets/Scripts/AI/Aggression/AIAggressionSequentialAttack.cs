using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An actor with this aggression script will perform a sequence of attacks from its list of attacks.  
/// The attacks can be performed at random time intervals, or on a so many seconds basis.
/// </summary>
public class AIAggressionSequentialAttack : BaseAIAggression
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
    [Tooltip("A list of each possible attack the actor can perform.  Place each one in the order they are to be performed.  If attacks repeat, place them as many times as they occur, where they occur.")]
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
        int sequenceIndex = 0;

        while (true)
        {
            yield return StartCoroutine(attacks[sequenceIndex].ExecuteAttack());

            sequenceIndex++;
            if (sequenceIndex >= attacks.Count)
                sequenceIndex = 0;

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
