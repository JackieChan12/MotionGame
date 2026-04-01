using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Command : MonoBehaviour, ISingleton
{
    public static Command Instance { get; private set; }

    public string CommmandTutorialHurdleRace { get; private set; } = "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>RUN IN PLACE</color></b> to move\n<b><color=#FF3333>JUMP</color></b> to clear hurdles</size></align>";

    public string[] CommandTutorialAnimalRace { get; private set; } = new string[] 
    {
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>RUN IN PLACE</color></b> to move forward</size></align>",
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>FLAP YOUR ARMS</color></b> up and down</size></align>",
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>LEAN LEFT & RIGHT</color></b> to move</size></align>",
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>SWIM FORWARD</color></b> with your arms</size></align>",
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>SPREAD ARMS & LEAN</color></b> side to side</size></align>",
        "<align=center><size=150%><b><color=#FFFF00>ARE YOU READY?</color></b></size>\n\n<size=110%><b><color=#33FF33>HANDS UP & RUN</color></b> in place</size></align>"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
