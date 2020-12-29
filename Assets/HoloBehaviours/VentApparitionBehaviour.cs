using Holo;

//HoloBehaviour rattaché à chaque "ApparitionPoint" (empties contenant une partie de conduit chacun 
//et qui sont les points de détection de la présence du joueur casque dans la zone de la partie de conduit concernée)

public class VentApparitionBehaviour : HoloBehaviour
{
    //Component permettant de détecter quand le joueur casque est dans la zone de cette partie de conduit
    [UserPositionTrigger] private UserPositionTriggerComponent userPositionTrigger;
    //HoloGameObject de la partie de conduit concernée par ce "ApparitionPoint"
    [Serialized] private HoloGameObject ventPart;


    public override void Start()
    {
        userPositionTrigger.OnUserEnter += OnUserEnter;
        userPositionTrigger.OnUserExit += OnUserExit;
    }

    //Est appelée quand le joueur casque entre dans la zone de cette partie de conduit
    public void OnUserEnter()
    {
        ventPart.SetActive(true);

    }

    //Est appelée quand le joueur casque sort de la zone de cette partie de conduit
    public void OnUserExit()
    {
        ventPart.SetActive(false);

    }
}
