using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName = "NewQuestionList", menuName = "Quiz/QuestionList", order = 1)]
public class QuestionList : ScriptableObject
{
    [field: SerializeField] public string Name;
    [field: SerializeField] public int NumberOfQuestionsToAdd = 1;
    [ListDrawerSettings][field: SerializeField] public List<QuestionData> Questions = new List<QuestionData>();

    public QuestionData QuestionData
    {
        get => default;
        set
        {
        }
    }

    [Button(ButtonSizes.Large)]
    [ButtonGroup("TopButtons", -9)]
    private void AddNewQuestion()
    {
        for (int i = 0; i < NumberOfQuestionsToAdd; i++)
        {
            QuestionData newQuestiond = new QuestionData();

            newQuestiond.AddOptions(3);
            Questions.Add(newQuestiond);
        }
    }

    [Button(ButtonSizes.Small)]
    [ButtonGroup("ListOperations", -10)]
    private void ClearQuestions()
    {
#if UNITY_EDITOR
        if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to clear all questions?", "Yes", "No"))
        {
            Questions.Clear();
        }
#endif

    }

    public void ShuffleQuestions()
    {
        Questions = Questions.OrderBy(x => Guid.NewGuid()).ToList();
    }

    [Button(ButtonSizes.Large)]
    [ButtonGroup("TopButtons", -8)]
    public void SetUnusedQuestions()
    {
        foreach (var question in Questions)
        {
            question.SetUsed(false);
        }
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

