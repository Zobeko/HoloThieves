using Holo;
using System;
using System.Globalization;


public class TCPHandler : HoloBehaviour
{
    [TCPComponent] private TCPComponent tcpComponent;
    //True si on veut que le server TCP nous envoie les valeurs du drone
    public bool droneTCPConnexion;

    //True si on veut que le server TCP nous envoie les valeurs du digicode
    private bool keypadTCPConnexion = false;

    //True si on veut que le server TCP nous envoie les valeurs du water level sensor (capteur de niveau d'eau)
    private bool waterSensorTcpConnexion = false;

    //True si on veut que le server TCP nous envoie les valeurs du bouton réel
    private bool buttonTCPConnexion = false;

    //True si on veut que le server TCP nous envoie les valeurs du capteur RFID
    private bool rfidTCPConnexion = false;

    //true si on s'est déjà connecté une fois au drone (on a envoyé "read_joystick") (sert à vérifier qu'on n'envoie pas plusieurs fois le message)
    private bool aiJeLeDroitDroneConnection = true;

    // true si on s'est déjà déconnecté du drone (on a envoyé "stop_read_joystick") (sert à vérifier qu'on n'envoie pas plusieurs fois le message)
    private bool aiJeLeDroitDroneDisconnection = false;


    //True si on veut que le client TCP se connecte au server
    [Serialized] private bool TCPConnected = true;

    //Valeurs en X et Y du joystick
    public float xDrone;
    public float yDrone;

    //Ensemble d'AudioSources permettant de déclencher les divers sons du jeu
    [SharedAudioComponent]private SharedAudioComponent AlarmAudioSource;
    [SharedAudioComponent] private SharedAudioComponent powerDownAudioSource;
    [SharedAudioComponent] private SharedAudioComponent codeBonAudioSource;
    [SharedAudioComponent] private SharedAudioComponent rfidAudioSource;

    //GameObjet représentant une AudioSource
    [Serialized] private HoloGameObject courantRetablitAudioSource;

    //Panneau d'indice qui apparait apres que le labyrinthe ait été terminé
    [Serialized] private HoloGameObject clueBoard;
    //Feedback de la zone wifi
    [Serialized] private HoloGameObject wifiZone;
    //Boule rouge permettant d'indiqué où est le water sensor
    [Serialized] private HoloGameObject surbrillanceWaterSensor;
    //Panneau de téléchargement des données qui apparait apres que le capteur RFID ait été activé
    [Serialized] private HoloGameObject finalDownloadPanel;
    //WIFI, si la wifi est setActive(false), alors on a finit l'epérience et on peut se déconnecter du serveur TCP
    [Serialized] private HoloGameObject wifi;

    //Bouton virtuel permettant de réactiver le courant
    [SharedAnimatorComponent] private SharedAnimatorComponent virtualButtonE3Animator;

    //Permet de savoir si on a déja demandé au RFID sa valeur
    private bool isRFIDMessageSent = false;
    //Permet de savoir si on a déja demandé à se déconnecter du serveur
    private bool isDisconnectionMessageSent = false;

    //Permet d'être sur que l'on ne lance le timer de l'alarme sonore une seule fois (sinon redondance des sons d'alarme)
    private bool timerAlarmON = false;
    //Delais avant que le son d'alarme se lance
    [Serialized] private float timerAlarmDelay;


    //[Serialized] private HoloGameObject cube;
    public override void Start()
    {
        //Permet d'utiliser la méthode Update() dans HoloScene
        Async.OnUpdate += Update;

        //Permet d'utiliser la méthode OnStringReceived() définie plus bas
        tcpComponent.OnStringReceived += OnStringReceived;
        
        //On se connecte au serveur TCP
        tcpComponent.Connect(OnConnected);

    }

    

    public void Update()
    {
        //Si je ne me suis pas encore connecté au drone et que je veux me connecter au drone
        if (droneTCPConnexion && aiJeLeDroitDroneConnection)
        {

            aiJeLeDroitDroneConnection = false;
            aiJeLeDroitDroneDisconnection = true;

            //On fait disparaitre le feedback de la zone wifi
            wifiZone.SetActive(false);
            //On envoie "read_joystick" au serveur TCP pour qu'il commence à envoyer les valeurs du drone au casque
            tcpComponent.Send("read_joystick");
            //Log("Send(read_joystick)");   
        }
        //Si on ne s'est pas encore déconnecté du drone et que l'on veut se déconnecter
        if (!droneTCPConnexion && aiJeLeDroitDroneDisconnection)
        {
            aiJeLeDroitDroneDisconnection = false;
            //On envoie le message "stop_read_joystick" au serveur TCP, ce qui aura pour conséquence d'arreter de transmettre les valeurs du drone
            tcpComponent.Send("stop_read_joystick");
        }



        
        //Si on s'est déconnecté du drone, alors on se connecte au digicode
        if (!droneTCPConnexion)
        {
            keypadTCPConnexion = true;
        }

        //Si on a demandé à lancer le timer de l'alarme (Afin d'éviter d'appeler la méthode plusieurs fois)
        if (timerAlarmON)
        {
            //On invoque la méthode DeclenchementAlarme() apres timerAlarmDelay secondes
            Async.InvokeAfterSeconds(DeclenchementAlarme, timerAlarmDelay);
            timerAlarmON = false;
        }

        if (!wifi.activeSelf && !isDisconnectionMessageSent)
        {
            ServerDisconnection();
            isDisconnectionMessageSent = true;
        }
        

    }


    //Callback de la connection au serveur TCP
    private void OnConnected(TCPComponent _tcpComponent, bool _success)
    {
        if (_success)
        {
            Log("Tcp connected");
            TCPConnected = true;
        }
    }

    //Méthode appelée à chaque fois que l'on reçoit un message du serveur TCP
    public void OnStringReceived(TCPComponent _tcpComponent, string _string)
    {
        //Si on est dans l'énigme du drone
        if (droneTCPConnexion)
        {

            
            //On traite le string recu afin d'obtenir une valeur entre -1 et 1 pour les axes X et Y
            string[] temp = _string.Split('\n');
            string[] str = new string[2];
            str = temp[0].Split(';');

            float x, y;


            //On convertit le string reçu en float
            if (float.TryParse(str[0], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out x) && float.TryParse(str[1], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out y))
            {
                //x appartient à [0, 1], on veut xValue appartien à [-1; 1]
                xDrone = (x * 2) - 1;
                yDrone = (y * 2) - 1;
            }
            else
            {
                Log("Parse failure !");
            }
        }

        //Si on est dans l'énigme du digicode
        if(keypadTCPConnexion)
        {
            //On traite le string reçu afin d'en extraire l'information utile 
            string[] str = new string[2];
            str = _string.Split('_');
            
            //Si le bon code a été rentré
            if (str[0] == "Keypad")
            {
                //On lance le timer de l'alarme
                timerAlarmON = true;
                
                //On active le son "code bon"
                codeBonAudioSource.Play();

                //On désaffiche le panneau d'indice
                clueBoard.SetActive(false);

                //On désactive la connexion avec le kaypad
                keypadTCPConnexion = false;
            }
        }

        //Si on veut avoir les valeurs du water sensor
        if (waterSensorTcpConnexion)
        {
            
            //On traite le message reçu
            string[] str = new string[2];
            str = _string.Split('_');

            //Si le xater sensor a été plongé dans l'ordre
            if (str[0] == "Water")
            {
                //On fait disparaitre l'aide du water sensor
                surbrillanceWaterSensor.SetActive(false);

                AlarmAudioSource.Stop();
                powerDownAudioSource.Play();
                //On dit que l'on souhaite se connecter au bouton réel
                buttonTCPConnexion = true;

                //On se déconnecte du water sensor
                waterSensorTcpConnexion = false;
            }
        }

        //Si on est connecté au bouton réel
        if (buttonTCPConnexion)
        {
            
            string[] str = new string[2];
            str = _string.Split('_');
            
            if(str[0] == "Button" && virtualButtonE3Animator.GetBoolParameter("isVirtualButtonPressed"))
            {
                

                //Log("Button pressed !");

                rfidTCPConnexion = true;
                tcpComponent.Send("read_RFID");
                isRFIDMessageSent = true;
                courantRetablitAudioSource.SetActive(true);

                buttonTCPConnexion = false;
                //virtualButtonE3.SetActive(true);

            }
        }

        //Si on est connecté au capteru RFID
        if (rfidTCPConnexion)
        {
            //On traite les données reçues
            string[] str = new string[2];
            str = _string.Split('_');
            //Si le badge a été passé sur le capteur RFID
            if (str[0] == "RFID")
            {
                rfidAudioSource.Play();
                finalDownloadPanel.SetActive(true);
                
                rfidTCPConnexion = false;
            }
        }
       
    }

    //Méthode de déclenchement de l'alarme sonore
    private void DeclenchementAlarme()
    {
        Log("timer alarm !");
        AlarmAudioSource.Play();
        waterSensorTcpConnexion = true;
        
        surbrillanceWaterSensor.SetActive(true);
    }

    public void ServerDisconnection()
    {
        Log("Disconnection");
        tcpComponent.Send("disconnection");
    }

}
