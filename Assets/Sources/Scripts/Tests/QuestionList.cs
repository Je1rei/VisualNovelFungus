using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName = "NewQuestionList", menuName = "Quiz/QuestionList", order = 1)]
public class QuestionList : ScriptableObject
{
    [field: SerializeField] private string _nameTest;
    [field: SerializeField] private int _numberOfQuestionsToAdd = 1;
    [ListDrawerSettings][field: SerializeField] private List<QuestionData> _questions = new List<QuestionData>();
    public List<QuestionData> Questions => _questions;
    public string Name => _nameTest;

    [Button(ButtonSizes.Large)]
    [ButtonGroup("TopButtons", -9)]
    private void AddNewQuestion()
    {
        for (int i = 0; i < _numberOfQuestionsToAdd; i++)
        {
            QuestionData newQuestiond = new QuestionData();

            newQuestiond.AddOptions(3);
            _questions.Add(newQuestiond);
        }
    }

    [Button(ButtonSizes.Small)]
    [ButtonGroup("ListOperations", -10)]
    private void ClearQuestions()
    {
#if UNITY_EDITOR
        if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to clear all questions?", "Yes", "No"))
        {
            _questions.Clear();
        }
#endif

    }

    public void ShuffleQuestions()
    {
        _questions = _questions.OrderBy(x => Guid.NewGuid()).ToList();
    }

    [Button(ButtonSizes.Large)]
    [ButtonGroup("TopButtons", -8)]
    public void SetUnusedQuestions()
    {
        foreach (var question in _questions)
        {
            question.SetUsed(false);
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

