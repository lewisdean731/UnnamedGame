using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private GameManager GM;

    private Label testLabel;
    private ProgressBar progressBar;
    private Button btnTimeScalex05, btnTimeScalex1, btnTimeScalex2, btnTimeScalex3, btnTimeScalex4, btnTimeScalex5;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        testLabel = root.Q<Label>("lblSeconds");
        progressBar = root.Q<ProgressBar>("DayProgress");
        btnTimeScalex05 = root.Q<Button>("btnTimeScalex05");
        btnTimeScalex1 = root.Q<Button>("btnTimeScalex1");
        btnTimeScalex2 = root.Q<Button>("btnTimeScalex2");
        btnTimeScalex3 = root.Q<Button>("btnTimeScalex3");
        btnTimeScalex4 = root.Q<Button>("btnTimeScalex4");
        btnTimeScalex5 = root.Q<Button>("btnTimeScalex5");

        btnTimeScalex05.clicked += () => { GameManager.timeScale = 0.5f; };
        btnTimeScalex1.clicked += () => { GameManager.timeScale = 1; };
        btnTimeScalex2.clicked += () => { GameManager.timeScale = 2; };
        btnTimeScalex3.clicked += () => { GameManager.timeScale = 3; };
        btnTimeScalex4.clicked += () => { GameManager.timeScale = 4; };
        btnTimeScalex5.clicked += () => { GameManager.timeScale = 5; };
    }

    private void Update()
    {
        testLabel.text = ((int)Math.Round(GameManager.elapsedTime)).ToString();
        progressBar.value = GameManager.dayProgressPercent;
    }
}