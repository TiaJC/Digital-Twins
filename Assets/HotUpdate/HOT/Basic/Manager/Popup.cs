using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public abstract class Popup<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Transform tf = YooManager.package.LoadAssetSync(typeof(T).ToString()).InstantiateSync(GameObject.Find("顶部菜单").transform).transform;
                instance = tf.GetComponent<T>();
            }
            instance.transform.SetAsFirstSibling();
            return instance;
        }
    }

    public abstract void Init();

    public abstract void Hide();

    public abstract void Start();

    public virtual async void ShowAsync(params object[] o)
    {
        Start();
        Init();
        gameObject.SetActive(true);
        await new WaitForEndOfFrame();
    }
}
