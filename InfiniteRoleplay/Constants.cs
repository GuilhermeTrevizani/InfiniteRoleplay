namespace InfiniteRoleplay
{
    public class Constants
    {
        public static GTANetworkAPI.Vector3 PosicaoPrisao { get; } = new GTANetworkAPI.Vector3(461.7921, -989.0697, 24.91488);

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        };
    }
}