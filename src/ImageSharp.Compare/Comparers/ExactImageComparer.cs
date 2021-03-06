using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using UkooLabs.ImageSharp.Compare.Exceptions;
using UkooLabs.ImageSharp.Compare.Reports;

namespace UkooLabs.ImageSharp.Compare
{
    public class ExactImageComparer : ImageComparer
    {
        public override ImageSimilarityReport<TPixelA, TPixelB> CompareImagesOrFrames<TPixelA, TPixelB>(
            ImageFrame<TPixelA> expected,
            ImageFrame<TPixelB> actual)
        {
            if (expected.Size() != actual.Size())
            {
                throw new ImageDimensionsMismatchException(expected.Size(), actual.Size());
            }

            int width = actual.Width;

            // TODO: Comparing through Rgba64 may not be robust enough because of the existence of super high precision pixel types.
            var aBuffer = new Rgba64[width];
            var bBuffer = new Rgba64[width];

            var differences = new List<PixelDifference>();
            Configuration configuration = expected.GetConfiguration();

            for (int y = 0; y < actual.Height; y++)
            {
                Span<TPixelA> aSpan = expected.GetPixelRowSpan(y);
                Span<TPixelB> bSpan = actual.GetPixelRowSpan(y);

                PixelOperations<TPixelA>.Instance.ToRgba64(configuration, aSpan, aBuffer);
                PixelOperations<TPixelB>.Instance.ToRgba64(configuration, bSpan, bBuffer);

                for (int x = 0; x < width; x++)
                {
                    Rgba64 aPixel = aBuffer[x];
                    Rgba64 bPixel = bBuffer[x];

                    if (aPixel != bPixel)
                    {
                        var diff = new PixelDifference(new Point(x, y), aPixel, bPixel);
                        differences.Add(diff);
                    }
                }
            }

            return new ImageSimilarityReport<TPixelA, TPixelB>(expected, actual, differences);
        }
    }
}
