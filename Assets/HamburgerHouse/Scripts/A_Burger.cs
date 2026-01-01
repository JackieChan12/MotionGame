using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A_Burger : MonoBehaviour
{
    public List<Sprite> ingredients = new List<Sprite>();
    public List<int> ints = new List<int>();
    public List<bool> choosenIngredients = new List<bool>();
    public List<Image> images = new List<Image>();
    public void GetImage()
    {
        for (int i= transform.childCount-1; i>=0; i--)
        {
            images.Add(transform.GetChild(i).GetComponent<Image>());
        }
        foreach(Image i in images)
        {
            i.enabled = false;
        }
    }

}
