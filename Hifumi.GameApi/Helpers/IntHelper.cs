namespace Hifumi.GameApi.Helpers
{
    public class IntHelper
    {
        /// <summary>Clamp a number between a min and max value (inclusive)</summary>
        /// <param name="value">Number to clamp</param>
        /// <param name="min">Lower bound of range</param>
        /// <param name="max">Upper bound of range</param>
        public static int Clamp(int value, int min, int max)
            => value < min ? min : value > max ? max : value;
    }
}
