using System;
using System.Collections;
using System.Collections.Generic;
using Data.Cards;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _descriptionGroup;
    [SerializeField] private TMP_Text _descriptionTitleText;
    [SerializeField] private TMP_Text _descriptionText;
    [Space(10)] 
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private Image _dayFillImage;
    [Space(10)] 
    [SerializeField] private GameObject _quotaAlertIcon;
    [SerializeField] private GameObject _quotaGauge;
    [SerializeField] private Image _quotaFillImage;
    [SerializeField] private TMP_Text _quotaText;
    
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

    public void UpdateFillDay(float fillAmount)
    {
        _dayFillImage.fillAmount = 1 - fillAmount;
    }
    
    public void SetNewDay(int day)
    {
        _dayText.text = $"Day {day}";
    }

    public void UpdateQuota(int current, int total)
    {
        _quotaText.text = $"{current} / {total}";
    }
    
    public void UpdateFillQuota(float fillAmount)
    {
        _quotaGauge.SetActive(fillAmount > 0);
        _quotaFillImage.fillAmount = fillAmount;
        _quotaAlertIcon.SetActive(fillAmount < 0.5f);
    }
}
