using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UkooLabs.ImageSharp.Compare.Reports
{
    public class ImageSimilarityReport<TPixelA, TPixelB> : ImageSimilarityReport
        where TPixelA : unmanaged, IPixel<TPixelA>
        where TPixelB : unmanaged, IPixel<TPixelB>
    {
        public ImageSimilarityReport(
            ImageFrame<TPixelA> expectedImage,
            ImageFrame<TPixelB> actualImage,
            IEnumerable<PixelDifference> differences,
            float? totalNormalizedDifference = null)
            : base(expectedImage, actualImage, differences, totalNormalizedDifference)
        {
        }

        public static ImageSimilarityReport<TPixelA, TPixelB> Empty =>
            new ImageSimilarityReport<TPixelA, TPixelB>(null, null, Enumerable.Empty<PixelDifference>(), 0f);

        public new ImageFrame<TPixelA> ExpectedImage => (ImageFrame<TPixelA>)base.ExpectedImage;

        public new ImageFrame<TPixelB> ActualImage => (ImageFrame<TPixelB>)base.ActualImage;
    }
}
