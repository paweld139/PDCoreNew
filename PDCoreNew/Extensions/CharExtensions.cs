namespace PDCoreNew.Extensions
{
    public static class CharExtensions
    {
        public static int GetIntForLetter(this char letter)
        {
            return char.ToUpper(letter) - 64 - 1;
        }
    }
}
