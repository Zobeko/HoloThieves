using Holo;


public class TCPHandler : HoloBehaviour
{
    [TCPComponent] private TCPComponent tcpComponent;
    //True si on veut que le server TCP nous envoie les valeurs du drone
    public bool droneTCPConnexion;
    //true si on s'est déjà connecté une fois au drone (on a envoyé "read_joystick")
    private bool aiJeLeDroitDroneConnection = true;
    //true si on s'est déjà déconnecté du server (on a envoyé "Disconnection")
    private bool aiJeLeDroitTCPDisconnection = true;
    // true si on s'est déjà déconnecté du drone (on a envoyé "stop_read_joystick")
    private bool aiJeLeDroitDroneDisconnection = false;


    //True si on veut que le client TCP se connecte au server
    [Serialized] private bool TCPConnected = true;

    [Serialized] private float maxSpeed;

    public float xDrone;
    public float yDrone;

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
            tcpComponent.Send("read_joystick");
            
        }
        if (!droneTCPConnexion && aiJeLeDroitDroneDisconnection)
        {
            aiJeLeDroitDroneDisconnection = false;
            tcpComponent.Send("stop_read_joystick");
            
        }



        if (!TCPConnected && aiJeLeDroitTCPDisconnection)
        {
            Disconnection();
            aiJeLeDroitTCPDisconnection = false;
        }


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

        //if (droneTCPConnexion)
        //{

        //Log(_string);

        string[] temp = _string.Split('\n');
        //Log(temp[0]);
        string[] str = new string[2];
        str = temp[0].Split(';');
        

        //Log(str[0]);
        //Log(str[1]);

        int x, y;

        //On convertit le string reçu en float
        if (int.TryParse(str[0], out x) && int.TryParse(str[1], out y))
        {
            //x appartient à [0, 1024[, on veut xValue appartien à [-1; 1]
            xDrone = ((float)x / 511.5f) - 1f;
            yDrone = ((float)y / 511.5f) - 1f;

            //Log(xDrone.ToString());
            //Log(yDrone.ToString());
        }
        else
        {
            Log("Parse failure !");

        }
        /*HoloVector3 speed = new HoloVector3(xDrone, 0, yDrone);

        cube.transform.position += speed * maxSpeed * TimeHelper.deltaTime;*/

        





    }

    public void Disconnection()
    {
        tcpComponent.Send("disconnection");

        Log("Disconnected");

        tcpComponent.Disconnect();
    }



}
