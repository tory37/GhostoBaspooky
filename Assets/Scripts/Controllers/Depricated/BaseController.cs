using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(GBActorStats))]
public abstract class BaseController : MonoBehaviour {

    [Header("Animation")]
    [Tooltip("The animator that controls the player's animations.")]
    public Animator anim;
	public AnimationClip hurtClip;
	public float hurtClipModifier;

	void Start()
	{
		if (!transform.HasTag(TagSystem.HasController))
		{
			transform.AddTag( TagSystem.HasController );
		}
	}

	#region Possession
	public virtual void PossessionSpecifics()
    {
        this.enabled = true;
		transform.AddTag( TagSystem.Player );
    }

    public virtual void ExitPossessionSpecifics()
    {
        this.enabled = false;
    }
	#endregion

	#region Taking Damage
	void OnCollisionEnter( Collision collision )
	{
        GBActorStats stats = collision.transform.GetComponent<GBActorStats>();

		if ( stats != null)
		{
			float damageDirection = transform.position.x - collision.transform.position.x;

			float otherBlowbackForce = stats.BlowbackForce;
			if ( damageDirection < 0 )
			{
				GetComponent<Rigidbody>().AddForce( -otherBlowbackForce, otherBlowbackForce, 0f, ForceMode.VelocityChange  );
			}
			else if (damageDirection >= 0)
			{
				GetComponent<Rigidbody>().AddForce( otherBlowbackForce, otherBlowbackForce, 0f , ForceMode.VelocityChange );
			}
			RecieveDamage( 10 );
		}
	}

    public void RecieveDamage(int damage)
    {
        GameMaster.Instance.playerCurrentHealth -= damage;

        GameMaster.Instance.healthSlider.value = ((float)GameMaster.Instance.playerMaxHealth / 100f) * (float)GameMaster.Instance.playerCurrentHealth;

        if (GameMaster.Instance.playerCurrentHealth <= 0)
        {
            GameMaster.Instance.HealthAtZero();
        }
        else
        {
            anim.SetTrigger("Hurt");
            //StartCoroutine(FlashHurtMeshRoutine());
        }
    }

    IEnumerator FlashHurtMeshRoutine()
    {
        AIControllerVariables ACV = GetComponent<AIControllerVariables>();
        ACV.actorMainRenderer.materials = GetComponent<AIControllerVariables>().gotHitMaterials;
        yield return new WaitForSeconds(.5f);
        ACV.actorMainRenderer.materials = GetComponent<AIControllerVariables>().regularMaterials;
	}
	#endregion
}
