using Holo;

//Script rattaché au panneau de téléchargement des données qui s'affiche après que le capteur RFID ait été activé
public class EndGameBehaviour : HoloBehaviour
{
    //GameObjects à faire disparaitre
    [Serialized] private HoloGameObject realWifi;
    [Serialized] private HoloGameObject fakeWifi1;
    [Serialized] private HoloGameObject fakeWifi2;
    [Serialized] private HoloGameObject finalBoard;
    [Serialized] private HoloGameObject virtualButtonE3;
    [Serialized] private HoloGameObject triangle;
    [Serialized] private HoloGameObject circle;
    [Serialized] private HoloGameObject hexagon;
    [Serialized] private HoloGameObject equation;



    //GameObject permettant de trigger le lancement du timer
    [Serialized] private HoloGameObject cube10_TriggerEndGame;


    //Delais avant disparrition de tous les objets de la scene à la fin du jeu
    [Serialized] private float timerDelay;



    public override void Start()
    {
        Async.OnUpdate += Update;
    }

    public void Update()
    {
        
        if (cube10_TriggerEndGame.activeSelf)
        {

            Async.InvokeAfterSeconds(TimerFinDeJeu, timerDelay);
            
        }


    }

    public void TimerFinDeJeu()
    {
        realWifi.SetActive(false);
        fakeWifi1.SetActive(false);
        fakeWifi2.SetActive(false);
        finalBoard.SetActive(false);
        virtualButtonE3.SetActive(false);
        triangle.SetActive(false);
        circle.SetActive(false);
        hexagon.SetActive(false);
        equation.SetActive(false);

    }


}
