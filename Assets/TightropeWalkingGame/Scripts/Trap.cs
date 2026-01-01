using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Obstacle
{
    public string animDeath;
    public string animSpawn;
    public Animator anim;
    void Start()
    {
        if (anim) anim.Play(animSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Impacted()
    {
        base.Impacted();
        if (animDeath == "" || animDeath == null || animDeath ==".") return;
        if (anim) anim.Play(animDeath);
        StartCoroutine(BackAnim());
    }

    IEnumerator BackAnim()
    {
        yield return new WaitForSeconds(1.5f);
        if (anim) anim.Play(animSpawn);
    }
    
}
