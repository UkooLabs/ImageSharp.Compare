using SixLabors.ImageSharp;

namespace UkooLabs.ImageSharp.Compare.Exceptions
{
    public class ImageFramesMismatchException : ImagesSimilarityException
    {
        public ImageFramesMismatchException(int expectedCount, int actualCount)
            : base($"The image frame count of {actualCount} does not match the expected count of {expectedCount}!")
        {
            ExpectedCount = expectedCount;
            ActualCount = actualCount;
        }

        public int ExpectedCount { get; }

        public int ActualCount { get; }
    }
}
