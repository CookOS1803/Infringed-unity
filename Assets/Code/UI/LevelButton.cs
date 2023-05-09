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
    private bool isLoading = false;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        StartCoroutine(LoadingScene());
    }

    private void Update()
    {
        if (!isLoading)
            return;

        loading.eulerAngles += Vector3.forward * 360 * Time.deltaTime;
    }

    private IEnumerator LoadingScene()
    {
        loading.gameObject.SetActive(true);

        foreach (Transform child in transform.parent)
        {
            if (child.TryGetComponent<Button>(out var b))
                button.enabled = false;
        }

        var task = SceneManager.LoadSceneAsync(sceneName);
        isLoading = true;

        while (!task.isDone)
        {
            yield return null;
        }

    }
}
