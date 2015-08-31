﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    //The function called when the player gets hit
    public static event Action<int> PlayerGotHit;

	#region Public Interface
	[Header("Actors")]
    [Tooltip("The player's BaseActor script")]
    public BaseController player;
    [Tooltip("The current actor's BaseActor script")]
    public BaseController currentActor;

    [Header("Player Variables")]
    [Tooltip("Current number of lives")]
    public int playerLives;
    [Tooltip("The max health of the player, no matter what form it's in")]
    public int playerMaxHealth;
    [Tooltip("The current health of the player, no matter what form it's in")]
    public int playerCurrentHealth;

    public Vector3 startPosition;

    public Slider healthSlider;
	#endregion

	/// <summary>
	/// Initialize Instance.
	/// </summary>
	void Start()
    {
        instance = this;

        player = GameObject.Find("Player").GetComponent<BaseController>();
        currentActor = player;

        SetActorSpecificEvents();
    }

    public void HealthAtZero()
    {
        playerLives -= 1;
        Debug.Log("Dead");
    }

    public static void PlayerGotHitMethod(int damage)
    {
        PlayerGotHit(damage);
    }

    public void SetActorSpecificEvents()
    {
        PlayerGotHit += currentActor.RecieveDamage;
    }

    public void ClearActorSpecificEvents()
    {
        if (PlayerGotHit != null)
            PlayerGotHit -= currentActor.RecieveDamage;
    }
}