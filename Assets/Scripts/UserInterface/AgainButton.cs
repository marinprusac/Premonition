using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class AgainButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(PlayAgain);
        }

        void PlayAgain()
        {
            SceneManager.LoadScene(0);
        }


    }
}
