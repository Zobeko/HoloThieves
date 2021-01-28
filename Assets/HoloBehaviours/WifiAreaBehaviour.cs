using Holo;
using System;

//Script rattaché à la borne wifi (qui gère la plupart des interactions liées à la borne wifi)

public class WifiAreaBehaviour : HoloBehaviour
{
    //Composant permettant de savoir quand le joueur est dans la zone WIFI
    [UserPositionTrigger] private UserPositionTriggerComponent userPositionComponent;
    //Animator du panel gérant apparition et disparition du logo WIFI quand on entre et sort de la zone WIFI (feedback zone WIFI)
    //[SharedAnimatorComponent] private SharedAnimatorComponent wifiUIAnimator;
    //Animator de la borne WIFI
    [SharedAnimatorComponent] private SharedAnimatorComponent wifiAnimator;

    [SharedAnimatorComponent] private SharedAnimatorComponent downloadBarAnimator;

    [SharedAudioComponent] private SharedAudioComponent vousEtesConnecteALaWifiAudioSource;

    [Serialized] private HoloGameObject panneauIndiceCode;

    [Serialized] private HoloGameObject conduit;

    //HoloGameObject de l'interface WIFI
    [Serialized] private HoloGameObject wifiBoard;

    [Serialized] private HoloGameObject feedbackZoneWifi;
    

    //true quand le bouton WIFI est appuyé
    private bool isButtonTrigger = false;
    //true quand la WIFI est activée pour la 1ère fois 
    private bool isWifiActivated = false;

    private bool isFeedbackZoneWifiActivated = false;
    //Timer gérant le delais de connexion à la WIFI
    private float timerWifiBoard = 0f;

    private bool isWifiBoardActivated = false;
    //private bool isDroneHacked = false;
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
        if (isButtonTrigger && !isWifiBoardActivated && isInWifiArea && !isWifiActivated)
        {

            
            //On incrémente le timer
            timerWifiBoard += (1 / 60f);
            //Log(timerWifiBoard.ToString());

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
                vousEtesConnecteALaWifiAudioSource.Play();
                isWifiBoardActivated = true;
                
                //On dit que la WIFI a été activé (pour ne pas pouvoir répeter cette action)
                isWifiActivated = true;
            }
        }

        if (isWifiActivated && !isFeedbackZoneWifiActivated)
        {
            feedbackZoneWifi.SetActive(true);
            isFeedbackZoneWifiActivated = true;
        }
    }



    //Appelée quand le joueur entre dans la zone WIFI
    public void OnUserEnter()
    {
        Log("IN");

        isInWifiArea = true;
        //wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);

        if (conduit.activeSelf)
        {
            downloadBarAnimator.SetBoolParameter("isDroneHacked", true);
        }

        if (isWifiBoardActivated && !panneauIndiceCode.activeSelf )
        {
            wifiBoard.SetActive(true);
        }
        
    }


    //Appelée quand le joueur sort de la zone WIFI
    public void OnUserExit()
    {
        Log("OUT");
        isInWifiArea = false;
        //On envoie isInWifiArea à l'Animator du pannel de feedback zone WIFI
        //wifiUIAnimator.SetBoolParameter("isEnabled", isInWifiArea);
        wifiBoard.SetActive(false);
        
    }



}
