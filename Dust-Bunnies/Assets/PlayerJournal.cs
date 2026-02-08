using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.IO;
using System.Collections;
using System;

public class PlayerJournal : MonoBehaviour
{
    [Header("UI Assignments")]
    public GameObject journalPanel;
    public RawImage photoDisplay;        
    public TMP_InputField noteInput;     

    [Header("Settings")]
    public KeyCode openJournalKey = KeyCode.J;
    public KeyCode closeJournalKey = KeyCode.Escape;
    public KeyCode takePhotoKey = KeyCode.P;

    private string lastPhotoPath;
    private bool isJournalOpen = false;

    void Start()
    {
        // Journal is closed at start
        journalPanel.SetActive(false);
    }

    void Update()
    {
        // Toggle Journal
        if (!isJournalOpen && Input.GetKeyDown(openJournalKey))
        {
            OpenJournal();
        }
        else if (isJournalOpen && Input.GetKeyDown(closeJournalKey))
        {
            CloseJournal();
        }

        // Take Photo (Only if journal is closed)
        if (!isJournalOpen && Input.GetKeyDown(takePhotoKey))
        {
            StartCoroutine(CapturePhoto());
        }
    }

    void OpenJournal()
    {
        isJournalOpen = true;
        journalPanel.SetActive(true);
        
        // Unlock Cursor for typing
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadLastPhoto();
    }

    void CloseJournal()
    {
        isJournalOpen = false;
        journalPanel.SetActive(false);

        // Lock Cursor back to game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator CapturePhoto()
    {
        // Wait for end of frame to ensure UI is drawn (or hidden)
        yield return new WaitForEndOfFrame();

        // Capture screen
        Texture2D screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        // Save to file
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"journal_photo_{timestamp}.png";
        lastPhotoPath = Path.Combine(Application.persistentDataPath, filename);

        byte[] imageBytes = screenImage.EncodeToPNG();
        File.WriteAllBytes(lastPhotoPath, imageBytes);
        Destroy(screenImage);

        Debug.Log("New photo saved to: " + lastPhotoPath);
    }

    void LoadLastPhoto()
    {
        if (!string.IsNullOrEmpty(lastPhotoPath) && File.Exists(lastPhotoPath))
        {
            // Clean up old texture memory
            if (photoDisplay.texture != null) Destroy(photoDisplay.texture);

            byte[] imageBytes = File.ReadAllBytes(lastPhotoPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            photoDisplay.texture = tex;
            photoDisplay.color = Color.white;
        }
        else
        {
            // No photo taken yet, make the space clear or show a placeholder
            photoDisplay.texture = null;
            photoDisplay.color = new Color(0, 0, 0, 0); // Transparent
        }
    }
    
    public void ToggleJournalButton()
    {
        if (isJournalOpen)
            CloseJournal();
        else
            OpenJournal();
    }
}