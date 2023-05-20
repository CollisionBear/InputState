namespace Fyrvall.Input
{
    public interface IIconSetProvider
    {
        InputIconSet GetIconSetForDeviceType(InputDeviceType deviceType);
    }
}