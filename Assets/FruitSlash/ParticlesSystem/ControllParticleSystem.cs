using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllParticleSystem : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] Sprite sp;
    public bool change = false;
    //private void Update()
    //{
    //    if (change)
    //    {
    //        change = false;
    //        ChangeImageInShape(sp);
    //        particleSystem.Play();
    //    }
    //}
    public void ChangeImageInShape(Sprite texture)
    {
        sp = texture;
        var s = particleSystem.shape;
        s.texture = texture.texture;
        ParticlesPlay();
    }

    public void ParticlesPlay()
    {
        particleSystem.Play();
    }
}
