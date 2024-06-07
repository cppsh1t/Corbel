namespace Corbel.IOC
{
    public interface IBean
    {
        void OnInitBean();

        void OnDestroyBean();
    }
}