namespace OA.Ultima.World.Entities.Mobiles
{
    class MobileMoveEvent
    {
        public bool CreatedByPlayerInput;
        public readonly int X, Y, Z, Facing, Fastwalk;

        public MobileMoveEvent(int x, int y, int z, int facing, int fastwalk)
        {
            X = x;
            Y = y;
            Z = z;
            Facing = facing;
            Fastwalk = fastwalk;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }
    }
}
