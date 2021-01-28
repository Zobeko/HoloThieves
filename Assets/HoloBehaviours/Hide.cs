using Holo;

public class Hide : HoloBehaviour
{
    [GazeComponent] private GazeComponent gazeComponent;
    public override void Start()
    {
        gazeComponent.attribute.UseSnap = false;
    }


    public void Update()
    {
        
    }
}
