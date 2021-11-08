public enum HexDirection {
    SE, S, SW, NW, N, NE
};

public static class HexDirectionExtensions {
    public static HexDirection Opposite (this HexDirection direction) {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }
    public static HexDirection Previous (this HexDirection direction) {
        return direction == HexDirection.SE ? HexDirection.NE : (direction - 1);
    }
    public static HexDirection Next (this HexDirection direction) {
        return direction == HexDirection.NE ? HexDirection.SE : (direction + 1);
    }
}