using UnityEngine;
using System.Collections;

public class BasePossessableAI : BaseAI {

    [Tooltip("Max health for actor")]
    public int maxHealth;
    public int currentHealth;

    protected void BaseStart()
    {
        currentHealth = maxHealth;
    }

    public virtual void GettingPossessed()
    {
        this.enabled = false;
    }

    public virtual void StopGettingPossessed()
    {
        this.enabled = true;
    }

    public void RecieveDamage(int damage)
    {

    }

    public virtual void HealthAtZero()
    {
        GetComponent<BaseController>().ExitPossessionSpecifics();
    }
}
