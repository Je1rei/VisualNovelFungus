using System.IO;
using UnityEngine;

public class QuizSaver : MonoBehaviour
{
    private const string _quizDataFolder = "QuizQuestions";
    private const string _quizFilePrefix = "Quiz_";

    [SerializeField] private QuestionList[] _quizes;

    private void Start()
    {
        SaveQuizs();
    }

    public void SaveQuizs()
    {
        if (_quizes != null)
        {
            foreach (QuestionList test in _quizes)
            {
                SaveQuiz(test);
            }
        }
    }

    public void SaveQuiz(QuestionList test)
    {
        string jsonData = test.ToJson();
        string filePath = Path.Combine(Application.persistentDataPath, _quizDataFolder);

        Directory.CreateDirectory(filePath);

        string fileName = _quizFilePrefix + test.Name + ".json";
        string fullPath = Path.Combine(filePath, fileName);
        File.WriteAllText(fullPath, jsonData);
    }
}

