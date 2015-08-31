using UnityEngine;
using System.Collections;

public class AttackMelee : BaseAttack {

    [Tooltip("The name of the attack")]
    public string attackName;
    [Tooltip("The distance from the actor the spherecast should check")]
    public float attackDistance;
    [Tooltip("The radius of the spherecast")]
    public float attackRadius;
    [Tooltip("The damage the actor deals when attacking")]
    public int attackDamage;
    [Tooltip("The clip that plays for this attack")]
    public AnimationClip clip;
    [Tooltip("The percent of the attack animation that the strike actually happens")]
    public float animationStrikePercent;
    [Tooltip("Mechanim state speed multiplier")]
    public float speedMultiplier;

    public override IEnumerator ExecuteAttack()
    {
        GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds((clip.length / speedMultiplier) * animationStrikePercent);
    }
}
