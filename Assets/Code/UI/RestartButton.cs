using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Infringed.UI
{
    public class RestartButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(_RestartGame);
        }

        private void _RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
