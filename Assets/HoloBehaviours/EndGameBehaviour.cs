using Holo;

public class EndGameBehaviour : HoloBehaviour
{
    //GO à faire disparaitre
    [Serialized] private HoloGameObject realWifi;
    [Serialized] private HoloGameObject fakeWifi1;
    [Serialized] private HoloGameObject fakeWifi2;
    [Serialized] private HoloGameObject finalBoard;
    [Serialized] private HoloGameObject virtualButtonE3;


    [Serialized] private HoloGameObject doneesTelechargeesGO;
    [Serialized] private HoloGameObject cube10_TriggerEndGame;


    private float timerGODisapearance = 0f;
    [Serialized] private float timerDelay;
    private float timerButtons = 0f;
    [Serialized] private float timerButtonsDelay;


    public override void Start()
    {
        Async.OnUpdate += Update;
    }

    public void Update()
    {
        


        if (cube10_TriggerEndGame.activeSelf)
        {
            //doneesTelechargeesGO.SetActive(true);

            timerGODisapearance += 1f / 60f;

            if (timerGODisapearance >= timerDelay)
            {
                realWifi.SetActive(false);
                fakeWifi1.SetActive(false);
                fakeWifi2.SetActive(false);
                finalBoard.SetActive(false);
                virtualButtonE3.SetActive(false);
            }
        }


    }


}
