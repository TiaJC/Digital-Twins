/*
***********************************
作者:       TianJC
创建时间:   2022/05/10 11:49:21
模块描述:   选择框下拉总是回调
***********************************
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CustomDropdown : Dropdown
{
    public bool AlwaysCallback = true;//是否开启 点击选项按钮总是回调

    public new void Show()
    {
        base.Show();
        Transform toggleRoot = transform.Find("Dropdown List/Viewport/Content");
        Toggle[] toggleList = toggleRoot.GetComponentsInChildren<Toggle>(false);
        for (int i = 0; i < toggleList.Length; i++)
        {
            Toggle temp = toggleList[i];
            temp.onValueChanged.RemoveAllListeners();
            temp.isOn = false;
            temp.onValueChanged.AddListener(x => OnSelectItemEx(temp));
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Show();
    }

    public void OnSelectItemEx(Toggle toggle)
    {
        if (!toggle.isOn)
        {
            toggle.isOn = true;
            return;
        }

        int selectedIndex = -1;
        Transform tr = toggle.transform;
        Transform parent = tr.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == tr)
            {
                selectedIndex = i - 1;
                break;
            }
        }

        if (selectedIndex < 0)
            return;
        if (value == selectedIndex && AlwaysCallback)
            onValueChanged.Invoke(value);
        else
            value = selectedIndex;
        Hide();
    }
}