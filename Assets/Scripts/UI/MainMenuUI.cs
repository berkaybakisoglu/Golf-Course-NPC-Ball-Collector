using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        startButton.onClick.RemoveListener(StartGame);
        GameSceneManager.Instance.LoadScene(GameSceneManager.GameSceneName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
