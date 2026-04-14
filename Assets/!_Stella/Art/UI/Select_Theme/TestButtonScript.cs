using UnityEngine;
using UnityEngine.InputSystem;

public class TestButtonScript : MonoBehaviour
{
    
    public void OnButtonClick()
    {
        Debug.Log("Button Clicked!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current != null)
            Debug.Log("마우스 인식됨: " + Mouse.current.position.ReadValue());
        else
            Debug.Log("마우스 없음!");
    }
}
