using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

[Serializable]
public class QuestionData
{
    [SerializeField] public string Question;
    [SerializeField] public List<string> Options;
    [SerializeField, Range(0, 2)] public int CorrectOptionIndex;
    [SerializeField] public bool Used;


    public QuestionData()
    {
        Options = new List<string>();
    }

    public void SetUsed(bool value) => Used = value;

    public void SetQuestion(string question)
    {
        Question = question;
    }

    public void SetOptions(List<string> options)
    {
        Options = options;
    }

    public void SetCorrectOptionIndex(int index)
    {
        CorrectOptionIndex = index;
    }

    public void AddOptions(int value)
    {
        for (int i = 0; i < value; i++)
        {
            Options.Add("");
        }
    }
}

