/*
***********************************
作者:       TianJC
创建时间:   2022/05/07 09:44:00
模块描述:
***********************************
*/

#region 修改日志
/*
***********************************
版本号:
修改者:
修改时间:
修改描述:
***********************************
*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts;

public static class ChartHelper
{
    public static void Init(this BarChart barChart, string[] xData, Dictionary<string, float[]> yData)
    {
        if (xData != null)
        {
            barChart.ClearAxisData();
            barChart.xAxis0.splitNumber = xData.Length;
            for (int i = 0; i < xData.Length; i++)
            {
                barChart.AddXAxisData(xData[i].ToString());
            }
        }

        if (yData != null)
        {
            barChart.series.ClearData();
            List<string> keys = new List<string>(yData.Keys);
            for (int i = 0; i < keys.Count && i < barChart.series.Count; i++)
            {
                barChart.series.GetSerie(i).name = keys[i];
                barChart.RefreshLabel();
                List<float> values = new List<float>(yData[keys[i]]);
                for (int j = 0; j < values.Count; j++) {
                    barChart.series.AddData(i, values[j]);
                }
            }
        }
    }

    public static void Init(this LineChart lineChart, string[] xData, Dictionary<string, float[]> yData)
    {
        if (xData != null)
        {
            lineChart.ClearAxisData();
            lineChart.xAxis0.splitNumber = xData.Length;
            for (int i = 0; i < xData.Length; i++)
            {
                lineChart.AddXAxisData(xData[i].ToString());
            }
        }

        if (yData != null)
        {
            lineChart.series.ClearData();
            List<string> keys = new List<string>(yData.Keys);
            for (int i = 0; i < keys.Count && i < lineChart.series.Count; i++)
            {
                lineChart.series.GetSerie(i).name = keys[i];
                lineChart.RefreshLabel();
                List<float> values = new List<float>(yData[keys[i]]);
                for (int j = 0; j < values.Count; j++)
                {
                    lineChart.series.AddData(i, values[j]);
                }
            }
        }
    }

    public static void Init(this LineChart lineChart, string[] xData, Dictionary<string, double[]> yData)
    {
        if (xData != null)
        {
            lineChart.ClearAxisData();
            lineChart.xAxis0.splitNumber = xData.Length;
            for (int i = 0; i < xData.Length; i++)
            {
                lineChart.AddXAxisData(xData[i].ToString());
            }
        }

        if (yData != null)
        {
            lineChart.series.ClearData();
            List<string> keys = new List<string>(yData.Keys);
            for (int i = 0; i < keys.Count && i < lineChart.series.Count; i++)
            {
                lineChart.series.GetSerie(i).name = keys[i];
                lineChart.RefreshLabel();
                List<double> values = new List<double>(yData[keys[i]]);
                for (int j = 0; j < values.Count; j++)
                {
                    lineChart.series.AddData(i, values[j]);
                }
            }
        }
    }

    public static void Init(this PieChart pieChart, Dictionary<string, float> keyValuePairs)
    {
        pieChart.series.ClearData();
        foreach (var key in keyValuePairs.Keys)
        {
            if (keyValuePairs[key] == 0) continue;
            pieChart.AddData(0, keyValuePairs[key], key);
        }
    }

    public static void Init(this PieChart pieChart, Dictionary<float, string> keyValuePairs)
    {
        pieChart.series.ClearData();
        foreach (var key in keyValuePairs.Keys)
        {
            if (key == 0) continue;
            pieChart.AddData(0, key, keyValuePairs[key]);
        }
    }

    public static void Init(this PieChart pieChart, string serieName, int dataIndex, int dimension, double value)
    {
        pieChart.UpdateData(serieName, dataIndex, dimension, value);
        pieChart.AnimationFadeIn();
    }

    public static void Init(this PieChart pieChart, string serieName, int dataIndex, string dataName)
    {
        pieChart.UpdateDataName(serieName, dataIndex, dataName);
        pieChart.AnimationFadeIn();
    }

    public static void Init(this LiquidChart liquidChart, double value)
    {
        liquidChart.UpdateData(0, 0, 1, value);
        liquidChart.AnimationFadeIn();
    }

    public static void Init(this RingChart ringChart, float[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            ringChart.UpdateData(0, i, 0, values[i]);
        }
        //ringChart.AnimationFadeIn();
    }

    public static void Init(this GaugeChart gaugeChart, string serieName, int dataIndex, int dimension, double value)
    {
        gaugeChart.UpdateData(serieName, dataIndex, dimension, value);
        gaugeChart.AnimationFadeIn();
    }
}
