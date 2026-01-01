using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardNumberController : MonoBehaviour
{
    public int number = 0;
    public List<MeshRenderer> renderers;
    public Material materialDefault;
    public Material materialChoosen;
    public ParticleSystem particle;
    private void Start()
    {
        materialDefault = renderers[0].material;
        RandomNumber();
    }
    public void RandomNumber()
    {
        renderers[number].material= materialDefault;
        int newNumber = 0;
        randAgain: 
        newNumber = Random.Range(0, 4);
        if(newNumber == number)
        {
            goto randAgain;
        }
        number = newNumber;
        renderers[number].material = materialChoosen;
        particle.transform.localPosition = renderers[number].transform.localPosition + new Vector3(-.3f,0,0);
    }


}
