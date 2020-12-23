using Holo;

public class ObjectFollowsCamera : HoloBehaviour
{
    [EaseRecenterComponent]
    private EaseRecenterComponent easeRecenter;






    public override void Start()
    {

        Async.OnUpdate += Update;

        //userPositionTrigger.OnUserEnter += OnUserEnter;
        //userPositionTrigger.OnUserExit += OnUserExit;

        easeRecenter.StartMoving();
        
    }

    public void Update()
    {
        

        /*if ((HoloVector3.Distance(transform.position, cameraTransform.position) >= 5) && !easeRecenter.IsMoving)
        {
            easeRecenter.StartMoving();
            
        }
        else if((HoloVector3.Distance(transform.position, cameraTransform.position) < 5) && easeRecenter.IsMoving)
        {
            easeRecenter.StopMoving();
        }*/
    }


    /*public void OnUserEnter()
    {
        easeRecenter.StopMoving();
    }

    public void OnUserExit()
    {
        easeRecenter.StartMoving();
    }*/


}
