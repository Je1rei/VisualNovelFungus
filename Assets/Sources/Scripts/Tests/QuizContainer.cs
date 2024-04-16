using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using AYellowpaper.SerializedCollections;
using System.Linq;

public partial class QuizContainer : MonoBehaviour
{
    private const string _flowChartTag = "FlowChart";

    private const string _nameTestVar = "nameTest";
    private const string _questionCountVar = "questionCount";
    private const string _questionTag = "question";
    private const string _firstAnswerTag = "firstAnswer";
    private const string _secondAnswerTag = "secondAnswer";
    private const string _thirdAnswerTag = "thirdAnswer";
    private const string _rightAnswerTag = "rightAnswer";

    [SerializeField] private QuestionList _questions;

    private Flowchart _flowchart;

    private QuestionData _currentQuestion;

    private void Start()
    {
        _flowchart = GameObject.FindWithTag(_flowChartTag)?.GetComponent<Flowchart>();

        SetUnusedQuestions();
        ShuffleQuestions();
        SetVariablesFlowchart();
    }

    public void SetQuestionCount()
    {
        SaveInt(_questionCountVar, _questions.Questions.Count(q => !q.Used));
    }

    public void SaveInt(string key, int value)
    {
        int valueSave = value;

        _flowchart.SetIntegerVariable(key, valueSave);
    }

    public void SaveString(string key, string value)
    {
        string valueSave = value;

        _flowchart.SetStringVariable(key, valueSave);
    }

    public void SetVariablesFlowchart()
    {
        if (_questions != null && _questions.Questions.Count > 0)
        {
            _currentQuestion = _questions.Questions.FirstOrDefault(q => !q.Used);

            if (_currentQuestion != null)
            {
                SaveString(_questionTag, _currentQuestion.Question);
                SaveString(_firstAnswerTag, _currentQuestion.Options[0]);
                SaveString(_secondAnswerTag, _currentQuestion.Options[1]);
                SaveString(_thirdAnswerTag, _currentQuestion.Options[2]);
                SaveString(_rightAnswerTag, _currentQuestion.Options[_currentQuestion.CorrectOptionIndex]);

                _currentQuestion.SetUsed(true);
            }
        }
    }

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