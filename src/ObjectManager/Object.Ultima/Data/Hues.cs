namespace OA.Ultima.Data
{
    public static class Hues
    {
        public static int[] SkinTones
        {
            get
            {
                var max = 7 * 8;
                var hues = new int[max];
                for (var i = 0; i < max; i++)
                    hues[i] = i < 37 ? i + 1002 : i + 1003;
                return hues;
            }
        }

        public static int[] HairTones
        {
            get
            {
                var max = 8 * 6;
                var hues = new int[max];
                for (var i = 0; i < max; i++)
                    hues[i] = i + 1102;
                return hues;
            }
        }

        public static int[] TextTones
        {
            get
            {
                var max = 1024;
                var hues = new int[max];
                for (var i = 0; i < max; i++)
                    hues[i] = i + 2;
                return hues;
            }
        }
    }
}
