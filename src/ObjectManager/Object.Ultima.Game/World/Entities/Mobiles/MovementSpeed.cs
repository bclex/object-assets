namespace OA.Ultima.World.Entities.Mobiles
{
    public static class MovementSpeed
    {
        static double _timeWalkFoot = (8d / 20d) * 1000d;
        static double _timeRunFoot = (4d / 20d) * 1000d;
        static double _timeWalkMount = (4d / 20d) * 1000d;
        static double _timeRunMount = (2d / 20d) * 1000d;

        public static double TimeToCompleteMove(AEntity entity, Direction facing)
        {
            if (entity is Mobile && (entity as Mobile).IsMounted)
                return (facing & Direction.Running) == Direction.Running ? _timeRunMount : _timeWalkMount;
            else
                return (facing & Direction.Running) == Direction.Running ? _timeRunFoot : _timeWalkFoot;
        }
    }
}
