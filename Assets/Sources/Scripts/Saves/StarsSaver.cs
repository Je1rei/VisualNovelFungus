using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fungus;

public class StarsSaver : MonoBehaviour
{
    [SerializeField] private TMP_InputField starsInputField;
    [SerializeField] private TMP_Text currentStarsText;
    [SerializeField] private Button saveStarsButton;
    [SerializeField] private Flowchart _flowchart;

    private JSONSaver jsonSaver;
    private Container container;

    private void Start()
    {
        container = FindObjectOfType<Container>();
        jsonSaver = FindObjectOfType<JSONSaver>();

        saveStarsButton.onClick.AddListener(SaveStars);
        DisplayCurrentStars();
    }

    private void DisplayCurrentStars()
    {
        if (container != null)
        {
            int currentStars = container.CoinCollected;
            currentStarsText.text = currentStars.ToString();
        }
    }

    private int GetCurrentStars()
    {
        if (_flowchart != null)
        {
            int currentStars = _flowchart.GetIntegerVariable(ConstantsSavers._coinCollected);
            return currentStars;
        }
        else
        {
            Debug.LogError("Flowchart is not assigned.");
            return 0;
        }
    }

    private void SaveStars()
    {
        if (container != null && int.TryParse(starsInputField.text, out int newStars))
        {
            int currentStars = container.CoinCollected;
            int difference = newStars - currentStars;

            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    container.IncreaseCoinCollected();
                }
            }
            else if (difference < 0)
            {
                for (int i = 0; i < Mathf.Abs(difference); i++)
                {
                    container.DecreaseCoinCollected();
                }
            }

            jsonSaver.SaveInt(ConstantsSavers._coinCollected, container.CoinCollected);
            DisplayCurrentStars();
        }
        else
        {
            Debug.LogError("Invalid input value.");
        }
    }
}