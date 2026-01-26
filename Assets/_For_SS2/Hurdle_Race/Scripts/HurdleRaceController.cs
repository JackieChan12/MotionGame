using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleRaceController : MonoBehaviour
{
    public List<Material> materials = new List<Material>();
    public SkinnedMeshRenderer skinnedMeshRenderer;
    void Awake()
    {
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0] = materials[Random.Range(0, materials.Count)];
        skinnedMeshRenderer.materials = mats;
    }

    // Update is called once per frame 
    void Update()
    {
        
    }
}
