using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class AttackTreeEntSpike : BaseAttack {

    [Header("Animations")]
    public AnimationClip clipSpikeAttackKneel;
    public AnimationClip clipSpikeAttackStandingUp;
	[Tooltip("The up and down animation of the spike.")]
    public AnimationClip spikeAnimation;

	[Tooltip("The spike prefab.")]
    public GameObject spike;

	[Tooltip("The rotation of the spike to spawn.")]
    public Vector3 spikeSpawnRotation;
	[Tooltip("The speed at which the spike rotates.")]
	public float spikeRotationSpeed;

	public int attackDamage;

    public AIScriptsSwitcher switcher;

    public override IEnumerator ExecuteAttack()
    {
        Vector3 targetPosition = GameMaster.Instance.currentActor.transform.position;

        Animator anim = GetComponent<Animator>();

        anim.SetTrigger("Spike Attack Start");
        yield return new WaitForSeconds(clipSpikeAttackKneel.length);

        GameObject spikeInstance = (GameObject)Instantiate(spike, targetPosition, Quaternion.Euler(spikeSpawnRotation));
		spikeInstance.GetComponent<AttackObjectTreeEntSpike>().SudoConstructor( spikeRotationSpeed, attackDamage, GetComponent<GBActor>() );

        yield return new WaitForSeconds(spikeAnimation.length);
        Destroy(spikeInstance);

        anim.SetTrigger("Spike Attack End");
        yield return new WaitForSeconds(clipSpikeAttackStandingUp.length);
    }
}
