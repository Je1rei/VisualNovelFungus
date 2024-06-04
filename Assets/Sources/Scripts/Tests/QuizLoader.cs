using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Fungus;
using UnityEngine.UI;
using System.Linq;

public class QuizLoader : MonoBehaviour
{
    private const string _quizDataFolder = "QuizQuestions";
    private const string _activeQuizDataFolder = "ActiveQuizQuestions";
    private const string _quizFilePrefix = "Quiz_";
    private const string _standardQuizzesFolder = "ScriptableObjects/Quizes";

    [SerializeField] private TMP_Dropdown _quizDropdown;
    [SerializeField] private TMP_Dropdown _questionDropdown;
    [SerializeField] private TMP_Dropdown _activeQuizzesDropdown;

    [SerializeField] private TMP_Text[] _answerTexts;
    [SerializeField] private TMP_Text _correctAnswerText;

    [SerializeField] private TMP_Text _limitTests;
    [SerializeField] private Button _addToActiveQuizzesButton;
    [SerializeField] private Button _removeFromActiveQuizzesButton;
    [SerializeField] private Button _clearActiveQuizzesButton;

    [SerializeField] private List<string> _excludedQuizzes;  // List to hold excluded quiz names

    private List<QuestionList> _quizList = new List<QuestionList>();
    private List<QuestionList> _activeQuizzes = new List<QuestionList>();

    private string _activeQuizDirectoryPath;
    private string _quizDirectoryPath;

    private int _selectedQuizIndex = -1;
    private int _selectedQuestionIndex = -1;

    private void Awake()
    {
        _quizDirectoryPath = Path.Combine(Application.persistentDataPath, _quizDataFolder);
        _activeQuizDirectoryPath = Path.Combine(Application.persistentDataPath, _activeQuizDataFolder);

        if (!Directory.Exists(_activeQuizDirectoryPath))
        {
            Directory.CreateDirectory(_activeQuizDirectoryPath);
        }

        if (!Directory.Exists(_quizDirectoryPath))
        {
            Directory.CreateDirectory(_quizDirectoryPath);
        }

        CopyStandardQuizzesIfNeeded();
    }

    private void Start()
    {
        LoadQuizzesFromJSON();
        LoadQuizzesFromActiveFolder();
        UpdateQuizDropdown();
        UpdateActiveQuizzesDropdown();

        _quizDropdown.onValueChanged.AddListener(SelectQuiz);
        _questionDropdown.onValueChanged.AddListener(SelectQuestion);

        _addToActiveQuizzesButton.onClick.AddListener(AddToActiveQuizzes);
        _removeFromActiveQuizzesButton.onClick.AddListener(RemoveFromActiveQuizzes);

        _clearActiveQuizzesButton.onClick.AddListener(ClearActiveQuizzes);

        if (_activeQuizzes.Count <= 5)
        {
            _limitTests.gameObject.SetActive(false);
            _addToActiveQuizzesButton.gameObject.SetActive(true);
            return;
        }
    }

    public void LoadQuizzesFromJSON()
    {
        _quizList.Clear();

        if (!Directory.Exists(_quizDirectoryPath))
        {
            Debug.LogError("Directory not found: " + _quizDirectoryPath);
            return;
        }

        string[] quizFiles = Directory.GetFiles(_quizDirectoryPath, "*.json");

        foreach (string filePath in quizFiles)
        {
            string json = File.ReadAllText(filePath);
            QuestionList quiz = ScriptableObject.CreateInstance<QuestionList>();
            JsonUtility.FromJsonOverwrite(json, quiz);

            if (!_quizList.Exists(q => q.Name == quiz.Name) && !_excludedQuizzes.Contains(quiz.Name))
            {
                _quizList.Add(quiz);
            }
        }

        UpdateQuizDropdown();
    }

    private void CopyStandardQuizzesIfNeeded()
    {
        string[] userQuizFiles = Directory.GetFiles(_quizDirectoryPath, "*.json");

        if (userQuizFiles.Length == 0)
        {
            string standardQuizzesPath = Path.Combine(Application.dataPath, _standardQuizzesFolder);
            string[] standardQuizFiles = Directory.GetFiles(standardQuizzesPath, "*.json");

            foreach (string standardQuizFile in standardQuizFiles)
            {
                string fileName = Path.GetFileName(standardQuizFile);
                string destinationPath = Path.Combine(_quizDirectoryPath, fileName);
                File.Copy(standardQuizFile, destinationPath, true);
            }

            Debug.Log("Standard quizzes copied to user data folder.");
        }
    }

    private void LoadQuizzesFromActiveFolder()
    {
        _activeQuizzes.Clear();

        if (!Directory.Exists(_activeQuizDirectoryPath))
        {
            Debug.LogError("Directory not found: " + _activeQuizDirectoryPath);
            return;
        }

        string[] quizFiles = Directory.GetFiles(_activeQuizDirectoryPath, "*.json");

        foreach (string filePath in quizFiles)
        {
            string json = File.ReadAllText(filePath);
            QuestionList quiz = ScriptableObject.CreateInstance<QuestionList>();
            JsonUtility.FromJsonOverwrite(json, quiz);

            if (!_activeQuizzes.Exists(q => q.Name == quiz.Name) && !_excludedQuizzes.Contains(quiz.Name))
            {
                _activeQuizzes.Add(quiz);
            }
        }

        UpdateActiveQuizzesDropdown();
    }

    private void UpdateQuizDropdown()
    {
        _quizDropdown.ClearOptions();

        List<string> quizNames = new List<string>();

        foreach (var quiz in _quizList)
        {
            if (!_excludedQuizzes.Contains(quiz.Name))
            {
                quizNames.Add(quiz.Name);
            }
        }

        _quizDropdown.AddOptions(quizNames);
    }

    public void SelectQuiz(int index)
    {
        if (index >= 0 && index < _quizList.Count)
        {
            _selectedQuizIndex = index;
            _selectedQuestionIndex = -1;
            UpdateQuestionDropdown();
        }
    }

    private void UpdateQuestionDropdown()
    {
        _questionDropdown.ClearOptions();
        List<string> questionTexts = new List<string>();

        if (_selectedQuizIndex >= 0 && _selectedQuizIndex < _quizList.Count)
        {
            QuestionList selectedQuiz = _quizList[_selectedQuizIndex];

            foreach (var question in selectedQuiz.Questions)
            {
                questionTexts.Add(question.Question);
            }

            _questionDropdown.AddOptions(questionTexts);

            if (questionTexts.Count > 0)
            {
                _questionDropdown.value = 0;
                SelectQuestion(0);
            }
        }
    }

    public void SelectQuestion(int index)
    {
        if (_selectedQuizIndex >= 0 && _selectedQuizIndex < _quizList.Count)
        {
            QuestionList selectedQuiz = _quizList[_selectedQuizIndex];

            if (index >= 0 && index < selectedQuiz.Questions.Count)
            {
                _selectedQuestionIndex = index;
                QuestionData selectedQuestion = selectedQuiz.Questions[index];
                DisplayQuestionDetails(selectedQuestion);
            }
        }
    }

    private void DisplayQuestionDetails(QuestionData question)
    {
        for (int i = 0; i < _answerTexts.Length; i++)
        {
            if (i < question.Options.Count)
                _answerTexts[i].text = question.Options[i];
            else
                _answerTexts[i].text = "";
        }

        _correctAnswerText.text = "Верный ответ - " + question.Options[question.CorrectOptionIndex];
    }

    public void DeleteSelectedQuiz()
    {
        if (_selectedQuizIndex >= 0 && _selectedQuizIndex < _quizList.Count)
        {
            string quizFilePath = Path.Combine(_quizDirectoryPath, _quizFilePrefix + _quizList[_selectedQuizIndex].Name + ".json");

            if (File.Exists(quizFilePath))
            {
                File.Delete(quizFilePath);
            }

            _quizList.RemoveAt(_selectedQuizIndex);
            _selectedQuizIndex = -1;
            _selectedQuestionIndex = -1;

            UpdateQuizDropdown();
            _questionDropdown.ClearOptions();
            ClearQuestionDetails();
        }

        if (_activeQuizzes.Count <= 5)
        {
            _limitTests.gameObject.SetActive(false);
            _addToActiveQuizzesButton.gameObject.SetActive(true);
            return;
        }
    }

    public void SetActiveQuizzes(List<QuestionList> activeQuizzes)
    {
        _activeQuizzes = activeQuizzes;
        UpdateActiveQuizzesDropdown();
    }

    private void ClearQuestionDetails()
    {
        foreach (var answerText in _answerTexts)
        {
            answerText.text = "";
        }

        _correctAnswerText.text = "";
    }

    private void AddToActiveQuizzes()
    {
        if (_activeQuizzes.Count >= 5)
        {
            _limitTests.gameObject.SetActive(true);
            _addToActiveQuizzesButton.gameObject.SetActive(false);
            Debug.Log("Максимальное количество тестов достигнуто");
            return;
        }

        int selectedQuizIndex = _quizDropdown.value;

        if (selectedQuizIndex >= 0 && selectedQuizIndex < _quizList.Count)
        {
            QuestionList selectedQuiz = _quizList[selectedQuizIndex];

            if (!_activeQuizzes.Any(q => q.Name == selectedQuiz.Name) && !_excludedQuizzes.Contains(selectedQuiz.Name))
            {
                _quizList.RemoveAt(selectedQuizIndex);

                _activeQuizzes.Add(selectedQuiz);
                UpdateActiveQuizzesDropdown();

                MoveQuizToActiveFolder(selectedQuiz);
                UpdateQuizDropdown();
            }
            else
            {
                Debug.LogWarning("Выбранный тест уже добавлен в активные тесты или находится в исключенных.");
            }
        }
    }

    private void RemoveFromActiveQuizzes()
    {
        int selectedQuizIndex = _activeQuizzesDropdown.value;

        if (selectedQuizIndex >= 0 && selectedQuizIndex < _activeQuizzes.Count)
        {
            QuestionList selectedQuiz = _activeQuizzes[selectedQuizIndex];
            _activeQuizzes.RemoveAt(selectedQuizIndex);
            UpdateActiveQuizzesDropdown();

            MoveQuizToMainFolder(selectedQuiz);

            if (!_quizList.Contains(selectedQuiz) && !_excludedQuizzes.Contains(selectedQuiz.Name))
            {
                _quizList.Add(selectedQuiz);
                UpdateQuizDropdown();
            }

            UpdateActiveQuizzesDropdown();
            UpdateQuizDropdown();
        }

        if (_activeQuizzes.Count <= 5)
        {
            _limitTests.gameObject.SetActive(false);
            _addToActiveQuizzesButton.gameObject.SetActive(true);
            return;
        }
    }

    private void MoveQuizToActiveFolder(QuestionList quiz)
    {
        string quizFilePath = Path.Combine(_quizDirectoryPath, _quizFilePrefix + quiz.Name + ".json");
        string activeQuizFilePath = Path.Combine(Application.persistentDataPath, _activeQuizDataFolder, _quizFilePrefix + quiz.Name + ".json");

        if (File.Exists(activeQuizFilePath))
        {
            File.Delete(activeQuizFilePath);
        }

        File.Move(quizFilePath, activeQuizFilePath);
    }

    private void MoveQuizToMainFolder(QuestionList quiz)
    {
        string activeQuizFilePath = Path.Combine(Application.persistentDataPath, _activeQuizDataFolder, _quizFilePrefix + quiz.Name + ".json");
        string quizFilePath = Path.Combine(_quizDirectoryPath, _quizFilePrefix + quiz.Name + ".json");

        if (File.Exists(quizFilePath))
        {
            File.Delete(activeQuizFilePath);
            return;
        }

        File.Move(activeQuizFilePath, quizFilePath);
    }

    public void ClearActiveQuizzes()
    {
        foreach (var quiz in _activeQuizzes)
        {
            MoveQuizToMainFolder(quiz);
            _quizList.Add(quiz);
        }

        _activeQuizzes.Clear();

        _activeQuizzes = _activeQuizzes.Distinct().ToList();

        UpdateActiveQuizzesDropdown();
        UpdateQuizDropdown();
        UpdateQuestionDropdown();
    }

    private void UpdateActiveQuizzesDropdown()
    {
        _activeQuizzesDropdown.ClearOptions();
        List<string> quizNames = new List<string>();

        foreach (var quiz in _activeQuizzes)
        {
            if (!_excludedQuizzes.Contains(quiz.Name))
            {
                quizNames.Add(quiz.Name);
            }
        }

        _activeQuizzesDropdown.AddOptions(quizNames);

        if (_activeQuizzes.Count <= 5)
        {
            _limitTests.gameObject.SetActive(false);
            _addToActiveQuizzesButton.gameObject.SetActive(true);
        }
    }

    public List<QuestionList> GetActiveQuizzes()
    {
        return _activeQuizzes;
    }
}
