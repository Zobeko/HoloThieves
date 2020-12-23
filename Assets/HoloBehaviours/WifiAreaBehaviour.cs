
using Holo;

public class WifiAreaBehaviour : HoloBehaviour
{
    [UserPositionTrigger]
    private UserPositionTriggerComponent userPositionComponent;

    [SharedAnimatorComponent] private SharedAnimatorComponent wifiUIAnimator;
    [SharedAnimatorComponent] private SharedAnimatorComponent wifiAnimator;


    [Serialized] private HoloGameObject wifi;

    [Serialized] private bool isButtonTrigger = false;

    private float timerWifiBoard = 0f;
    [Serialized] private float delayBeforeWifiBoardApparition;


    public bool isInWifiArea = false;

    public override void Start()
    {
        base.Start();

        userPositionComponent.OnUserEnter += OnUserEnter;
        userPositionComponent.OnUserExit += OnUserExit;
        Async.OnUpdate += Update;
    }

    public void Update()
    {
        isButtonTrigger = wifiAnimator.GetBoolParameter("ButtonIsTrigger");
        Log(isButtonTrigger.ToString());
        if (isButtonTrigger && !wifi.activeSelf)
        {
            timerWifiBoard += 1 / 60f;
            Log(timerWifiBoard.ToString());

            if ((timerWifiBoard >= delayBeforeWifiBoardApparition) && isInWifiArea)
            {

                wifi.SetActive(true);
            }
        }
    }




    public void OnUserEnter()
    {

        Log("Vous êtes dans la zone WIFI");
        isInWifiArea = true;
        Log(isInWifiArea.ToString());
        wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);

        
    }

    public void OnUserExit()
    {
        Log("Vous sortez de la zone WIFI");
        isInWifiArea = false;
        Log(isInWifiArea.ToString());
        wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);
        wifi.SetActive(false);
    }



}
