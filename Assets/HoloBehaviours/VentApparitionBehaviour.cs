using Holo;

public class VentApparitionBehaviour : HoloBehaviour
{
    [UserPositionTrigger] private UserPositionTriggerComponent userPositionTrigger;

    [Serialized] private HoloGameObject ventPart;


    // Start is called before the first frame update
    public override void Start()
    {
        userPositionTrigger.OnUserEnter += OnUserEnter;
        userPositionTrigger.OnUserExit += OnUserExit;
    }

    public void OnUserEnter()
    {
        Log("Enter !");
        ventPart.SetActive(true);

    }

    public void OnUserExit()
    {
        Log("Exit !");
        ventPart.SetActive(false);

    }
}
