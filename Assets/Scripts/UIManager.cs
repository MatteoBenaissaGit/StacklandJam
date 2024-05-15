using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _description;
    [SerializeField] private TMP_Text _descriptionText;

    private void Awake()
    {
        SetDescription(false, string.Empty);
    }

    public void SetDescription(bool doShow, string text = null)
    {
        _description.SetActive(doShow);
        _descriptionText.text = text;
    }
}
