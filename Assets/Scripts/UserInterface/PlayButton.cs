using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class PlayButton : MonoBehaviour
    {
        private Button _button;
        
        void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(LoadMainScene);
        }

        void LoadMainScene()
        {
            SceneManager.LoadScene(1);
        }


    }
}
