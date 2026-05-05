using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Text scoreText;

   private static UIManager u_instance;
    public static UIManager Instance
    {
        get
        {
            if (u_instance == null)
            {
                u_instance = new UIManager();
            }
            return u_instance;
        }
    }
    

    private void Awake()
    {
        scoreText.text = "Score: " + scoreText.text;

        if (u_instance == null)
        {
            u_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 인스턴스가 이미 할당돼있다면(2개 이상이라면) 파괴합니다.
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowScoreText(int score)
    {
        scoreText.text = "Score: " + score;
    }
}
