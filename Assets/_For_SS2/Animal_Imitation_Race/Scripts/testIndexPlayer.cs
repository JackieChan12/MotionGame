using nuitrack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class testIndexPlayer : MonoBehaviour
{
    public List<RectTransform> listIndexPlayer = new List<RectTransform>();
    public List<TMP_Text> listText = new List<TMP_Text>();
    public GameObject prefabIndex;
    public Transform transParent;

    [SerializeField] RectTransform baseRect;
    void Start()
    {
        NuitrackManager.SkeletonTracker.SetNumActiveUsers(3);
    }

    // Update is called once per frame
    void Update()
    {
        List<Skeleton> userData = NuitrackManager.SkeletonTracker.GetSkeletonData().Skeletons.ToList();

        if(listIndexPlayer.Count< userData.Count)
        {
            for(int i= listIndexPlayer.Count; i< userData.Count; i++)
            {
                listIndexPlayer.Add(Instantiate(prefabIndex, transParent).GetComponent<RectTransform>());
                listText.Add(listIndexPlayer[i].GetComponentInChildren<TMP_Text>());
                listText[i].text = i.ToString();
                listIndexPlayer[i].gameObject.SetActive(true);
            }
            prefabIndex.SetActive(false);
        }
        else
        {
            for (int i = 0; i < userData.Count; i++)
            {
                listIndexPlayer[i].anchoredPosition = AnchoredPosition(userData[i].GetJoint(JointType.Head).Proj, baseRect.rect, listIndexPlayer[i]);
            }
        }

    }
    public Vector2 AnchoredPosition(nuitrack.Vector3 proj, Rect parentRect, RectTransform rectTransform)
    {
        Vector2 vector2 = new Vector2(Mathf.Clamp01(proj.X), Mathf.Clamp01(1 - proj.Y));
        return Vector2.Scale(vector2 - rectTransform.anchorMin, parentRect.size);
    }
}
