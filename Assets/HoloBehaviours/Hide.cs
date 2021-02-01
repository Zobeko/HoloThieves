using Holo;


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
        GazerHelper.ShowCursor(false);
    }

}
