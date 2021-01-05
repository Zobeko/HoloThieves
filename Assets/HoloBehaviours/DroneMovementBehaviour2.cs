using Holo;

//HoloBehaviour rattaché au drone
public class DroneMovementBehaviour2 : HoloBehaviour
{
    //HoloTransform du drone
    [Serialized] private readonly HoloTransform transform = null;
    //Vitesse maximale que le drone peut avoir
    [Serialized] private readonly float maxSpeed = 0f;
    //Vitesse normalisée du drone à chaque Update
    [Serialized] private HoloVector3 speed = new HoloVector3(0, 0, 0);

    //Distance de detection des checkPoints
    [Serialized] private float detectionDistance = 0f;


    //Tableau contenant tous les HoloGameObject des points de changement de direction pour le drone 
    [Serialized] private readonly HoloGameObject[] checkPoints;
    //Tableau contenant tous les HoloTransform des points de changement de direction pour le drone 
    //[Serialized] private readonly HoloTransform[] checkPointsTransform;

    //HoloGameObject du point de départ du drone dans le conduit
    //[Serialized] private readonly HoloGameObject startPoint;

    //AudioSource sur le drone permettant de jouer le son de victoire à la fin du labyrinthe/conduit
    [SharedAudioComponent] private readonly SharedAudioComponent victorySoundAudioSource;

    
    //Le point de changement de direction du drone actuel
    [Serialized] private HoloGameObject currentCheckPoint;
    //true si le drone à finit le labyrinthe/conduit
    private bool isVentFinished = false;
    //Timer qui va permettre de créer un pause de 'delayBeforeVentDisseppearance' avant la disparition du conduit (et l'apparition du IDServeur)
    private int tempsAvantDisparition = 0;
    //Empty "Conduit" contenant tout ce qui est en rapport avec le labyrinthe/conduit
    [Serialized] private readonly HoloGameObject vent;
    //HoloGameObject de l'IDServeur
    [Serialized] private readonly HoloGameObject IDSeveurText;
    //Durée du delay avant la disparition du conduit (et apparition IDServeur)
    [Serialized] private readonly float delayBeforeVentDisseppearance;

    private int n = 0;

    
    [Serialized] private HoloGameObject checkPoint1;
    [Serialized] private HoloGameObject checkPoint2;

    public override void Start()
    {
        Log("Start DroneMovement");
        Log("n start = " + n.ToString());
        Async.OnUpdate += Update;

        currentCheckPoint = checkPoints[0];
    }

    public void Update()
    {
        Log("Update DroneMovement");
        //Si le labyrinthe/conduit est affiché 
        if (vent.activeSelf)
        {
            Log("if(vent.activeSelf)");
            //Gestion des changement de currentCheckPoint lorsque le drone est assez proche d'un checkPoint[i]
            CheckPointSubstitution();

            //Gestion des déplacements du drone en fonction du currentCheckPoint
            DroneMovement();

            //Gestion du delais avant dispartion conduit
            if ((tempsAvantDisparition / 60) >= delayBeforeVentDisseppearance)
            {
                IDSeveurText.SetActive(true);
                vent.SetActive(false);
            }

            
        }
    }

    public void DroneMovement()
    {
        speed = new HoloVector3(0, 0, 0);
        

        //xPos
        if((n == 0) || (n==2) || (n==4) || (n==10))
        {
            Log("X Positive");
            speed.x = 1f;
        }
        //xNeg
        else if((n==6) || (n == 8))
        {
            Log("X Negative");
            speed.x = -1f;
        }
        //zPos
        else if ((n == 1) || (n == 7))
        {
           Log("Y Positive");
            speed.z = 1f;
        }
        //zNeg
        else if ((n == 3) || (n == 5) || (n == 9) || (n == 11)) 
        {
            Log("Y Negative");
            speed.z = -1f;
        }
        //FinishPoint
        else if (n == 12)
        {
            Log("Finish");
            //Si on est à la fin du labyrinthe, on rend le drone immobile 
            speed = new HoloVector3(0, 0, 0);
            //Si c'est la première fois qu'on finit le labyrinthe (évite que ces actions puissent être répétées plusieurs fois
            if (!isVentFinished)
            {
                //On joue le son de victoire
                victorySoundAudioSource.Play();
                //On dit qu'on a finit le labyrinthe (important pour la boucle if juste au dessus) 
                isVentFinished = true;
            }

            
            //On incrémente le timer 
            tempsAvantDisparition++;
            //Log((tempsAvantDisparition / 60f).ToString());
        }

        //On déplace le drone 
        transform.position += speed * maxSpeed * TimeHelper.deltaTime;
    }

    public void CheckPointSubstitution()
    {
        //On teste sur tous les checkPoints 

        //Si le drone est à moins de 1m d'un checkPoint
        if (n < 12)
        {
            Log("n = " + n.ToString());
            if ((HoloVector3.Distance(transform.position, checkPoints[n + 1].transform.position) < detectionDistance))
            {
                Log("Transform");
                //Ce checkPoint devient le currentCheckPoint
                //currentCheckPoint = checkPoints[n + 1];
                n++;
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
