using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Transform loading;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        StartCoroutine(LoadingScene());
    }

    private IEnumerator LoadingScene()
    {
        loading.gameObject.SetActive(true);

        foreach (Transform child in transform.parent)
        {
            if (child.TryGetComponent<Button>(out var b))
                button.enabled = false;
        }

        yield return new WaitForEndOfFrame();

        var task = SceneManager.LoadSceneAsync(sceneName);

        while (!task.isDone)
        {
            loading.eulerAngles += Vector3.forward * 360 * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

    }
}
