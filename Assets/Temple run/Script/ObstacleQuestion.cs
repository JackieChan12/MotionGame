using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObstacleQuestion : Obstacle
{
    [Header("Question")]
    public TMP_Text textQuiz;

    [Header("Answer 1")]
    public TMP_Text textAnswer1;
    public GameObject objAnswer1;

    [Header("Question")]
    public TMP_Text textAnswer2;
    public GameObject objAnswer2;

    [Header("Question")]
    public TMP_Text textAnswer3;
    public GameObject objAnswer3;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        GenerateQuiz();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void GenerateQuiz()
    {
        int num1 = UnityEngine.Random.Range(1, 99); // Số ngẫu nhiên từ 1 đến 20
        int num2 = UnityEngine.Random.Range(1, 99); // Số ngẫu nhiên từ 1 đến 20
        int correctAnswer = num1 + num2;

        int wrongAnswer1 = correctAnswer + UnityEngine.Random.Range(1, 5);
        int wrongAnswer2 = correctAnswer - UnityEngine.Random.Range(1, 5);

        // Đảm bảo các đáp án sai không trùng với đáp án đúng
        if (wrongAnswer1 == correctAnswer) wrongAnswer1 += 1;
        if (wrongAnswer2 == correctAnswer) wrongAnswer2 -= 1;

        // Đảm bảo các đáp án sai không trùng lẫn nhau
        if (wrongAnswer1 == wrongAnswer2)
        {
            wrongAnswer2 += UnityEngine.Random.Range(1, 3);
        }

        // Trộn các đáp án
        int[] answers = new int[] { correctAnswer, wrongAnswer1, wrongAnswer2 };
        ShuffleArray(answers);

        textQuiz.text = $"{num1} + {num2} = ?";

        textAnswer1.text = answers[0].ToString();
        if (answers[0] == correctAnswer) objAnswer1.name = objAnswer1.name + "True";

        textAnswer2.text = answers[1].ToString();
        if (answers[1] == correctAnswer) objAnswer2.name = objAnswer2.name + "True";

        textAnswer3.text = answers[2].ToString();
        if (answers[2] == correctAnswer) objAnswer3.name = objAnswer3.name + "True";


        Console.WriteLine($"Câu hỏi: {num1} + {num2} = ?");
        Console.WriteLine($"A. {answers[0]}");
        Console.WriteLine($"B. {answers[1]}");
        Console.WriteLine($"C. {answers[2]}");
    }

    static void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
