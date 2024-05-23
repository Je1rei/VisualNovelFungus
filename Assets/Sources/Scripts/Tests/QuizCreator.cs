using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizCreator : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionCountView;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TMP_InputField _questionInputField;
    [SerializeField, OdinSerialize] private List<TMP_InputField> _answerInputFields;
    [SerializeField] private TMP_Dropdown _correctAnswerDropdown;
    [SerializeField] private GameObject _panelTest;
    [SerializeField] private GameObject _panelSaveTest;

    [SerializeField] private Button _saveQuestionButton;
    [SerializeField] private Button _saveQuizButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _clearCompleteTestButton;

    [SerializeField] private int _maxCountQuestion = 5;
    [SerializeField] private QuizSaver _quizSaver;

    private QuestionList _newQuiz;
    private int _currentQuestionCount = 0;

    private void Start()
    {
        _newQuiz = ScriptableObject.CreateInstance<QuestionList>();

        _panelSaveTest.gameObject.SetActive(false);

        for (int j = 0; j < _answerInputFields.Count; j++)
        {
            int optionIndex = j;
            _answerInputFields[j].onValueChanged.AddListener((value) => UpdateDropdownOptions(optionIndex, value));
        }

        _saveQuestionButton.onClick.AddListener(SaveQuestion);
        _saveQuizButton.onClick.AddListener(SaveQuiz);
        _clearButton.onClick.AddListener(ClearTest);
        _clearCompleteTestButton.onClick.AddListener(ClearTest);
    }

    private void SaveQuestion()
    {
        if (_currentQuestionCount < _maxCountQuestion)
        {
            if (_currentQuestionCount == 0)
            {
                _newQuiz.Name = _nameInputField.text;
            }

            QuestionData newQuestion = new QuestionData();
            newQuestion.SetQuestion(_questionInputField.text);

            List<string> options = new List<string>();

            foreach (var answerInputField in _answerInputFields)
            {
                options.Add(answerInputField.text);
            }

            newQuestion.SetOptions(options);

            newQuestion.SetCorrectOptionIndex(_correctAnswerDropdown.value);

            _newQuiz.Questions.Add(newQuestion);
            _currentQuestionCount++;

            if (_currentQuestionCount > 0)
            {
                _nameInputField.gameObject.SetActive(false);
            }

            if (_currentQuestionCount >= _maxCountQuestion)
            {
                _panelTest.gameObject.SetActive(false);
                _panelSaveTest.gameObject.SetActive(true);
                _currentQuestionCount = 0;
            }

            _questionCountView.text = "Вопрос " + (_currentQuestionCount + 1).ToString();

            Debug.Log($"Question {_currentQuestionCount} saved.");
        }
    }
    private void SaveQuiz()
    {
        _newQuiz.Name = _nameInputField.text;
        _quizSaver.SaveQuiz(_newQuiz);

        ClearInputFields();
        _currentQuestionCount = 0;
        _newQuiz = ScriptableObject.CreateInstance<QuestionList>();
        Debug.Log("Quiz saved.");
    }

    private void ClearTest()
    {
        ClearInputFields();
        _nameInputField.gameObject.SetActive(true);
        _currentQuestionCount = 0;

        _questionCountView.text = "Вопрос " + (_currentQuestionCount + 1).ToString();
        _newQuiz = ScriptableObject.CreateInstance<QuestionList>();
    }

    private void ClearInputFields()
    {
        _nameInputField.text = "";
        _questionInputField.text = "";

        foreach (var answerInputField in _answerInputFields)
        {
            answerInputField.text = "";
        }

        _correctAnswerDropdown.value = 0;
    }

    private void UpdateDropdownOptions(int optionIndex, string value)
    {
        List<string> options = new List<string>();

        foreach (var answerInputField in _answerInputFields)
        {
            options.Add(answerInputField.text);
        }

        _correctAnswerDropdown.ClearOptions();
        _correctAnswerDropdown.AddOptions(options);
    }
}
