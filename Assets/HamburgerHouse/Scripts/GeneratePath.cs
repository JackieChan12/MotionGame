using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SocialPlatforms;
public class Gena : MonoBehaviour
{
    public HamburgerHouse main;

    public Transform[] waypoints; // Các điểm di chuyển
    public GameObject objectToSpawn; // Đối tượng để sinh
    public float spawnInterval = 3f; // Khoảng thời gian giữa các lần sinh đối tượng
    public float moveDuration = 5f; // Thời gian di chuyển qua mỗi đoạn
    public float timeScale = 2f;

    private bool isPaused = false; 
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Sequence> listSequenceObject = new List<Sequence>();
    private bool initialRun = true;

    public List<Sprite> sprites = new List<Sprite>();
    public List<int> listIndexSprite = new List<int>();
    int index = 0;
    Ingredient ing;
    public List<Ingredient> ingredients = new List<Ingredient>();

    public RectTransform rect;
    float ratio;

    void Start() 
    {
        ratio = rect.sizeDelta.x / 1920;
        // Bắt đầu sinh đối tượng định kỳ
        //InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval); 
    }

    public void StartGame(bool first = true)
    {
        if (first) InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
        else ResumeGroupMovement();
    }
    
    void SpawnObject() 
    { 
        GameObject spawnedObject = Instantiate(objectToSpawn, waypoints[0].position, Quaternion.identity,transform);
        ing = spawnedObject.GetComponent<Ingredient>();

        ing.stt = listIndexSprite[index];
        ing.ChangeImage(main.listImageIngredients[ing.stt]);
        ing.canChoose = false;
        ing.choosen = CheckImageChoose;

        ingredients.Add(ing);
        index++;
        if (index >= listIndexSprite.Count)
        {
            index = 0;
        }
        spawnedObjects.Add(spawnedObject);
        MoveObjectAlongPath(spawnedObject);
    } 

    void CheckImageChoose(Ingredient ingred)
    {
        main.CheckIngredients(ingred);
    }

    void MoveObjectAlongPath(GameObject obj) 
    { 
        Sequence sequence = DOTween.Sequence();
        listSequenceObject.Add(sequence);
        for (int i = 1; i < waypoints.Length; i++) 
        { 
            int targetIndex = i;
            float targetDistance = Vector3.Distance(waypoints[i].position, waypoints[i - 1].position);
            if (initialRun && targetIndex == waypoints.Length - 2) // Dừng tại điểm thứ 2 từ dưới lên
            {
                initialRun = false;
                sequence.Append(obj.transform.DOMove(waypoints[targetIndex].position, /*moveDuration*/targetDistance/(moveDuration*ratio)).SetEase(Ease.Linear).OnComplete(() => PauseGroupMovement()));
            }
            else 
            { 
                sequence.Append(obj.transform.DOMove(waypoints[targetIndex].position, /*moveDuration*/targetDistance / (moveDuration * ratio)).SetEase(Ease.Linear));
            } 
        } 
        sequence.OnComplete(() => 
        { 
            spawnedObjects.Remove(obj); Destroy(obj); listSequenceObject.Remove(sequence);// Xóa đối tượng khi hoàn tất di chuyển
        }); 
        sequence.Play(); 
    } 
    void PauseGroupMovement() 
    {
        if (!isPaused) 
        { 
            isPaused = true; 
            foreach (Sequence s in listSequenceObject)
            {
                s.Pause();
            }

            foreach (Ingredient i in ingredients)
            {
                i.canChoose = true;
            }
            CancelInvoke(nameof(SpawnObject)); // Tạm dừng sinh đối tượng mới
        } 
    } 
    void ResumeGroupMovement() 
    {
        Debug.LogWarning("JoinResume");
        if (isPaused) 
        {
            isPaused = false;
            foreach (Sequence s in listSequenceObject)
            {
                s.Play();
            }
            foreach(Ingredient i in ingredients)
            {
                i.canChoose = false;
            }
            ingredients.Clear();
            listSequenceObject.Clear();
            InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval); // Tiếp tục sinh đối tượng mới
            initialRun = true; // Chuyển sang lượt mới
        } 
    } 

    public void EndTurnPlayer()
    {
        if (isPaused)
        {
            Debug.LogWarning("JoinEndTurn");
            //isPaused = false;
            foreach (Sequence s in listSequenceObject)
            {
                s.Play();
            }
            foreach (Ingredient i in ingredients)
            {
                i.canChoose = false;
            }
            ingredients.Clear();
            listSequenceObject.Clear();
            initialRun = true;
        }
    }
    void Update() 
    { 
        //if (isPaused && Input.GetKeyDown(KeyCode.Space)) 
        //{ 
        //    ResumeGroupMovement();
        //} 
    }
}

