using Holo;
using System;
using System.Globalization;


public class TCPHandler : HoloBehaviour
{
    [TCPComponent] private TCPComponent tcpComponent;
    //True si on veut que le server TCP nous envoie les valeurs du drone
    public bool droneTCPConnexion;

    private bool keypadTCPConnexion = false;

    private bool waterSensorTcpConnexion = false;

    private bool buttonTCPConnexion = false;

    private bool rfidTCPConnexion = false;

    //true si on s'est déjà connecté une fois au drone (on a envoyé "read_joystick")
    private bool aiJeLeDroitDroneConnection = true;

    // true si on s'est déjà déconnecté du drone (on a envoyé "stop_read_joystick")
    private bool aiJeLeDroitDroneDisconnection = false;


    //True si on veut que le client TCP se connecte au server
    [Serialized] private bool TCPConnected = true;


    public float xDrone;
    public float yDrone;

    [SharedAudioComponent]private SharedAudioComponent AlarmAudioSource;
    [SharedAudioComponent] private SharedAudioComponent powerDownAudioSource;
    [SharedAudioComponent] private SharedAudioComponent codeBonAudioSource;
    [SharedAudioComponent] private SharedAudioComponent rfidAudioSource;

    [Serialized] private HoloGameObject clueBoard;
    [Serialized] private HoloGameObject wifiZone;
    [Serialized] private HoloGameObject surbrillanceWaterSensor;
    [Serialized] private HoloGameObject finalDownloadPanel;
    [SharedAnimatorComponent] private SharedAnimatorComponent virtualButtonE3Animator;
    private bool isRFIDMessageSent = false;

    [Serialized] private HoloGameObject courantRetablitAudioSource;

    private bool timerAlarmON = false;
    [Serialized] private float timerAlarmDelay;


    //[Serialized] private HoloGameObject cube;
    public override void Start()
    {
        base.Start();

        Async.OnUpdate += Update;
        tcpComponent.OnStringReceived += OnStringReceived;
        

        tcpComponent.Connect(OnConnected);

        

    }

    

    public void Update()
    {
        if (droneTCPConnexion && aiJeLeDroitDroneConnection)
        {
            aiJeLeDroitDroneConnection = false;
            aiJeLeDroitDroneDisconnection = true;
            wifiZone.SetActive(false);
            tcpComponent.Send("read_joystick");
            //Log("Send(read_joystick)");
            
        }
        if (!droneTCPConnexion && aiJeLeDroitDroneDisconnection)
        {
            aiJeLeDroitDroneDisconnection = false;
            tcpComponent.Send("stop_read_joystick");
            
        }



        

        if (!droneTCPConnexion)
        {
            keypadTCPConnexion = true;
        }

        if (timerAlarmON)
        {
            Async.InvokeAfterSeconds(DeclenchementAlarme, timerAlarmDelay);
            timerAlarmON = false;
        }
        

        /*if (virtualButtonE3Animator.GetBoolParameter("isVirtualButtonPressed") && !isRFIDMessageSent)
        {
            

            rfidTCPConnexion = true;
            tcpComponent.Send("read_RFID");
            isRFIDMessageSent = true;
            courantRetablitAudioSource.SetActive(true);
        }*/

        

    }



    private void OnConnected(TCPComponent _tcpComponent, bool _success)
    {
        if (_success)
        {
            Log("Tcp connected");
            TCPConnected = true;
        }
    }

    public void OnStringReceived(TCPComponent _tcpComponent, string _string)
    {

        if (droneTCPConnexion)
        {

            //Log(_string);

            string[] temp = _string.Split('\n');
            //Log(temp[0]);
            string[] str = new string[2];
            str = temp[0].Split(';');


            //Log(str[0]);
            //Log(str[1]);



            float x, y;


            //On convertit le string reçu en float
            if (float.TryParse(str[0], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out x) && float.TryParse(str[1], System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out y))
            {
                //x appartient à [0, 1], on veut xValue appartien à [-1; 1]
                xDrone = (x * 2) - 1;
                yDrone = (y * 2) - 1;

                //Log(xDrone.ToString());
                //Log(yDrone.ToString());
            }
            else
            {
                Log("Parse failure !");
            }
        }

        //Log(_string);
        //Log(keypadTCPConnexion.ToString());
        if(keypadTCPConnexion)
        {
            //string[] temp = _string.Split('\n');
            //Log(temp[0]);
            string[] str = new string[2];
            str = _string.Split('_');
            //Log(str[0]);

            if (str[0] == "Keypad")
            {
                timerAlarmON = true;
                
                codeBonAudioSource.Play();
                clueBoard.SetActive(false);

                

                keypadTCPConnexion = false;
            }
        }
        //Log(waterSensorTcpConnexion.ToString());
        if (waterSensorTcpConnexion)
        {
            //Log("blabla");
            string[] str = new string[2];
            str = _string.Split('_');
            if (str[0] == "Water")
            {
                surbrillanceWaterSensor.SetActive(false);
                AlarmAudioSource.Stop();
                powerDownAudioSource.Play();
                buttonTCPConnexion = true;
                waterSensorTcpConnexion = false;
                //Log("Alarme désactivée !");
            }
        }

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

        if (rfidTCPConnexion)
        {
            string[] str = new string[2];
            str = _string.Split('_');
            if (str[0] == "RFID")
            {
                rfidAudioSource.Play();
                finalDownloadPanel.SetActive(true);
                //Log("RFID !");
                rfidTCPConnexion = false;
            }
        }
       
        

        
        /*HoloVector3 speed = new HoloVector3(xDrone, 0, yDrone);

        cube.transform.position += speed * maxSpeed * TimeHelper.deltaTime;*/

        





    }


    private void DeclenchementAlarme()
    {

        

        
        Log("timer alarm !");
        AlarmAudioSource.Play();
        waterSensorTcpConnexion = true;
        
        surbrillanceWaterSensor.SetActive(true);

        

    }


    public override void OnDestroy()
    {
        tcpComponent.Send("disconnection");

        Log("Disconnected");

        tcpComponent.Disconnect();
    }



}
