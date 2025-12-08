using UnityEngine;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance; 
    
    public GameObject postProcessingNormal;
    public GameObject postProcessingJournal;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void TurnOnJournal() {
        postProcessingJournal.SetActive(true);
        postProcessingNormal.SetActive(false);
    }
    
    public void TurnOffJournal() {
        postProcessingJournal.SetActive(false);
        postProcessingNormal.SetActive(true);
    }

}
