using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Infringed.UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private Transform _loading;
        private Button _button;
        private bool _isLoading = false;

        private void Awake()
        {
            _button = GetComponent<Button>();

            _button.onClick.AddListener(_LoadLevel);
        }

        private void Update()
        {
            if (!_isLoading)
                return;

            _loading.eulerAngles += Vector3.forward * 360 * Time.deltaTime;
        }

        private void _LoadLevel()
        {
            StartCoroutine(_LoadingScene());
        }

        private IEnumerator _LoadingScene()
        {
            _loading.gameObject.SetActive(true);

            foreach (Transform child in transform.parent)
            {
                if (child.TryGetComponent<Button>(out var b))
                    _button.enabled = false;
            }

            var task = SceneManager.LoadSceneAsync(_sceneName);
            _isLoading = true;

            while (!task.isDone)
            {
                yield return null;
            }

        }
    }
}
