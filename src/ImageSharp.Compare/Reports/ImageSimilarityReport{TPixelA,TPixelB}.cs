using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UkooLabs.ImageSharp.Compare.Reports
{
    public class ImageSimilarityReport<TPixelA, TPixelB> : ImageSimilarityReport
        where TPixelA : unmanaged, IPixel<TPixelA>
        where TPixelB : unmanaged, IPixel<TPixelB>
    {
        public ImageSimilarityReport(
            ImageFrame<TPixelA> expectedImageFrame,
            ImageFrame<TPixelB> actualImageFrame,
            IEnumerable<PixelDifference> differences,
            float? totalNormalizedDifference = null)
            : base(expectedImageFrame, actualImageFrame, differences, totalNormalizedDifference)
        {
        }

        public static ImageSimilarityReport<TPixelA, TPixelB> Empty =>
            new ImageSimilarityReport<TPixelA, TPixelB>(null, null, Enumerable.Empty<PixelDifference>(), 0f);

        public new ImageFrame<TPixelA> ExpectedImageFrame => (ImageFrame<TPixelA>)base.ExpectedImageFrame;

        public new ImageFrame<TPixelB> ActualImageFrame => (ImageFrame<TPixelB>)base.ActualImageFrame;
    }
}
