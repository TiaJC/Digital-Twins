// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2023/01/16 11:45:02
// 模块描述:   添加事件监听
// ***********************************************************************

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class BindEventTrigger
{
    public static EventTrigger.Entry Bind(this GameObject _gameObject, EventTriggerType triggerType, Action<PointerEventData> func)
    {
        EventTrigger trigger = _gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = _gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener((data) => { func?.Invoke((PointerEventData)data); });
        trigger.triggers.Add(entry);
        return entry;
    }

    public static void Remove(this GameObject _gameObject, EventTriggerType triggerType)
    {
        EventTrigger trigger = _gameObject.GetComponent<EventTrigger>();
        if (trigger == null) return;
        try
        {
            for (int i = 0; i < trigger.triggers.Count; i++)
            {
                if (trigger.triggers[i].eventID == triggerType)
                {
                    trigger.triggers.RemoveAt(i);
                    i--;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Append("移除时间监听", ex.Message);
        }
    }

    public static void Remove(this GameObject _gameObject, EventTrigger.Entry entry)
    {
        EventTrigger trigger = _gameObject.GetComponent<EventTrigger>();
        if (trigger == null) return;
        if (trigger.triggers.Contains(entry))
        {
            try
            {
                trigger.triggers.Remove(entry);
            }
            catch (Exception ex)
            {
                Log.Append("移除时间监听",ex.Message);
            }
        }
    }

    public static void RemoveAll(this GameObject _gameObject)
    {
        EventTrigger trigger = _gameObject.GetComponent<EventTrigger>();
        if (trigger == null) return;
        try
        {
            UnityEngine.Object.Destroy(trigger);
        }
        catch (Exception ex)
        {
            Log.Append("移除时间监听", ex.Message);
        }
    }
}
