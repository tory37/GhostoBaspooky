using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class AttackGhostoSpiritBall : BaseAttack
{

	[Header( "Animations" )]
	public AnimationClip clipSpiritBallAttack;
	[Tooltip( "The speed modifier on the state that the animation clip plays in." )]
	public float clipSpeedModifier;
	[Tooltip( "The percentage of the clip at which ghosto releases the ball." )]
	public float percentageToReleaseBall;

	[Tooltip( "The spirit ball prefab." )]
	public GameObject spiritBall;
	[Tooltip( "The bone for the hand at which the spirit ball will spawn." )]
	public GameObject attackingHand;

	[Tooltip( "The speed at which the spirit ball moves." )]
	public float spiritBallSpeed;
	[Tooltip( "The amount of time the spirit ball flies without collision before bursting." )]
	public float spiritBallDuration;

	[Tooltip("The damage this attack does.")]
	public int attackDamage;

	public AIScriptsSwitcher switcher;

	public override IEnumerator ExecuteAttack()
	{
		Animator anim = GetComponent<Animator>();

		anim.SetTrigger( "Spirit Ball Attack" );

		GameObject spiritBallInstance = (GameObject)Instantiate( spiritBall, attackingHand.transform.position, Quaternion.identity );
		spiritBallInstance.GetComponent<AttackObjectGhostoSpiritBall>().SudoConstructor( spiritBallSpeed, Vector3.right, spiritBallDuration, (percentageToReleaseBall * clipSpiritBallAttack.length) / clipSpeedModifier, attackDamage, GetComponent<GBActor>() );
		spiritBallInstance.transform.parent = attackingHand.transform;

		yield return new WaitForSeconds( clipSpiritBallAttack.length / clipSpeedModifier);
	}
}
