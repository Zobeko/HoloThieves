using Holo;

//Script permettant désactiver le curseur blanc au centre du casque
public class Hide : HoloBehaviour
{
    
    public override void Start()
    {
        GazerHelper.ShowCursor(false);
        
    }


    public void Update()
    {
        
    }

    public override void OnDestroy()
    {
        GazerHelper.ShowCursor(true);
    }

}
