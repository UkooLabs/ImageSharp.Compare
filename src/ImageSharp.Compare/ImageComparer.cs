using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UkooLabs.ImageSharp.Compare
{
    public abstract class ImageComparer
    {
        public static ImageComparer Exact { get; } = Tolerant(0, 0);

        /// <summary>
        /// Returns an instance of <see cref="TolerantImageComparer"/>.
        /// Individual manhattan pixel difference is only added to total image difference when the individual difference is over 'perPixelManhattanThreshold'.
        /// </summary>
        /// <returns>A ImageComparer instance.</returns>
        public static ImageComparer Tolerant(
            float imageThreshold = TolerantImageComparer.DefaultImageThreshold,
            int perPixelManhattanThreshold = 0)
        {
            return new TolerantImageComparer(imageThreshold, perPixelManhattanThreshold);
        }

        /// <summary>
        /// Returns Tolerant(imageThresholdInPercents/100)
        /// </summary>
        /// <returns>A ImageComparer instance.</returns>
        public static ImageComparer TolerantPercentage(float imageThresholdInPercents, int perPixelManhattanThreshold = 0)
            => Tolerant(imageThresholdInPercents / 100F, perPixelManhattanThreshold);

        public abstract ImageSimilarityReport<TPixelA, TPixelB> CompareImagesOrFrames<TPixelA, TPixelB>(
            ImageFrame<TPixelA> expected,
            ImageFrame<TPixelB> actual)
            where TPixelA : unmanaged, IPixel<TPixelA>
            where TPixelB : unmanaged, IPixel<TPixelB>;
    }
}
