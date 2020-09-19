using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UkooLabs.ImageSharp.Compare.Reports;

namespace UkooLabs.ImageSharp.Compare
{
    public abstract class ImageComparer
    {
        // 1% of all pixels in a 100*100 pixel area are allowed to have a difference of 1 unit
        // 257 = (1 / 255) * 65535.
        public const float DefaultTolerantImageThreshold = 257F / (100 * 100 * 65535);

        public const int DefaultPerPixelChannelManhattanThreshold = 257;

        public static ImageComparer Exact()
        {
            return new ExactImageComparer();
        }

        public static ImageComparer TolerantExact()
        {
            return Tolerant(0, 0);
        }

        /// <summary>
        /// Returns an instance of <see cref="TolerantImageComparer"/>.
        /// Individual manhattan pixel difference is only added to total image difference when the individual difference is over 'perPixelManhattanThreshold'.
        /// </summary>
        /// <returns>A ImageComparer instance.</returns>
        public static ImageComparer Tolerant(
            float imageThreshold = DefaultTolerantImageThreshold,
            int perPixelManhattanThreshold = 0)
        {
            return new TolerantImageComparer(imageThreshold, perPixelManhattanThreshold);
        }

        /// <summary>
        /// Returns Calculated Per Pixel Manhattan Threshold Value
        /// </summary>
        /// <returns>channelCount * perPixelChannelManhattanThreshold.</returns>
        public static int CalculatePerPixelManhattanThresholdValue(byte channelCount, int perPixelChannelManhattanThreshold = DefaultPerPixelChannelManhattanThreshold)
        {
            return channelCount * perPixelChannelManhattanThreshold;
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
