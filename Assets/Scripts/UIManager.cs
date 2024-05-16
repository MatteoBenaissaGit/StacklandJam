using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Cards;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _descriptionGroup;
    [SerializeField] private TMP_Text _descriptionTitleText;
    [SerializeField] private TMP_Text _descriptionText;
    [Space(10)] 
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private Image _dayFillImage;
    [SerializeField] private GameObject _dayAlertIcon;
    [Space(10)] 
    [SerializeField] private GameObject _quotaAlertIcon;
    [SerializeField] private GameObject _quotaGauge;
    [SerializeField] private Image _quotaFillImage;
    [SerializeField] private TMP_Text _quotaText;
    [Space(10)]
    [SerializeField] private CanvasGroup _gameOverGroup;
    [SerializeField] private Button _gameOverButton;
    
    private void Awake()
    {
        _descriptionGroup.alpha = 0;
        _descriptionGroup.interactable = false;
        _descriptionGroup.blocksRaycasts = false;
        
        _gameOverGroup.interactable = false;
        _gameOverGroup.blocksRaycasts = false;
        _gameOverGroup.alpha = 0;
        
        _gameOverButton.onClick.AddListener(() => SceneManager.LoadScene(0));
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
        _dayAlertIcon.SetActive(fillAmount > 0.65f);
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
    
    public async void ShowGameOver()
    {
        _gameOverGroup.interactable = true;
        _gameOverGroup.blocksRaycasts = true;
        
        _gameOverGroup.DOKill();
        _gameOverGroup.DOFade(1, 0.2f);
    }
}
