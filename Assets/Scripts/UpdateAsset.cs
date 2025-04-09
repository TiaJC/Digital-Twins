using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;
using DG.Tweening;

public class UpdateAsset : MonoBehaviour
{
    public string url;
    [SerializeField] private TMPro.TextMeshProUGUI prompt;
    [SerializeField] private TMPro.TextMeshProUGUI version;
    [SerializeField] private Slider progress;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        Application.runInBackground = true;
        version.text = Application.version;  
    }

    private async void Start()
    {
        YooController.OnDownloadProgress += Instance_OnDownloadProgress;
        YooController.OnDownloadProgress += Instance_OnDownloadProgress;
        YooController.OnDownloadError += Instance_OnDownloadError;
        YooController.Prompt += YooController_Prompt;
        YooController.Complete += YooController_Complete;
        await YooController.UpdateAsset(url);
    }

    private async Task LoadMain()
    {
        prompt.text = "加载界面中";
        SceneOperationHandle sceneOperation = YooController.package.LoadSceneAsync("Main", LoadSceneMode.Additive);
        await sceneOperation;
    }

    private async Task LoadTerrain()
    {
        prompt.text = "加载地形中";
        SceneOperationHandle sceneOperation = YooController.package.LoadSceneAsync("Terrain", LoadSceneMode.Additive, true);
        float displayProgress = 0;
        float toProgress = 0;
        float range;
        while (displayProgress < 1)
        {
            toProgress = sceneOperation.Progress * 100 / 90;
            while (toProgress > displayProgress)
            {
                ++displayProgress;
                range = UnityEngine.Random.Range(0.5F, 1F);
                progress.DOValue(displayProgress, range).OnUpdate(() =>
                {
                    prompt.text = $"加载地形中 {(progress.value * 100).ToString("f2")}%";
                });
                await new WaitForSeconds(range);
            }
            await new WaitForFixedUpdate();
        }
        sceneOperation.UnSuspend();
    }

    private void YooController_Prompt(string content)
    {
        prompt.text = content;
    }

    private async void YooController_Complete()
    {
        //await LoadTerrain();
        await LoadMain();
        await SceneManager.UnloadSceneAsync("Start");
    }

    private void Instance_OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        progress.value = currentDownloadBytes / totalDownloadBytes;
        prompt.text = $"{(currentDownloadBytes / (1024 * 1024))}/{(totalDownloadBytes / (1024 * 1024))}";
    }

    private void Instance_OnDownloadError(string fileName, string error)
    {
        prompt.text = $"{fileName}下载出错:{error}";
    }
}
