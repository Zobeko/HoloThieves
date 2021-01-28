using Holo;

//HoloBehaviour rattaché au drone
public class DroneMovementBehaviour2 : HoloBehaviour
{
    //HoloTransform du drone
    [Serialized] private readonly HoloTransform transform = null;
    //Vitesse maximale que le drone peut avoir
    [Serialized] private readonly float maxSpeed = 0f;

    private HoloVector3 speed;

    //Distance de detection des checkPoints
    [Serialized] private float detectionDistance = 0f;


    //Tableau contenant tous les HoloGameObject des points de changement de direction pour le drone 
    [Serialized] private readonly HoloGameObject[] checkPoints;

    //Tableau contenant les VentCheckPointsBehaviours de chaque checkPoint
   // [Serialized]private VentCheckPointsBehaviour[] checkPointsBehaviours /*= new VentCheckPointsBehaviour[13]*/;
    

    //AudioSource sur le drone permettant de jouer le son de victoire à la fin du labyrinthe/conduit
    [SharedAudioComponent] private readonly SharedAudioComponent victorySoundAudioSource;

    //true si le drone à finit le labyrinthe/conduit
    private bool isVentFinished = false;
    //Timer qui va permettre de créer un pause de 'delayBeforeVentDisseppearance' avant la disparition du conduit (et l'apparition du IDServeur)
    private int tempsAvantDisparition = 0;
    //Empty "Conduit" contenant tout ce qui est en rapport avec le labyrinthe/conduit
    [Serialized] private readonly HoloGameObject vent;
    //HoloGameObject de l'IDServeur
    [Serialized] private readonly HoloGameObject IDSeveurText;
    [Serialized] private HoloGameObject panneauIndiceCode;
    [Serialized] private HoloGameObject panneauWifi;

    private bool isDroneTCPConnectionTrue = false;
        
    //Durée du delay avant la disparition du conduit (et apparition IDServeur)
    [Serialized] private readonly float delayBeforeVentDisseppearance;

    private int numeroDuCurrentChechpoint = 0;

    [Serialized] private HoloGameObject tcpHandler;
    private TCPHandler tcpHandlerScript;


    


    public override void Start()
    {
        //Log(transform.forward.ToString());

        //Log("Start DroneMovement");
        //Log("numeroDuCurrentChechpoint start = " + numeroDuCurrentChechpoint.ToString());
        Async.OnUpdate += Update;

        tcpHandlerScript = (TCPHandler)tcpHandler.GetBehaviour("TCPHandler");

        

        /*for (int i = 0; i < 13; i++)
        {
            
            checkPointsBehaviours[i] = (VentCheckPointsBehaviour)checkPoints[i].GetBehaviour("VentCheckPointsBehaviour");
            Log(checkPointsBehaviours[i].speed.ToString());
        }*/
    }

    public void Update()
    {
        //Log("Update DroneMovement");
        //Si le labyrinthe/conduit est affiché 
        if (vent.activeSelf)
        {
            if (!isDroneTCPConnectionTrue)
            {
                tcpHandlerScript.droneTCPConnexion = true;
                isDroneTCPConnectionTrue = true;

                
            }

            //Log("if(vent.activeSelf)");
            //Gestion des changement de currentCheckPoint lorsque le drone est assez proche d'un checkPoint[i]
            CheckPointSubstitution();

            //Gestion des déplacements du drone en fonction du currentCheckPoint
            DroneMovement();

            //Log(tcpHandlerScript.droneTCPConnexion.ToString());
            
        }
    }

    public void DroneMovement()
    {
        speed = new HoloVector3(0, 0, 0);
        

        //xPos
        if((numeroDuCurrentChechpoint == 0) || (numeroDuCurrentChechpoint==2) || (numeroDuCurrentChechpoint==4) || (numeroDuCurrentChechpoint==10))
        {
            float xSpeed = tcpHandlerScript.xDrone;
            if (xSpeed < -0.2f)
            {
                //Log("X Positive");
                speed = transform.forward * xSpeed;
                //Log(xSpeed.ToString());
            }
        }
        //xNeg
        else if((numeroDuCurrentChechpoint==6) || (numeroDuCurrentChechpoint == 8))
        {
            float xSpeed = tcpHandlerScript.xDrone;
            //Log("X Negative");
            if (xSpeed > 0.2f)
            {
                //Log("X Positive");
                speed = transform.forward * xSpeed;
                //Log(xSpeed.ToString());
            }
        }
        //zPos
        else if ((numeroDuCurrentChechpoint == 1) || (numeroDuCurrentChechpoint == 7))
        {
            float ySpeed = tcpHandlerScript.yDrone;
            //Log("Y Positive");
            if (ySpeed > 0.2f)
            {
                speed = transform.right * ySpeed;
            }
        }
        //zNeg
        else if ((numeroDuCurrentChechpoint == 3) || (numeroDuCurrentChechpoint == 5) || (numeroDuCurrentChechpoint == 9) || (numeroDuCurrentChechpoint == 11)) 
        {
            float ySpeed = tcpHandlerScript.yDrone;
            //Log("Y Negative");
            if (ySpeed < -0.2f)
            {
                speed = transform.right * ySpeed;
            }
        }
        //FinishPoint
        
        if (numeroDuCurrentChechpoint == 12)
        {
            //Log("Finish");
            

            //Si c'est la première fois qu'on finit le labyrinthe (évite que ces actions puissent être répétées plusieurs fois)
            if (!isVentFinished)
            {
                tcpHandlerScript.droneTCPConnexion = false;
                //On joue le son de victoire
                victorySoundAudioSource.Play();
                //On dit qu'on a finit le labyrinthe (important pour la boucle if juste au dessus) 
                isVentFinished = true;
            }

            
            //On incrémente le timer 
            tempsAvantDisparition++;
            

            //Gestion du delais avant dispartion conduit
            if ((tempsAvantDisparition / 60) >= delayBeforeVentDisseppearance)
            {
                //IDSeveurText.SetActive(true);
                panneauWifi.SetActive(false);
                panneauIndiceCode.SetActive(true);
                vent.SetActive(false);
            }

        }

        //On déplace le drone 
        //Log(numeroDuCurrentChechpoint.ToString());


        transform.position +=  speed * maxSpeed * TimeHelper.deltaTime;
    }

    public void CheckPointSubstitution()
    {
        //On teste sur tous les checkPoints 

        //Si le drone est à moins de 1m d'un checkPoint
        if (numeroDuCurrentChechpoint < 12)
        {
            //Log("numeroDuCurrentChechpoint = " + numeroDuCurrentChechpoint.ToString());
            if ((HoloVector3.Distance(transform.position, checkPoints[numeroDuCurrentChechpoint + 1].transform.position) < detectionDistance))
            {
                //Log("Transform");
                //Ce checkPoint devient le currentCheckPoint
                numeroDuCurrentChechpoint++;
            }

            
            /*if ((HoloVector3.Distance(transform.position, checkPoint1.transform.position) < detectionDistance))
            {

                //Ce checkPoint devient le currentCheckPoint
                //currentCheckPoint = checkPoints[n + 1];
                n = 1;
            }
            if ((HoloVector3.Distance(transform.position, checkPoint2.transform.position) < detectionDistance))
            {

                //Ce checkPoint devient le currentCheckPoint
                //currentCheckPoint = checkPoints[n + 1];
                n = 2;
            }*/
        }
        


    }

    

   



}
