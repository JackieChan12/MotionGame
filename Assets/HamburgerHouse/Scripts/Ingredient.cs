using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    public int stt;
    public Image imageIngredient;
    public Image imageChoosen;
    public delegate void choosenEvent(Ingredient i);
    public choosenEvent choosen;
    public bool canChoose = false;
    
    void OnEnable()
    {
        imageChoosen.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canChoose) choosen?.Invoke(this);
    }

    public void HideIngredient()
    {
        imageChoosen.gameObject?.SetActive(true);
    }

    public void  ChangeImage(Sprite sprite)
    {
        imageIngredient.sprite = sprite;
    }
}
