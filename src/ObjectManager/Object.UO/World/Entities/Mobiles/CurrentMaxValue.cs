namespace OA.Ultima.World.Entities.Mobiles
{
    public class CurrentMaxValue
    {
        public int Current;
        public int Max;

        public CurrentMaxValue()
        {
            Current = 1;
            Max = 1;
        }

        public CurrentMaxValue(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public void Update(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Current, Max);
        }
    }
}