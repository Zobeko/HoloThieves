using Holo;

public class BeginVentPartBehaviour : HoloBehaviour
{
    [Serialized] private HoloGameObject vent;
    [Serialized] private HoloGameObject hackedText;
    [Serialized] private bool isDroneHacked = false;
    [SharedAudioComponent] private SharedAudioComponent dronePirateAudioSource;

    public override void Start()
    {
        base.Start();
        Async.OnUpdate += Update;
        
    }

    public void Update()
    {
        if (hackedText.activeSelf && !isDroneHacked)
        {
            isDroneHacked = true;
            vent.SetActive(true);
            dronePirateAudioSource.Play();
        }
    }
}
