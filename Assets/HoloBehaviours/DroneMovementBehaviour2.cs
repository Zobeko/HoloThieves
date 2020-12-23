using Holo;

public class DroneMovementBehaviour2 : HoloBehaviour
{
    [Serialized] private HoloTransform transform = null;

    [Serialized] private float maxSpeed = 0f;
    [Serialized] private HoloVector3 speed = new HoloVector3(0, 0, 0);


    [Serialized] private HoloGameObject drone;

    [Serialized] private HoloGameObject[] checkPoints;
    [Serialized] private HoloTransform[] checkPointsTransform;

    [Serialized] private HoloGameObject startPoint;

    [SharedAudioComponent] private SharedAudioComponent victorySoundAudioSource;

    

    [Serialized] private HoloGameObject currentCheckPoint;
    private bool isVentFinished = false;
    private int tempsAvantDisparition = 0;
    [Serialized] private HoloGameObject vent;
    [Serialized] private HoloGameObject IDSeveurText;
    [Serialized] private float delayBeforeVentDisseppearance;


    public override void Start()
    {
        base.Start();

        
        Async.OnUpdate += Update;
        

        currentCheckPoint = startPoint;

    }

    public void Update()
    {
        if (vent.activeSelf)
        {
            CheckPointSubstitution();

            DroneMovement();

            

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
            speed = new HoloVector3(0, 0, 0);
            if (!isVentFinished)
            {
                victorySoundAudioSource.Play();
                isVentFinished = true;
            }

            
            
            tempsAvantDisparition++;
            Log((tempsAvantDisparition / 60f).ToString());
        }


        transform.position += speed * maxSpeed * TimeHelper.deltaTime;
    }

    public void CheckPointSubstitution()
    {
        for (int i = 0; i < checkPoints.Length; i++)
        {
            if (HoloVector3.Distance(transform.position, checkPointsTransform[i].position) < 1f)
            {
                currentCheckPoint = checkPoints[i];
            }
        }


    }

    

   



}
