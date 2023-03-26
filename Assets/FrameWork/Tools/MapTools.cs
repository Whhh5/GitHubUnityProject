

public enum EDoorDirction
{
    Left,
    Right,
    Up,
    Bottom,
    EnumCount,
}


public enum EMapPointType : int
{
    None = 0,
    Wall = 1 << 1,
    Floor = 1 << 2,
    Property = 1 << 3,
    EnumCount = 1 << 4,
}