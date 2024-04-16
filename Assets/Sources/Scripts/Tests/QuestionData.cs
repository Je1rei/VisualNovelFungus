using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Linq;

[Serializable]
public class QuestionData
{
    [SerializeField] private string _question;
    [SerializeField] private List<string> _options;
    [SerializeField, Range(0, 2)] private int _correctOptionIndex;
    [SerializeField] private bool _used;

    public string Question => _question;
    public List<string> Options => _options;

    public int CorrectOptionIndex => _correctOptionIndex;
    public bool Used => _used;

    public QuestionData()
    {
        _options = new List<string>();
    }

    public void SetUsed(bool value) => _used = value;

    public void AddOptions(int value)
    {
        for (int i = 0; i < value; i++)
        {
            _options.Add("");
        }
    }
}

