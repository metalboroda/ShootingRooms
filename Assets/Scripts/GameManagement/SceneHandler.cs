using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameManagement
{
    public class SceneHandler : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}