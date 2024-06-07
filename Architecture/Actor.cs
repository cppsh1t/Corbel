namespace Corbel
{

    public abstract class SceneActor : Actor
    {
        public override Regulator GetRegulator()
        {
            return CorbelRoot.CurrentSceneRegulator;
        }
    }


    public abstract class RootActor : Actor
    {
        public override Regulator GetRegulator()
        {
            return CorbelRoot.Instance;
        }
    }

}