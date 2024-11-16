using System;
using System.Collections;
using System.Collections.Generic;
using GolfCourse.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace GolfCourse.UI
{
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
    }
}
