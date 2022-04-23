using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MenuOptions : MonoBehaviour
{
    public Animator loadingScreen; //loading screen animator to fade in the loading screen before changing scenes
    public Slider loadingBar; //the progress bar on the loading screen
    public TextMeshProUGUI progressPercentage; //the progress percentage on the progress bar

    public void LoadScene(int buildIndex)
    {
        StartCoroutine(Load(buildIndex));
    }

    IEnumerator Load(int buildIndex)
    {
        //start fading in the loading screen and wait for it to finish before starting to change the scenes
        loadingScreen.Play("FadeIn");
        yield return new WaitForSeconds(loadingScreen.GetCurrentAnimatorClipInfo(0).Length);

        //start loading the next scene in the background
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(buildIndex);
        while (!asyncOperation.isDone)
        {
            //while the next scene is not ready update the progress bar and percentage displayed on the loading screen
            float progress = Mathf.Clamp01(asyncOperation.progress / .9f);

            loadingBar.value = progress;
            progressPercentage.text = (progress * 100).ToString("0") + "%";

            yield return null;
        }
    }
}
