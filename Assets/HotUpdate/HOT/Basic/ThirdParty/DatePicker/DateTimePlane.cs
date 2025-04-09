using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DateTimePlane : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown year;
    [SerializeField] private TMP_Dropdown month;
    [SerializeField] private TMP_InputField hour;
    [SerializeField] private TMP_InputField minute;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform prefab;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Button today;
    [SerializeField] private Button clear;
    [SerializeField] private Button submit;

    public delegate void GetDateTime(string dateTime);
    public event GetDateTime GetDateTimeEvent;

    public void Start()
    {
        today.onClick.AddListener(() =>
        {
            year.captionText.text = DateTime.Now.Year + "年";
            month.captionText.text = DateTime.Now.Month + "月";
            ShowDay(DateTime.Now.Day);
        });

        clear.onClick.AddListener(() =>
        {
            GetDateTimeEvent?.Invoke(null);
            gameObject.SetActive(false);
        });

        submit.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        hour.onValueChanged.AddListener(arg0 => { GetSelectDateTime(); });
        minute.onValueChanged.AddListener(arg0 => { GetSelectDateTime(); });

        year.onValueChanged.AddListener((int arg0) => { ShowDay(); });
        month.onValueChanged.AddListener((int arg0) => { ShowDay(); });
    }

    public void ShowAsync(Vector2 anchoredPosition, DateTime dateTime)
    {
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        hour.text = dateTime.Hour.ToString(); 
        minute.text = dateTime.Minute.ToString();
        ShowYearAndMonth(year, month, day);
        GetSelectDateTime();
        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        gameObject.SetActive(true);
    }

    private void ShowYearAndMonth(int year = -1, int month = -1,int day = -1)
    {
        if (year == -1) year = DateTime.Now.Year;
        if (month == -1) month = DateTime.Now.Month;

        this.year.options.Clear();
        for (int i = year; i >= year - 5; i--)
        {
            this.year.options.Add(new TMP_Dropdown.OptionData(i + "年"));
        }
        this.year.itemText.text = year + "年";

        this.month.options.Clear();
        for (int i = 1; i <= 12; i++)
        {
            this.month.options.Add(new TMP_Dropdown.OptionData(i + "月"));
        }
        this.month.value = month - 1;

        this.year.captionText.text = DateTime.Now.Year + "年";
        this.month.captionText.text = DateTime.Now.Month + "月";

        ShowDay(day);
    }

    private void ShowDay(int day = -1)
    {
        while (parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
        int year = int.Parse(this.year.captionText.text.Replace("年", ""));
        int month = int.Parse(this.month.captionText.text.Replace("月", ""));
        if (day == -1) day = DateTime.Now.Day;

        DateTime dateTime = new DateTime(year, month, 1);

        for (int i = Convert.ToInt32(dateTime.DayOfWeek); i > 0; i--)
        {
            Transform tran = Instantiate(prefab, parent);
            tran.GetComponentInChildren<TextMeshProUGUI>().text = "";
            tran.GetComponent<Toggle>().interactable = false;
            tran.gameObject.SetActive(true);
        }

        for (int i = 0; i < DateTime.DaysInMonth(year, month); i++)
        {
            Transform tran = Instantiate(prefab, parent);
            tran.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            tran.GetComponent<Toggle>().onValueChanged.AddListener((bool arg0) => { if (arg0) { GetSelectDateTime(); } });
            if (i + 1 == day && year == DateTime.Now.Year && month == DateTime.Now.Month) tran.GetComponent<Toggle>().isOn = true;
            tran.gameObject.SetActive(true);
        }
    }

    private void GetSelectDateTime()
    {
        if (!toggleGroup.AnyTogglesOn()) GetDateTimeEvent?.Invoke(null);
        int year = int.Parse(this.year.captionText.text.Replace("年", ""));
        int month = int.Parse(this.month.captionText.text.Replace("月", ""));
        foreach (var item in toggleGroup.ActiveToggles())
        {
            if (!int.TryParse(this.hour.text.Trim(), out int hour)) hour = 0;
            if (!int.TryParse(this.minute.text.Trim(), out int minute)) minute = 0;
            if (!int.TryParse(item.GetComponentInChildren<TextMeshProUGUI>().text, out int day)) day = 1;
            GetDateTimeEvent?.Invoke(new DateTime(year, month, day, hour, minute, 0).ToString("yyyy-MM-dd HH:mm"));
        }
    }

    private void OnDisable()
    {
        GetDateTimeEvent = null;
    }
}
