using Holo;

//HoloBehaviour rattaché à la downloadBar de l'interface wifi

public class BeginVentPartBehaviour : HoloBehaviour
{
    //Empty nommé "Conduit" qui contient tout ce qui a à voir avec le conduit d'aeration et le drone
    [Serialized] private HoloGameObject vent;

    //Texte "Drone piraté" présent sur l'interface de la wifi
    [Serialized] private readonly HoloGameObject hackedText;
    //true à partir du moment où le drone à été piraté
    public bool isDroneHacked = false;
    //AudioSource sur la download bar qui permet de jouer le son "Drone piraté !"
    [SharedAudioComponent] private readonly SharedAudioComponent dronePirateAudioSource;

    public override void Start()
    {
        isDroneHacked = false;
        Async.OnUpdate += Update;   
    }

    public void Update()
    {
        //Si le drone n'a pas déjà été piraté (!isDroneHacked) et que l'on vient de pirater le drone (quand le texte "Drone piraté" s'affiche)
        if (hackedText.activeSelf && !isDroneHacked)
        {
            //On ne pourra plus pirater le drone de nouveau
            isDroneHacked = true;
            //On fait apparaitre l'empty "Conduit" qui contient tous les éléments en rapport avec le conduit et le drone
            vent.SetActive(true);
            //On joue l'audio "Drone piraté !"
            dronePirateAudioSource.Play();
        }
    }
}
