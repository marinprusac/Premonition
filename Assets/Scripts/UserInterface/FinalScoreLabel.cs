using Actions;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class FinalScoreLabel : MonoBehaviour
    {
        void Start()
        {
            GetComponent<TMP_Text>().text = $"Good game!\nYour final score is:\n{GameOverChecker.Instance.finalScore}";
            GameOverChecker.Instance.Destroy();
        }
    }
}
