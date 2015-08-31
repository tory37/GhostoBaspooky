using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSystem : MonoBehaviour
{

    public static CombatSystem instance;

    void Awake()
    {
        instance = this;
    }

    public IEnumerator MeleeAttackCoroutine(GBActor attacker, AttackMelee meleeAttack, Vector3 attackDirection)
    {
        attacker.GetComponent<AIControllerVariables>().AttackingCoroutineIsPlaying = true;

        yield return new WaitForSeconds((meleeAttack.clip.length / meleeAttack.speedMultiplier) * meleeAttack.animationStrikePercent);

        RaycastHit hit;
        if (Physics.SphereCast(new Ray(attacker.transform.position + attacker.GetComponent<AIControllerVariables>().centerPointOffset, attackDirection), meleeAttack.attackRadius, out hit, meleeAttack.attackDistance, attacker.GetComponent<AIControllerVariables>().attackableLayers))
        {
            GBActor target = hit.collider.GetComponent<GBActor>();
            if (target)
            {
                if (target == GameMaster.instance.currentActor)
                {
                    GameMaster.PlayerGotHitMethod(meleeAttack.attackDamage);
                }
                else
                {
                    target.GetComponent<BasePossessableAI>().RecieveDamage(meleeAttack.attackDamage);
                    Debug.Log(target.GetComponent<BasePossessableAI>().currentHealth);
                }
            }
        }
        

        attacker.GetComponent<AIControllerVariables>().AttackingCoroutineIsPlaying = false;
    }
}
