using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMainController : MonoBehaviour
{
    public GameObject[] images; // Mảng chứa 3 ảnh
    public float switchTime = 30f; // Thời gian chuyển đổi ảnh

    private int currentIndex = 0;
    void Start()
    {
        InvokeRepeating("SwitchImage", switchTime, switchTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchImage()
    {
        int nextIndex = (currentIndex + 1) % images.Length;

        // Đặt vị trí ảnh mới ngoài màn hình bên phải
        images[nextIndex].transform.position = new Vector3(Screen.width, images[nextIndex].transform.position.y, images[nextIndex].transform.position.z);
        images[nextIndex].SetActive(true);

        // Tạo hai hiệu ứng trượt cùng lúc
        images[currentIndex].transform.DOMoveX(-Screen.width/2, 1f).SetEase(Ease.InOutQuad);
        images[nextIndex].transform.DOMoveX(Screen.width/2, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            images[currentIndex].SetActive(false);
            currentIndex = nextIndex;
        });
    }
}
