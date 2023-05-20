namespace CollisionBear.InputState
{
    public interface IIconSetProvider
    {
        InputIconSet GetIconSetForDeviceType(InputDeviceType deviceType);
    }
}