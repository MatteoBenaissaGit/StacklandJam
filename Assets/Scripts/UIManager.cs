using System;
using System.Collections;
using System.Collections.Generic;
using Data.Cards;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _descriptionGroup;
    [SerializeField] private TMP_Text _descriptionTitleText;
    [SerializeField] private TMP_Text _descriptionText;

    private void Awake()
    {
        _descriptionGroup.alpha = 0;
        _descriptionGroup.interactable = false;
        _descriptionGroup.blocksRaycasts = false;
    }

    public void SetDescription(bool doShow, CardData data)
    {
        _descriptionGroup.DOKill();
        _descriptionGroup.DOFade(doShow ? 1 : 0, 0.2f);
        
        _descriptionGroup.interactable = doShow;
        _descriptionGroup.blocksRaycasts = doShow;
        
        _descriptionText.text = data.Description;
        _descriptionTitleText.text = data.Name;
    }
}
