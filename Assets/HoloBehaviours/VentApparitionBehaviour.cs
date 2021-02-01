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
        //Permet d'utiliser les méthode OnUserEnter() et OnUserExit() définies ci dessous 
        //(méthode permettant de savoir si on est assez proche d'une partie de conduit pour la faire apparaitre)
        userPositionTrigger.OnUserEnter += OnUserEnter;
        userPositionTrigger.OnUserExit += OnUserExit;
    }

    //Est appelée quand le joueur casque entre dans la zone de cette partie de conduit
    public void OnUserEnter()
    {
        //On affiche la partie concernée
        ventPart.SetActive(true);

    }

    //Est appelée quand le joueur casque sort de la zone de cette partie de conduit
    public void OnUserExit()
    {
        //On fait disparaitre la partie de conduit concernée
        ventPart.SetActive(false);

    }
}
