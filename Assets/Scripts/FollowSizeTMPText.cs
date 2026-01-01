using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FollowSizeTMPText : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text textTarget;

    // Update is called once per frame
    void Update()
    {
        text.fontSize = textTarget.fontSize;
    }
}
