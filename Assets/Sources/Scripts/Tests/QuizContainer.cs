using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using AYellowpaper.SerializedCollections;
using System.Linq;
using System.IO;

public partial class QuizContainer : MonoBehaviour
{
    private const string _activeQuizDataFolder = "ActiveQuizQuestions";
    private const string _quizDataFolder = "QuizQuestions";
    private const string _flowChartTag = "FlowChart";

    private const string _nameTestVar = "nameTest";
    private const string _questionCountVar = "questionCount";
    private const string _questionTag = "question";
    private const string _firstAnswerTag = "firstAnswer";
    private const string _secondAnswerTag = "secondAnswer";
    private const string _thirdAnswerTag = "thirdAnswer";
    private const string _rightAnswerTag = "rightAnswer";
    private const string _userAnswerTag = "choisedAnswer";

    private Flowchart _flowchart;

    private QuestionList _questions;
    private string _quizDirectoryPath;

    private QuestionData _currentQuestion;
    private int _currentQuestionIndex;

    private void Awake()
    {
        _quizDirectoryPath = Path.Combine(Application.persistentDataPath, _activeQuizDataFolder);
        LoadQuizFromJSON();
    }

    private void Start()
    {
        _flowchart = GameObject.FindWithTag(_flowChartTag)?.GetComponent<Flowchart>();

        if (_flowchart == null)
        {
            Debug.LogError("Flowchart not found with tag: " + _flowChartTag);
            return;
        }

        SetUnusedQuestions();
        ShuffleQuestions();
        _currentQuestionIndex = 0;
        SetNextQuestion();

        SetHalfCollected();
    }

    public void LoadQuizFromJSON()
    {
        if (!Directory.Exists(_quizDirectoryPath))
        {
            Debug.LogError("Directory not found: " + _quizDirectoryPath);
            return;
        }

        string[] quizFiles = Directory.GetFiles(_quizDirectoryPath, "*.json");

         if (quizFiles.Length > 0)
        {
            string randomQuizFile = quizFiles[UnityEngine.Random.Range(0, quizFiles.Length)]; // Случайный выбор файла
            string json = File.ReadAllText(randomQuizFile);
            _questions = ScriptableObject.CreateInstance<QuestionList>();
            JsonUtility.FromJsonOverwrite(json, _questions);

            if (_questions != null && _questions.Questions.Count > 0)
            {
                Debug.Log("Quiz loaded: " + _questions.Name);
            }
            else
            {
                Debug.LogError("Quiz is null or has no questions.");
            }
        }
        else
        {
            Debug.LogError("No quiz files found in directory: " + _quizDirectoryPath);
        }
    }

    public void SetNextQuestion()
    {
        if (_questions != null && _questions.Questions.Count > 0 && _currentQuestionIndex < _questions.Questions.Count)
        {
            _currentQuestion = _questions.Questions[_currentQuestionIndex];

            SaveString(_questionTag, _currentQuestion.Question);
            SaveString(_firstAnswerTag, _currentQuestion.Options[0]);
            SaveString(_secondAnswerTag, _currentQuestion.Options[1]);
            SaveString(_thirdAnswerTag, _currentQuestion.Options[2]);
            SaveString(_rightAnswerTag, _currentQuestion.Options[_currentQuestion.CorrectOptionIndex]);

            _currentQuestion.SetUsed(true);
            _currentQuestionIndex++;
        }
        else
        {
            Debug.LogWarning("No more questions available or question list is null.");
        }
    }

    public void CheckAnswer()
    {
        string userAnswer = _flowchart.GetStringVariable(_userAnswerTag);
        string correctAnswer = _currentQuestion.Options[_currentQuestion.CorrectOptionIndex];

        if (userAnswer == correctAnswer)
        {
            Debug.Log("Correct Answer");

            // Получаем текущее значение переменной totalCollected
            int totalCollected = _flowchart.GetIntegerVariable("totalCollected");

            // Получаем значение, которое нужно добавить к totalCollected
            int valueEach = _flowchart.GetIntegerVariable("valueEach");

            // Прибавляем значение каждого правильного ответа
            totalCollected += valueEach;

            // Устанавливаем новое значение переменной totalCollected
            _flowchart.SetIntegerVariable("totalCollected", totalCollected);
        }
        else
        {
            Debug.Log("Incorrect Answer");
            // Perform action for incorrect answer
        }

        SetNextQuestion();
    }

    public void SetHalfCollected()
    {
        if (_questions != null)
        {
            // Получаем общее количество вопросов в тесте
            int totalQuestions = _questions.Questions.Count;

            // Вычисляем 3/4 от общего количества вопросов
            int halfCollectedValue = totalQuestions * 3 / 4;

            // Устанавливаем значение переменной halfcollected в Flowchart
            _flowchart.SetIntegerVariable("halfCollect", halfCollectedValue);
            Debug.Log("Set halfcollected variable in Flowchart: " + halfCollectedValue);
        }
        else
        {
            Debug.LogWarning("Questions list is null.");
        }
    }

    public void SetQuestionCount()
    {
        SaveInt(_questionCountVar, _questions.Questions.Count(q => !q.Used));
    }

    public void SaveInt(string key, int value)
    {
        int valueSave = value;

        _flowchart.SetIntegerVariable(key, valueSave);
        Debug.Log("Set Flowchart int variable: " + key + " = " + value);
    }

    public void SaveString(string key, string value)
    {
        string valueSave = value;

        _flowchart.SetStringVariable(key, valueSave);
        Debug.Log("Set Flowchart string variable: " + key + " = " + value);

    }

    //public void SetVariablesFlowchart()
    //{
    //    if (_questions != null && _questions.Questions.Count > 0)
    //    {
    //        _currentQuestion = _questions.Questions.FirstOrDefault(q => !q.Used);

    //        if (_currentQuestion != null)
    //        {
    //            SaveString(_questionTag, _currentQuestion.Question);
    //            SaveString(_firstAnswerTag, _currentQuestion.Options[0]);
    //            SaveString(_secondAnswerTag, _currentQuestion.Options[1]);
    //            SaveString(_thirdAnswerTag, _currentQuestion.Options[2]);
    //            SaveString(_rightAnswerTag, _currentQuestion.Options[_currentQuestion.CorrectOptionIndex]);

    //            _currentQuestion.SetUsed(true);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("No unused questions found.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Questions list is null or empty.");
    //    }
    //}


    public void ShuffleQuestions()
    {
        if (_questions != null)
            _questions.ShuffleQuestions();
    }

    public void SetNameTest()
    {
        SaveString(_nameTestVar, _questions.Name);
    }

    public void SetUnusedQuestions()
    {
        _questions.SetUnusedQuestions();
    }
}