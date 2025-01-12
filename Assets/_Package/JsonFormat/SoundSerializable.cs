[System.Serializable]
public class SoundSerializable
{
    public bool isSound = true;
    public float volumeSound = 1;
    public bool isMusic = true;
    public float volumeMusic = 1;
    public bool isVibrate = true;

    public SoundSerializable()
    {
        isMusic = true;
        volumeMusic = 1;
        isSound = true;
        volumeSound = 1;
        isVibrate = true;
    }
}
