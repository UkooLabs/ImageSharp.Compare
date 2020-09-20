namespace UkooLabs.ImageSharp.Compare.Tests
{
    public static class TestHelper
    {
        public static byte SlightChange(byte value)
        {
            return value > 128 ? --value : ++value;
        }

        public static byte BigChange(byte value)
        {
            return (byte)(255 - value);
        }
    }
}
