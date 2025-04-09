// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2018/01/18 11:03:37
// 模块描述:   数值自增动画
// ***********************************************************************

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class Emoticons
{
    public static float SetNumber(this TextMeshProUGUI textMeshPro, double number, string prefix = null, string suffix = null, bool zero = false, float duration = 0)
    {
        EmoticonsVluae emoticonsVluae = textMeshPro.GetComponent<EmoticonsVluae>();
        if (emoticonsVluae == null) emoticonsVluae = textMeshPro.gameObject.AddComponent<EmoticonsVluae>();
        return emoticonsVluae.CountingNumber(textMeshPro, number, prefix, suffix, zero, duration);
    }


    public static void SetNumber(this Text textMeshPro, double number, string prefix = null, string suffix = null, bool zero = false, float duration = 0)
    {
        EmoticonsVluae emoticonsVluae = textMeshPro.GetComponent<EmoticonsVluae>();
        if (emoticonsVluae == null) emoticonsVluae = textMeshPro.gameObject.AddComponent<EmoticonsVluae>();
        emoticonsVluae.CountingNumber(textMeshPro, number, prefix, suffix, zero, duration);
    }
}

public class EmoticonsVluae : MonoBehaviour
{
    int integerLength = 0;  //整数长度
    int decimalLength = 0;  //小数长度
    private double currentValue;

    public void CountingNumber(Text textMeshPro, double number, string prefix, string suffix, bool zero, float duration)
    {
        string[] str = number.ToString().Split('.');
        integerLength = str[0].Length;
        if (str.Length == 2) decimalLength = str[1].Length;

        DOTween.To(
            () => currentValue = zero ? 0 : currentValue,
            x => currentValue = x,
            number,
           duration: duration == 0 ? (integerLength - 1 <= 0 ? 0.5F : integerLength - 1) : duration
            ).OnUpdate(() =>
            {
                textMeshPro.text = prefix + currentValue.ToString("f" + decimalLength.ToString()) + suffix;
            }).OnComplete(() =>
            {
                currentValue = number;
                textMeshPro.text = prefix + currentValue.ToString("f" + decimalLength.ToString()) + suffix;
            });
    }

    public float CountingNumber(TextMeshProUGUI textMeshPro, double number, string prefix, string suffix, bool zero, float duration)
    {
        string[] str = number.ToString().Split('.');
        integerLength = str[0].Length;
        if (str.Length == 2) decimalLength = str[1].Length;
        float time = duration == 0 ? (integerLength - 1 <= 0 ? 0.5F : integerLength - 1) : duration;
        DOTween.To(
           () => currentValue = zero ? 0 : currentValue,
           x => currentValue = x,
           number,duration: time
           ).OnUpdate(() =>
           {
               textMeshPro.text = prefix + currentValue.ToString("f" + decimalLength.ToString()) + suffix;
           }).OnComplete(() =>
           {
               currentValue = number;
               textMeshPro.text = prefix + currentValue.ToString("f" + decimalLength.ToString()) + suffix;
           });
        return time;
    }
}
