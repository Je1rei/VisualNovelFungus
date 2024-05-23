using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Fungus;
using System;
using TMPro;
using UnityEngine.UI;

public class PriceSaver : MonoBehaviour
{
    private const string _testPriceKey = "test_Price_3";
    private const string _priceKey = "Price";
    private const string _priceDataFolder = "Prices";
    private const string _fileName = "pricesActivites.json";

    [SerializeField] private Flowchart _flowchart;

    [SerializeField] private TMP_Dropdown _variableDropdown;
    [SerializeField] private TMP_Text _currentVariableText;
    [SerializeField] private TMP_InputField _newVariableInputField;
    [SerializeField] private Button _saveButtonVariable;

    private List<string> _variableKeys = new List<string>();

    private void Start()
    {
        PopulateDropdown();
        _variableDropdown.onValueChanged.AddListener(ShowCurrentValue);
        _saveButtonVariable.onClick.AddListener(SaveNewValue);
    }

    private void PopulateDropdown()
    {
        if (_flowchart != null)
        {
            _variableKeys.Clear();
            _variableDropdown.ClearOptions();

            foreach (var variable in _flowchart.Variables)
            {
                if (variable.GetType() == typeof(IntegerVariable) && variable.Key.Contains(_priceKey))
                {
                    _variableKeys.Add(variable.Key);
                }
            }

            _variableDropdown.AddOptions(_variableKeys);
            ShowCurrentValue(0);
        }
    }

    private void ShowCurrentValue(int index)
    {
        if (index >= 0 && index < _variableKeys.Count)
        {
            string selectedKey = _variableKeys[index];
            int currentValue = _flowchart.GetIntegerVariable(selectedKey);
            _currentVariableText.text = "" + currentValue;
        }
    }

    private void SaveNewValue()
    {
        string selectedKey = _variableKeys[0];

        if (int.TryParse(_newVariableInputField.text, out int newValue))
        {
            selectedKey = _variableKeys[_variableDropdown.value];
            _flowchart.SetIntegerVariable(selectedKey, newValue);
            SavePrices();
            ClearInputFields();
        }
        else
        {
            _flowchart.SetIntegerVariable(selectedKey, 1);
        }
    }

    [ContextMenu("SavePrice")]
    public void SavePrices()
    {
        if (_flowchart != null)
        {
            PriceData priceData = new PriceData();

            foreach (var variable in _flowchart.Variables)
            {
                if (variable.GetType() == typeof(IntegerVariable) && variable.Key.Contains(_priceKey))
                {
                    int intValue = (int)variable.GetValue();
                    priceData.Add(variable.Key, intValue);
                }
            }

            Save(priceData);
        }
    }


    public void Save(PriceData priceData)
    {
        string filePath = Path.Combine(Application.persistentDataPath, _priceDataFolder);
        string fullPath = Path.Combine(filePath, _fileName);

        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        try
        {
            string jsonData = JsonUtility.ToJson(priceData);

            File.WriteAllText(fullPath, jsonData);

            Debug.Log("Price data saved to: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save price data: " + e.Message);
        }
    }
    private void ClearInputFields()
    {
        _newVariableInputField.text = string.Empty;
        _currentVariableText.text = "";
    }

    [ContextMenu("LoadPrice")]
    public void LoadPrices()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _priceDataFolder, _fileName);

        try
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);

                PriceData priceData = JsonUtility.FromJson<PriceData>(jsonData);

                foreach (var priceActivity in priceData.PriceActivities)
                {
                    foreach (var variable in _flowchart.Variables)
                    {
                        if (variable.Key == priceActivity.Key)
                        {
                            _flowchart.SetIntegerVariable(variable.Key, priceActivity.Value);
                            break;
                        }
                    }
                }

                Debug.Log("Price data loaded from: " + filePath);
            }
            else
            {
                Debug.LogWarning("Price data file does not exist at path: " + filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load price data: " + e.Message);
        }
    }

    public void SaveTestPrices()
    {
        if (_flowchart != null)
        {
            PriceData priceData = new PriceData();
            int intValue = _flowchart.GetIntegerVariable(_testPriceKey);
            priceData.Add(_testPriceKey, intValue);
            Save(priceData);
        }
    }
}
