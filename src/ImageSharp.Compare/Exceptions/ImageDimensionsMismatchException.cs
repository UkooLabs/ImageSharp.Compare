using SixLabors.ImageSharp;

namespace UkooLabs.ImageSharp.Compare.Exceptions
{
    public class ImageDimensionsMismatchException : ImagesSimilarityException
    {
        public ImageDimensionsMismatchException(Size expectedSize, Size actualSize)
            : base($"The image dimensions {actualSize} do not match the expected {expectedSize}!")
        {
            ExpectedSize = expectedSize;
            ActualSize = actualSize;
        }

        public Size ExpectedSize { get; }

        public Size ActualSize { get; }
    }
}
