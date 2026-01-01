using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Test : MonoBehaviour
{
    public GameObject squarePrefab;
    public Transform spawnPosition;
    public AudioSource audioSource;
    public float lastSpawnTime = 0;
    public bool isSilent;

    float[] spectrumData = new float[256];
    float threshold = 0.1f;
    float bpm = 120;

    void AnalyzeAudio() {
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
    }


    void DetectBeat() {
        int bassIndex = 2; // Tần số bass
        int midIndex = 30; // Tần số trung (melody)
        int highIndex = 60; // Tần số cao (hi-hat, snare)

        float peakThreshold = 0.1f;

        if (spectrumData[bassIndex] > peakThreshold ||
            spectrumData[midIndex] > peakThreshold ||
            spectrumData[highIndex] > peakThreshold) {

            float timeBetweenBeats = 60f / bpm;
            if (Time.time - lastSpawnTime > timeBetweenBeats) {
                SpawnSquare();
                lastSpawnTime = Time.time;
            }
        }
    }
    void DetectSilence() {
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);

        float totalVolume = 0f;
        foreach (float sample in samples) {
            totalVolume += Mathf.Abs(sample); // Lấy tổng biên độ âm thanh
        }

        float averageVolume = totalVolume / samples.Length; // Tính mức âm lượng trung bình
        float silenceThreshold = 0.02f; // Ngưỡng xác định đoạn nghỉ

        isSilent = averageVolume < silenceThreshold;
    }
    bool HasStrongBeat() {
        int bassIndex = 2;
        int midIndex = 30;
        float peakThreshold = 0.15f;

        return spectrumData[bassIndex] > peakThreshold || spectrumData[midIndex] > peakThreshold;
    }
    void AutoGenerateBeats() {
        if (!audioSource.isPlaying || isSilent) return;
        float timeBetweenBeats = 60f / bpm;

        if (Time.time - lastSpawnTime > timeBetweenBeats) {
            SpawnSquare();
            lastSpawnTime = Time.time;
        }
    }
    void CalculateBPM() {
        int peakCount = 0;
        float lastPeakTime = 0;

        foreach (float value in spectrumData) {
            if (value > threshold) {
                float currentTime = Time.time;
                float interval = currentTime - lastPeakTime;

                if (interval > 0) {
                    bpm = 60f / interval; // Chuyển đổi khoảng cách giữa beats thành BPM
                    peakCount++;
                }
                lastPeakTime = currentTime;
            }
        }
    }

    void SpawnSquare() {
        GameObject square = Instantiate(squarePrefab, spawnPosition.position, Quaternion.identity);
        Destroy(square, 3.0f); // Ô vuông tồn tại trong 3 giây
        Debug.Log("A");
    }

    public void Start() {
        audioSource.Play(); // Phát bài hát
        InvokeRepeating("CalculateBPM", 1f, 1f); // Gọi BPM mỗi giây
    }

    public void Update() {
        AnalyzeAudio();
        DetectSilence();
        if (HasStrongBeat()) {
            DetectBeat(); // Sử dụng beat thực từ bài hát
        } else {
            AutoGenerateBeats(); // Tạo nhịp giả nếu bài hát không có beat mạnh
        }
    }
}
