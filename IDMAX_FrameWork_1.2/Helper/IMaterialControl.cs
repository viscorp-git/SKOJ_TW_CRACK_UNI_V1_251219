namespace IDMAX_FrameWork
{
    public interface IMaterialControl
    {
        int Depth { get; set; }
        MaterialSkinManager SkinManager { get; }
        MouseActiveState MouseState { get; set; }

    }

    public enum MouseActiveState
    {
        HOVER,
        DOWN,
        OUT
    }
}
