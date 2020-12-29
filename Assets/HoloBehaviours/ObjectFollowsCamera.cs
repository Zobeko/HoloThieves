using Holo;

//Scrypt rattaché au User qui suit contament la position du joueur casque (utile si on veut que des objets suivent le joueur)

public class ObjectFollowsCamera : HoloBehaviour
{
    //Component permettant que l'objet auquel est rattaché ce script suive la position du joueur 
    [EaseRecenterComponent]
    private EaseRecenterComponent easeRecenter;


    public override void Start()
    {
        //On dit à l'objet de commencer à suivre la position du joueur
        easeRecenter.StartMoving();
        
    }



}
