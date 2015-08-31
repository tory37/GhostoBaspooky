using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AttackFrilledMouseSpit : BaseAttack {

	[Header( "Animations" )]
	public AnimationClip clipSpitAttack;
	[Tooltip( "The speed modifier on the state that the animation clip plays in." )]
	public float clipSpeedModifier;
	[Tooltip( "The percentage of the clip at which ghosto releases the ball." )]
	public float percentageToInstantiateSpit;

	[Tooltip( "The spirit ball prefab." )]
	public GameObject spit;
	[Tooltip( "The bone for the hand at which the spirit ball will spawn." )]
	public GameObject mouthBone;

	[Tooltip( "The speed at which the spirit ball moves." )]
	public float spitSpeed;
	[Tooltip( "The amount of time the spirit ball flies without collision before bursting." )]
	public float spitDuration;

	[Tooltip("The damage this attack does.")]
	public int attackDamage;

	public AIScriptsSwitcher switcher;

	public override IEnumerator ExecuteAttack()
	{
		Animator anim = GetComponent<Animator>();

		anim.SetTrigger( "Spit Attack" );

		yield return new WaitForSeconds( (clipSpitAttack.length * percentageToInstantiateSpit) / clipSpeedModifier );

		//GameObject spitInstance = (GameObject)Instantiate( spit, mouthBone.transform.position, Quaternion.identity );
		//spitInstance.GetComponent<AttackObjectFrilledMouseSpit>().SudoConstructor( spitSpeed, GetComponent<ControllerGhosto>().MoveDirection, spitDuration, attackDamage, GetComponent<GBActor>() );
		//spiritBallInstance.transform.parent = attackingHand.transform;
	}
}
