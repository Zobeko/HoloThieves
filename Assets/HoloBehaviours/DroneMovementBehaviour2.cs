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
    [Serialized] private readonly HoloTransform[] checkPointsTransform;
    //HoloGameObject du point de départ du drone dans le conduit
    [Serialized] private readonly HoloGameObject startPoint;
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


    public override void Start()
    {
        
        Async.OnUpdate += Update;

        currentCheckPoint = startPoint;
    }

    public void Update()
    {
        //Si le labyrinthe/conduit est affiché 
        if (vent.activeSelf)
        {
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
        if((currentCheckPoint == startPoint) || (currentCheckPoint == checkPoints[1]) || (currentCheckPoint == checkPoints[3]) || (currentCheckPoint == checkPoints[9]))
        {
            speed.x = 1f;
        }
        //xNeg
        if((currentCheckPoint == checkPoints[5]) || (currentCheckPoint == checkPoints[7]))
        {
            speed.x = -1f;
        }
        //zPos
        if ((currentCheckPoint == checkPoints[0]) || (currentCheckPoint == checkPoints[6]))
        {
            speed.z = 1f;
        }
        //zNeg
        if ((currentCheckPoint == checkPoints[2]) || (currentCheckPoint == checkPoints[4]) || (currentCheckPoint == checkPoints[8]) || (currentCheckPoint == checkPoints[10])) 
        {
            speed.z = -1f;
        }
        //FinishPoint
        if(currentCheckPoint == checkPoints[11])
        {
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
            Log((tempsAvantDisparition / 60f).ToString());
        }

        //On déplace le drone 
        transform.position += speed * maxSpeed * TimeHelper.deltaTime;
    }

    public void CheckPointSubstitution()
    {
        //On teste sur tous les checkPoints 
        for (int i = 0; i < checkPoints.Length; i++)
        {
            //Si le drone est à moins de 1m d'un checkPoint
            if (HoloVector3.Distance(transform.position, checkPointsTransform[i].position) < detectionDistance)
            {
                //Ce checkPoint devient le currentCheckPoint
                currentCheckPoint = checkPoints[i];
            }
        }


    }

    

   



}
