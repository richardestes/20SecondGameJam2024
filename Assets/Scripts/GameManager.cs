using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public Image buttonPromptImage;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text streakText;
    public TMP_Text averageReactionTimeText;
    public TMP_Text highestStreakText;
    public GameObject streakScreen;
    public GameObject endGameScreen;
    [Range(0.1f,1f)]
    public float vibrationDuration = 0.2f;
    [Range(0.1f,1f)]
    public float vibrationStrengthLow, vibrationStrengthHigh = 0.5f;
    [SerializeField]
    public SerializableDictionary<string, Sprite> iconDictionary;

    private Dictionary<string, InputAction> inputActionMap;
    private MainInputActions inputActions;
    private string currentButtonPrompt;
    private float promptStartTime;
    
    private float timer = 20f;
    private int baseScore = 0;
    private int totalReactionScore = 0;
    private float totalReactionTime = 0f;
    private float averageReactionTime = 0f;
    private int totalScore = 0;
    private float streakScore = 1f;
    private int currentStreak = 0;
    private float highestStreak = 1f;
    private float totalStreakScore = 0;
    private bool gameActive = true;
    private bool gameEnded = false;
    private bool controllerConnectedGeneric;
    private bool controllerConnectedPlaystation;
    private bool controllerConnectedKBM;

    void Start()
    {
        if (buttonPromptImage == null)
        {
            Debug.LogError("Button prompt image is null"); 
            return;
        }
        if (scoreText == null)
        {
            Debug.LogError("Score text element is null"); 
            return;
        } 
        if (endGameScreen == null)
        {
            Debug.LogError("End game screen element is null"); 
            return;
        }
        else if (endGameScreen.activeSelf)
        {
            endGameScreen.SetActive(false);
        }  
        GenerateNewButtonPrompt();
    }
    
    void Awake()
    {
        inputActions = new MainInputActions();
        SetupControllers();
        SetupContexts();
        streakScreen.SetActive(false);
        inputActions.Enable();
    }
    
    void SetupControllers()
    {
        if(Gamepad.current == null) return;
        foreach (var device in Gamepad.all)
        {
            Debug.Log(device.name);
            if(device.name.Contains("XInput"))
            {
                controllerConnectedGeneric = true;
            }
            if(device.name.Contains("Sony"))
            {
                controllerConnectedPlaystation = true;
            }
        }
        return;
    }
    
    
    void SetupContexts()
    {
        inputActionMap = new Dictionary<string, InputAction>();
        List<KeyValuePair<string, InputAction>> genericButtons = new List<KeyValuePair<string, InputAction>>()
        {
            new KeyValuePair<string, InputAction>("GenericA", inputActions.Generic.GenericA),  
            new KeyValuePair<string, InputAction>("GenericB", inputActions.Generic.GenericB),
            new KeyValuePair<string, InputAction>("GenericY", inputActions.Generic.GenericY),
            new KeyValuePair<string, InputAction>("GenericX", inputActions.Generic.GenericX),
            new KeyValuePair<string, InputAction>("GenericLB", inputActions.Generic.GenericLB),  
            new KeyValuePair<string, InputAction>("GenericLT", inputActions.Generic.GenericLT),
            new KeyValuePair<string, InputAction>("GenericRB", inputActions.Generic.GenericRB),
            new KeyValuePair<string, InputAction>("GenericRT", inputActions.Generic.GenericRT),
            new KeyValuePair<string, InputAction>("GenericL3", inputActions.Generic.GenericL3),  
            new KeyValuePair<string, InputAction>("GenericR3", inputActions.Generic.GenericR3),
            new KeyValuePair<string, InputAction>("GenericSelect", inputActions.Generic.GenericSelect),
            new KeyValuePair<string, InputAction>("GenericStart", inputActions.Generic.GenericStart),
            new KeyValuePair<string, InputAction>("GenericDPadUp", inputActions.Generic.GenericDPadUp),  
            new KeyValuePair<string, InputAction>("GenericDPadLeft", inputActions.Generic.GenericDPadLeft),
            new KeyValuePair<string, InputAction>("GenericDPadRight", inputActions.Generic.GenericDPadRight),
            new KeyValuePair<string, InputAction>("GenericDPadDown", inputActions.Generic.GenericDPadDown),
        };
        
        List<KeyValuePair<string, InputAction>> playstationButtons = new List<KeyValuePair<string, InputAction>>()
        {
            new KeyValuePair<string, InputAction>("PlaystationCross", inputActions.Playstation.PlaystationCross),
            new KeyValuePair<string, InputAction>("PlaystationCircle", inputActions.Playstation.PlaystationCircle),
            new KeyValuePair<string, InputAction>("PlaystationTriangle", inputActions.Playstation.PlaystationTriangle),
            new KeyValuePair<string, InputAction>("PlaystationSquare", inputActions.Playstation.PlaystationSquare),
            new KeyValuePair<string, InputAction>("PlaystationL1", inputActions.Playstation.PlaystationL1),
            new KeyValuePair<string, InputAction>("PlaystationL2", inputActions.Playstation.PlaystationL2),
            new KeyValuePair<string, InputAction>("PlaystationR1", inputActions.Playstation.PlaystationR1),
            new KeyValuePair<string, InputAction>("PlaystationR2", inputActions.Playstation.PlaystationR2),
            new KeyValuePair<string, InputAction>("PlaystationL3", inputActions.Playstation.PlaystationL3),
            new KeyValuePair<string, InputAction>("PlaystationR3", inputActions.Playstation.PlaystationR3),
            new KeyValuePair<string, InputAction>("PlaystationSelect", inputActions.Playstation.PlaystationSelect),
            new KeyValuePair<string, InputAction>("PlaystationStart", inputActions.Playstation.PlaystationStart),
            new KeyValuePair<string, InputAction>("PlaystationDPadUp", inputActions.Playstation.PlaystationDPadUp),  
            new KeyValuePair<string, InputAction>("PlaystationDPadLeft", inputActions.Playstation.PlaystationDPadLeft),
            new KeyValuePair<string, InputAction>("PlaystationDPadRight", inputActions.Playstation.PlaystationDPadRight),
            new KeyValuePair<string, InputAction>("PlaystationDPadDown", inputActions.Playstation.PlaystationDPadDown),
        };
        
        if (controllerConnectedGeneric)
        {
            foreach (var kvp in genericButtons)
            {
                inputActionMap.Add(kvp.Key, kvp.Value);
            }
        }
        
        if (controllerConnectedPlaystation)
        {
            foreach (var kvp in playstationButtons)
            {
                inputActionMap.Add(kvp.Key, kvp.Value);
            }
        }
        
        // Setup callbacks to detect if correct button is pressed
        foreach (var action in inputActionMap.Values)
        {
            action.performed += ctx => OnButtonPress(ctx.action.name);
        }
        
    }
    
    void OnEnable()
    {
        inputActions?.Enable();
    }

    void OnDisable()
    {
        inputActions?.Disable();
    }
    
    void Update()
    {
        if (!gameActive && Input.GetKeyDown(KeyCode.R)) 
        {
            Restart();
        }
        else if (!gameActive) return;
        if (timer <= 0) EndGame();
        if (gameActive)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();
        }
    }

    void GenerateNewButtonPrompt()
    {
        KeyValuePair<string, InputAction> randomButton = GetRandomElement(inputActionMap);
        currentButtonPrompt = randomButton.Key;
        bool imageExists = iconDictionary.TryGetValue(currentButtonPrompt, out Sprite buttonImage);
        if (imageExists)
        {
            buttonPromptImage.sprite = buttonImage;
            // Prevent stretching
            buttonPromptImage.preserveAspect = true;
            // buttonPromptImage.SetNativeSize(); 
        }
        else
        {
            Debug.Log("Button image not found");
        }
        promptStartTime = Time.time;
    }
    
    private KeyValuePair<string, InputAction> GetRandomElement(Dictionary<string, InputAction> dictionary)
    {
        List<KeyValuePair<string, InputAction>> list = new(dictionary);
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    void OnButtonPress(string buttonPressed)
    {
        Debug.Log(buttonPressed);
        // Correct button     
        if (gameActive && buttonPressed == currentButtonPrompt)
        {
            // Calculate reaction time
            float reactionTime = Time.time - promptStartTime;
            totalReactionTime += reactionTime;
            baseScore += 1;
            int reactionScore = Mathf.Max(1, (int)(.1/reactionTime));        
            if (reactionTime >= 1.2f)
            {
                currentStreak = 0;
                streakScore = 0f;
                streakScreen.SetActive(false);
            }
            else
            {
                streakScore += .2f;
                currentStreak++;
            }
            if (currentStreak >= 5)
            {
                streakScreen.SetActive(true);
                streakText.text = "Streak: " + currentStreak;
            }
            if (currentStreak > highestStreak) highestStreak = currentStreak;
            totalStreakScore += streakScore;
            totalReactionScore += reactionScore;
            totalScore = (int)(totalScore + 1 + reactionScore + streakScore);
        }
        // Incorrect button
        else
        {
            totalScore -= 2;
            currentStreak = 0;
            streakScore = 0f;
            VibrateController();
            streakScreen.SetActive(false);
        }
        scoreText.text = totalScore.ToString();
        GenerateNewButtonPrompt();
    }

    void EndGame()
    {
        inputActions.Disable();
        // Ensures this only runs once
        if(gameEnded) return;
        gameEnded = true;
        gameActive = false;
        CalculateAverageReactionTime();
        
        // Display end game screen
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(true);
            averageReactionTimeText.text = "Average reaction time: " + averageReactionTime.ToString() + " seconds";
            highestStreakText.text = "Highest streak: " + GetHighestStreak().ToString();
        }
        
        Debug.Log("Base score: " + baseScore.ToString());
        Debug.Log("Streak score: " + totalStreakScore.ToString());
        Debug.Log("Reaction score: " + totalReactionScore.ToString());
        Debug.Log("Average reaction time: " + averageReactionTime);
        Debug.Log("Total score: " + totalScore.ToString());
    }
    
    void Restart()
    {
        timer = 20f;
        gameEnded = false;
        gameActive = true;
        baseScore = 0;
        totalReactionScore = 0;
        totalReactionTime = 0f;
        averageReactionTime = 0f;
        totalScore = 0;
        currentStreak = 0;
        streakScore = 1f;
        totalStreakScore = 0;
        scoreText.text = "0";
        endGameScreen.SetActive(false);
        streakScreen.SetActive(false);
        inputActions?.Enable();
        Start();
    }
    
    int GetHighestStreak()
    {
        return (int)highestStreak;
    }
    
    void CalculateAverageReactionTime()
    {
        averageReactionTime = Mathf.Round(totalReactionTime / baseScore * 100f) / 100f;
    }
    
    
    void VibrateController()
    {
        // Check if a gamepad is connected
        if (Gamepad.current != null)
        {
            // Set motor speeds (low frequency, high frequency) and duration
            Gamepad.current.SetMotorSpeeds(vibrationStrengthLow,vibrationStrengthHigh);
            Invoke("StopVibration", vibrationDuration);
        }
    }
    
    void StopVibration()
    {
        Gamepad.current?.SetMotorSpeeds(0,0);
    }
}
