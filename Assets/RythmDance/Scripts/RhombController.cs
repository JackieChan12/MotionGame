using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class A_Movement
{
    public List<Transform> listPosSquareLeft =new List<Transform>();
    public List<Transform> listPosSquareRight = new List<Transform>();

}

public class RhombController : MonoBehaviour
{
    public int indexPlayer = 0;
    public RectTransform rectTransform;
    public RectTransform rectTransformSpawner;
    public GameObject squarePrefab;
    public Transform spawnPosition;
    public AudioSource audioSource;
    public float lastSpawnTime = 0;
    public bool isSilent;
    public bool startGame;
    public Difficulty difficulty;
    
    List<Square> squareList = new List<Square>();

    float[] spectrumData = new float[256];
    float threshold = 0.1f;
    float bpm = 120;

    float minX, maxX, minY, maxY;
    public AudioController audioController;
    public float point = 0;
    int countSq = 0;

    public RhombController player2;

    public List<A_Movement> listMovements;
    public A_Movement usedMovement = null;
    int indexPosSquare = 0;
    float timeCountDelay;
    float defaultTimeDelay = 0.2f;
    bool haveMovement = false;

    void AnalyzeAudio()
    {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
    }

    [System.Obsolete]
    void DetectBeat()
    {
        int bassIndex = 2; // Tần số bass
        int midIndex = 30; // Tần số trung (melody)
        int highIndex = 60; // Tần số cao (hi-hat, snare)

        float peakThreshold = 0.05f;

        if (spectrumData[bassIndex] > peakThreshold ||
            spectrumData[midIndex] > peakThreshold ||
            spectrumData[highIndex] > peakThreshold)
        {

            float timeBetweenBeats = 60f / bpm;
            if (Time.time - lastSpawnTime > timeBetweenBeats)
            {
                SpawnSquare();
                lastSpawnTime = Time.time;
            }
        }
    }
    void DetectSilence()
    {
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);

        float totalVolume = 0f;
        foreach (float sample in samples)
        {
            totalVolume += Mathf.Abs(sample); // Lấy tổng biên độ âm thanh
        }

        float averageVolume = totalVolume / samples.Length; // Tính mức âm lượng trung bình
        float silenceThreshold = 0.01f; // Ngưỡng xác định đoạn nghỉ

        isSilent = averageVolume < silenceThreshold;
    }
    bool HasStrongBeat()
    {
        int bassIndex = 2;
        int midIndex = 30;
        float peakThreshold = 0.15f;

        return spectrumData[bassIndex] > peakThreshold || spectrumData[midIndex] > peakThreshold;
    }

    [System.Obsolete]
    void AutoGenerateBeats()
    {
        if (!audioSource.isPlaying || isSilent) return;
        float timeBetweenBeats = 60f / bpm;

        if (Time.time - lastSpawnTime > timeBetweenBeats)
        {
            SpawnSquare();
            lastSpawnTime = Time.time;
        }
    }
    void CalculateBPM()
    {
        int peakCount = 0;
        float lastPeakTime = 0;

        foreach (float value in spectrumData)
        {
            if (value > threshold)
            {
                float currentTime = Time.time;
                float interval = currentTime - lastPeakTime;

                if (interval > 0)
                {
                    bpm = 60f / interval; // Chuyển đổi khoảng cách giữa beats thành BPM
                    peakCount++;
                }
                lastPeakTime = currentTime;
            }
        }
    }

    [System.Obsolete]
    void SpawnSquare()
    {
        //if(difficulty == Difficulty.Easy && squareList.Count >= 4)
        //{
        //    return;
        //}
        haveMovement = true;
        if(usedMovement == null ) { usedMovement = listMovements[Random.RandomRange(0, listMovements.Count)]; indexPosSquare = 0; }
        Vector3 sp = Vector3.zero;
        sp.x = usedMovement.listPosSquareLeft[indexPosSquare].localPosition.x;
        sp.y = usedMovement.listPosSquareLeft[indexPosSquare].localPosition.y;
        GameObject square = Instantiate(squarePrefab, spawnPosition.position, squarePrefab.transform.rotation, transform);
        countSq++;
        Square sqControl = square.GetComponent<Square>();
        squareList.Add(sqControl);
        sqControl.choosen = OnSquare;
        sqControl.removeEvent = objectDestroy;
        if (difficulty == Difficulty.Hard) sqControl.defaultTimeEnd = 2;
        if (difficulty == Difficulty.Normal) sqControl.defaultTimeEnd = 3.5f;
        if (difficulty == Difficulty.Easy) sqControl.text.text = countSq.ToString();
        square.transform.localPosition = sp;
        square.SetActive(true);
        //if (player2 != null) player2.SpawnSquare(square.transform.localPosition, countSq);

        if(usedMovement.listPosSquareRight.Count > 0)
        {
            sp.x = usedMovement.listPosSquareRight[indexPosSquare].localPosition.x;
            sp.y = usedMovement.listPosSquareRight[indexPosSquare].localPosition.y;
            GameObject squareR = Instantiate(squarePrefab, spawnPosition.position, squarePrefab.transform.rotation, transform);
            //countSq++;
            Square sqControlR = squareR.GetComponent<Square>();
            squareList.Add(sqControlR);
            sqControlR.choosen = OnSquare;
            sqControlR.removeEvent = objectDestroy;
            if (difficulty == Difficulty.Hard) sqControlR.defaultTimeEnd = 2;
            if (difficulty == Difficulty.Normal) sqControlR.defaultTimeEnd = 3.5f;
            if (difficulty == Difficulty.Easy) sqControlR.text.text = countSq.ToString();
            squareR.transform.localPosition = sp;
            squareR.SetActive(true);
            //if (player2 != null) player2.SpawnSquare(squareR.transform.localPosition,countSq);
        }

        indexPosSquare++;
        if(indexPosSquare >= usedMovement.listPosSquareLeft.Count)
        {
            usedMovement = null;
            indexPosSquare = 0;
            haveMovement = false;
        }
    }

    public void SpawnSquare(Vector3 sp, int c)
    {
        //if (difficulty == Difficulty.Easy && squareList.Count >= ((int)difficulty) + 4)
        //{
        //    return;
        //}
        
        GameObject square = Instantiate(squarePrefab, spawnPosition.position, squarePrefab.transform.rotation, transform);
        
        Square sqControl = square.GetComponent<Square>();
        squareList.Add(sqControl);
        sqControl.choosen = OnSquare;
        sqControl.removeEvent = objectDestroy;
        if (difficulty == Difficulty.Hard) sqControl.defaultTimeEnd = 2;
        if (difficulty == Difficulty.Normal) sqControl.defaultTimeEnd = 3.5f;
        if (difficulty == Difficulty.Easy) sqControl.text.text = c.ToString();
        square.transform.localPosition = sp;
        square.SetActive(true);
    }

    public void Start()
    {
        
        //audioSource.Play(); // Phát bài hát
        //InvokeRepeating("CalculateBPM", 1f, 1f); // Gọi BPM mỗi giây
    }

    public void OnSquare(Square s)
    {
        point += 1;
        audioController.PlaySplat();
        squareList.Remove(s);
        Destroy(s.gameObject);
    }

    [System.Obsolete]
    private void Update()
    {
        CaculateMaxMinUI();
        if (startGame )
        {
            AnalyzeAudio();
            /*if(difficulty != Difficulty.Easy)*/ DetectSilence();

            if ( haveMovement)
            {
                timeCountDelay += Time.deltaTime;
                if (timeCountDelay >= defaultTimeDelay)
                {
                    timeCountDelay = timeCountDelay-defaultTimeDelay;

                    SpawnSquare();
                }
                return;
            }
            if (squareList.Count > 0) return;
            if (HasStrongBeat())
            {
                usedMovement = null;
                DetectBeat(); // Sử dụng beat thực từ bài hát
                timeCountDelay = 0;
            }
            else
            {
                usedMovement = null;
                AutoGenerateBeats(); // Tạo nhịp giả nếu bài hát không có beat mạnh
                timeCountDelay = 0;
            }
        }
    }

    public void CaculateMaxMinUI()
    {
        minX = rectTransform.sizeDelta.x * (rectTransformSpawner.anchorMin.x - 0.5f) + 60;
        maxX = rectTransform.sizeDelta.x * (rectTransformSpawner.anchorMax.x - 0.5f) - 60;
        minY = rectTransform.sizeDelta.y * (rectTransformSpawner.anchorMin.y - 0.5f);
        maxY = rectTransform.sizeDelta.y * (rectTransformSpawner.anchorMax.y - 0.5f);
    }

    public void StartGame()
    {
        countSq = 0;
        usedMovement = null;
        startGame = true;
    }

    public void ResetGame()
    {
        startGame = false;
        RemoveAllSquare();
        usedMovement = null;
    }

    void RemoveAllSquare()
    {
        foreach (var square in squareList)
        {
            if(square) Destroy(square.gameObject);
        }
        squareList.Clear();
    }
    void objectDestroy(Square s)
    {
        Destroy(s.gameObject);
        squareList.Remove(s);
    }
}
