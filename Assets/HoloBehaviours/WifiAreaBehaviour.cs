using Holo;

//Script rattaché à la borne wifi (qui gère la plupart des interactions liées à la borne wifi)

public class WifiAreaBehaviour : HoloBehaviour
{
    //Composant permettant de savoir quand le joueur est dans la zone WIFI
    [UserPositionTrigger] private UserPositionTriggerComponent userPositionComponent;
    //Animator du panel gérant apparition et disparition du logo WIFI quand on entre et sort de la zone WIFI (feedback zone WIFI)
    [SharedAnimatorComponent] private SharedAnimatorComponent wifiUIAnimator;
    //Animator de la borne WIFI
    [SharedAnimatorComponent] private SharedAnimatorComponent wifiAnimator;

    //HoloGameObject de l'interface WIFI
    [Serialized] private HoloGameObject wifiBoard;

    //true quand le bouton WIFI est appuyé
    private bool isButtonTrigger = false;
    //true quand la WIFI est activée pour la 1ère fois 
    private bool isWifiActivated = false;
    //Timer gérant le delais de connexion à la WIFI
    private float timerWifiBoard = 0f;
    //Delais de la connexion WIFI 
    [Serialized] private readonly float delayBeforeWifiBoardApparition;

    //HoloGameObject des 4 lumières vertes sur la borne WIFI
    [Serialized] private readonly HoloGameObject greenLight1;
    [Serialized] private readonly HoloGameObject greenLight2;
    [Serialized] private readonly HoloGameObject greenLight3;
    [Serialized] private readonly HoloGameObject greenLight4;

    //true lorsque le joueur est dans la zone WIFI
    public bool isInWifiArea = false;

    public override void Start()
    {
        userPositionComponent.OnUserEnter += OnUserEnter;
        userPositionComponent.OnUserExit += OnUserExit;
        Async.OnUpdate += Update;
    }

    public void Update()
    {
        //On récupère le booleen ButtonIsTrigger qui est à truq quand on appuie sur le bouton WIFI
        isButtonTrigger = wifiAnimator.GetBoolParameter("ButtonIsTrigger");
        //Si on appuie sur le bouton WIFI & l'interface WIFI n'est pas encore activée & on est dans la zone wifi & c'est la 1ère fois quon active la WIFI
        if (isButtonTrigger && !wifiBoard.activeSelf && isInWifiArea && !isWifiActivated)
        {
            //On incrémente le timer
            timerWifiBoard += (1 / 60f);

            //On fait passer au vert les lumières de la borne WIFI
            if ((timerWifiBoard >= delayBeforeWifiBoardApparition*0.25))
            {

                greenLight1.SetActive(true);

            }
            if ((timerWifiBoard >= delayBeforeWifiBoardApparition*0.5))
            {

                greenLight2.SetActive(true);

            }
            if ((timerWifiBoard >= delayBeforeWifiBoardApparition*0.75))
            {

                greenLight3.SetActive(true);

            }
            if ((timerWifiBoard >= delayBeforeWifiBoardApparition))
            {

                greenLight4.SetActive(true);

                //Quand les 4 lumières sont vertes, on active la WIFI (on fait apparaitre l'interface WIFI)
                wifiBoard.SetActive(true);
                //On dit que la WIFI a été activé (pour ne pas pouvoir répeter cette action)
                isWifiActivated = true;
            }
        }
    }



    //Appelée quand le joueur entre dans la zone WIFI
    public void OnUserEnter()
    {
        isInWifiArea = true;
        wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);

        
    }


    //Appelée quand le joueur sort de la zone WIFI
    public void OnUserExit()
    {
        isInWifiArea = false;
        //On envoie isInWifiArea à l'Animator du pannel de feedback zone WIFI
        wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);
        wifiBoard.SetActive(false);
    }



}
