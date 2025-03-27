using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;

    [Header("Dialogue Settings")]

    public float typingSpeed = 0.05f;

    [Header("Input Settings")]

    public InputActionReference continueDialogueAction;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private bool isDialogueActive;

    public static DialogueManager Instance { get; private set; }

    public UnityEvent onDialogueEnded;

    private void Awake()
    {
        if(Instance != null && Instance !=this)
        {
            Destroy(this.gameObject);
            return;
        }


        Instance = this;

        dialoguePanel.SetActive(false);

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        if(continueDialogueAction != null)
        {
            continueDialogueAction.action.started += OnContinueDialogueInput;
        }
        if(true)
        {
            
        }

    }

    private void OnEnable()
    {

        if(continueDialogueAction != null)
        {
            continueDialogueAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if(continueDialogueAction != null)
        {
            continueDialogueAction.action.Disable();
        }
    }

    private void OnDestroy()
    {
        if(continueDialogueAction != null)
        {
            continueDialogueAction.action.started -= OnContinueDialogueInput;
        }

    }

    private void OnContinueDialogueInput(InputAction.CallbackContext context)
    {
        if(!isDialogueActive)
        {
            return;
        }

        if(isTyping)
            {
            CompleteTyping();
            }
        else
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        DisplayNextLine();

    }

    private void DisplayNextLine()
    {
        if(currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        if(continueIndicator)
        {
            continueIndicator.SetActive(false);
        }


        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
        currentLineIndex++;

    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach(char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if(continueIndicator)
        {
            continueIndicator.SetActive(true);
        }

    }

    private void CompleteTyping()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentLines[currentLineIndex - 1];

        isTyping = false;

        if(continueIndicator)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        onDialogueEnded?.Invoke();


    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }


    





  
}
