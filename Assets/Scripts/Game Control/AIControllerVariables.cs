using UnityEngine;
using System.Collections;

/// <summary>
/// These are the variables that are the same for an actors AI and Controller.
/// This eliminates these variables appearing in the AI and Controller.
/// </summary>
public class AIControllerVariables : MonoBehaviour {

    [Tooltip("Name of this creature")]
    public string characterName;

    [Header("Variables for AI and Controller")]
    [Tooltip("The vector from the origin of actor to the center of actor")]
    public Vector3 centerPointOffset;
    [Tooltip("The layers this actor are allowed to attack")]
    public LayerMask attackableLayers;

    [Header("Combat Shit")]
    [Tooltip("Normal material for this actor")]
    public Material[] regularMaterials;
    [Tooltip("Material for this actor when it gets attacked")]
    public Material[] gotHitMaterials;
    [Tooltip("The main renderer for this actor")]
    public Renderer actorMainRenderer;

    [Tooltip("The list of melee attacks for this actor")]
    public AttackMelee[] meleeAttacks;
    [Tooltip("The amount of time to wait after an attack before being able to attack again")]
    public float timeBetweenAttacks;

    public bool AttackingCoroutineIsPlaying { get; set; }
    void Start()
    {
        AttackingCoroutineIsPlaying = false;
    }

    public IEnumerator FlashHurtMeshRoutine()
    {
        actorMainRenderer.materials = gotHitMaterials;
        yield return new WaitForSeconds(.5f);
        actorMainRenderer.materials = regularMaterials;
    }
}
