using UnityEngine;

public class AudioStateManager : MonoBehaviour
{
    [SerializeField] float defaultReverbSfxValue = 0;
    [SerializeField] float defaultRoomToneVolValue = 0;

    [SerializeField] AK.Wwise.RTPC reverbSfx = null;
    [SerializeField] AK.Wwise.RTPC roomToneVol = null;

    private void Start()
    {
        DefaultStates();
        DefaultRtpcValues();
    }

    public void DefaultStates()
    {
        AkSoundEngine.SetState("AreaStates", "empty");
        AkSoundEngine.SetState("AudioState", "Gameplay");
    }

    public void DefaultRtpcValues()
    {
        reverbSfx.SetGlobalValue(defaultReverbSfxValue);
        roomToneVol.SetGlobalValue(defaultRoomToneVolValue);
    }
}
