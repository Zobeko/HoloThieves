using Holo;
using System;
using System.Collections;

//HoloBehaviour rattaché au drone
public class DroneMovementBehaviour2 : HoloBehaviour
{
    //HoloTransform du drone
    [Serialized] private readonly HoloTransform transform = null;
    //Vitesse maximale que le drone peut avoir
    [Serialized] private readonly float maxSpeed = 0f;
    //Vitesse du drone
    private HoloVector3 speed;

    //Distance de detection des checkPoints
    [Serialized] private float detectionDistance = 0f;


    //Tableau contenant tous les HoloGameObject des points de changement de direction pour le drone 
    [Serialized] private readonly HoloGameObject[] checkPoints;

    //AudioSource sur le drone permettant de jouer le son de victoire à la fin du labyrinthe/conduit
    [SharedAudioComponent] private readonly SharedAudioComponent victorySoundAudioSource;

    //true si le drone à finit le labyrinthe/conduit
    private bool isVentFinished = false;

    //Empty "Conduit" contenant tout ce qui est en rapport avec le labyrinthe/conduit
    [Serialized] private readonly HoloGameObject vent;

    //Panneau d'indice pour le code du digicode
    [Serialized] private HoloGameObject panneauIndiceCode;
    //Interface de piratage du drone
    [Serialized] private HoloGameObject panneauWifi;

    //Permet de savoir si la connection avec le joystick est établie ou non 
    private bool isDroneTCPConnectionTrue = false;
        
    //Durée du delay avant la disparition du conduit (et apparition IDServeur)
    [Serialized] private readonly float delayBeforeVentDisseppearance;

    private int numeroDuCurrentChechpoint = 0;

    [Serialized] private HoloGameObject tcpHandler;
    private TCPHandler tcpHandlerScript;

    [Serialized] private HoloGameObject croixIndicationDirections;


    


    public override void Start()
    {

        Async.OnUpdate += Update;

        //On récupère le script "TCPHandler" associé au GameObject du même nom
        tcpHandlerScript = (TCPHandler)tcpHandler.GetBehaviour("TCPHandler");

        


    }

    public void Update()
    {

        //Si le labyrinthe/conduit est affiché 
        if (vent.activeSelf)
        {
            //Si on n'est pas connecté au joystick via le serveur TCP (permet d'appeler une seule fois ce qui suit)
            if (!isDroneTCPConnectionTrue)
            {
                //On lance le timer d'apparition de la croix indiquant l'orientation du labyrinthe/conduit
                Async.InvokeAfterSeconds(IndicationApparition, 50f);

                tcpHandlerScript.droneTCPConnexion = true;
                isDroneTCPConnectionTrue = true;

                
            }


            //Gestion des changement de currentCheckPoint lorsque le drone est assez proche d'un checkPoint[i]
            CheckPointSubstitution();

            //Gestion des déplacements du drone en fonction du currentCheckPoint
            DroneMovement();

            
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
                speed = transform.forward * xSpeed;
            }
        }
        //xNeg
        else if((numeroDuCurrentChechpoint==6) || (numeroDuCurrentChechpoint == 8))
        {
            float xSpeed = tcpHandlerScript.xDrone;
            //Log("X Negative");
            if (xSpeed > 0.2f)
            {

                speed = transform.forward * xSpeed;

            }
        }
        //zPos
        else if ((numeroDuCurrentChechpoint == 1) || (numeroDuCurrentChechpoint == 7))
        {
            float ySpeed = tcpHandlerScript.yDrone;

            if (ySpeed > 0.2f)
            {
                speed = transform.right * ySpeed;
            }
        }
        //zNeg
        else if ((numeroDuCurrentChechpoint == 3) || (numeroDuCurrentChechpoint == 5) || (numeroDuCurrentChechpoint == 9) || (numeroDuCurrentChechpoint == 11)) 
        {
            float ySpeed = tcpHandlerScript.yDrone;

            if (ySpeed < -0.2f)
            {
                speed = transform.right * ySpeed;
            }
        }

        //Si on est au poit final du labyrinthe (i.e. on a finit le labyrinthe)
        if (numeroDuCurrentChechpoint == 12)
        {

            

            //Si c'est la première fois qu'on finit le labyrinthe (évite que ces actions puissent être répétées plusieurs fois)
            if (!isVentFinished)
            {
                tcpHandlerScript.droneTCPConnexion = false;
                //On joue le son de victoire
                victorySoundAudioSource.Play();
                //On dit qu'on a finit le labyrinthe (important pour la boucle if juste au dessus) 
                isVentFinished = true;
            }


            //On lance le timer de disparition du labyrinthe + drone + indication
            Async.InvokeAfterSeconds(VentDisseppearance, delayBeforeVentDisseppearance);


            

        }

        //On déplace le drone 
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

                //Ce checkPoint devient le currentCheckPoint
                numeroDuCurrentChechpoint++;
            }

        }
        


    }

    //Méthode d'apparition de l'indice d'orientation du labyrinthe
    public void IndicationApparition()
    {
        Log("Timer indications drone !");
        croixIndicationDirections.SetActive(true);
        return;
    }

    //Méthode de disparrition du labyrinthe + drone + indication
    public void VentDisseppearance()
    {
        Log("timer disparition conduit !");

        //IDSeveurText.SetActive(true);
        panneauWifi.SetActive(false);
        panneauIndiceCode.SetActive(true);
        vent.SetActive(false);
    }






}
