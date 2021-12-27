namespace CaptainCoder
{

    public class Utils
    {

        public static T MemoizeField<T>(ref T actual, T initValue)
        {
            if (actual == null)
            {
                actual = initValue;
            }
            return actual;
        }

    }

}