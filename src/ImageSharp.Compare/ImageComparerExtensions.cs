using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using UkooLabs.ImageSharp.Compare.Exceptions;
using UkooLabs.ImageSharp.Compare.Reports;

namespace UkooLabs.ImageSharp.Compare
{
    public static class ImageComparerExtensions
    {
        public static ImageSimilarityReport<TPixelA, TPixelB> CompareImagesOrFrames<TPixelA, TPixelB>(
            this ImageComparer comparer,
            Image<TPixelA> expected,
            Image<TPixelB> actual)
            where TPixelA : unmanaged, IPixel<TPixelA>
            where TPixelB : unmanaged, IPixel<TPixelB>
        {
            return comparer.CompareImagesOrFrames(expected.Frames.RootFrame, actual.Frames.RootFrame);
        }

        public static IEnumerable<ImageSimilarityReport<TPixelA, TPixelB>> CompareImages<TPixelA, TPixelB>(
            this ImageComparer comparer,
            Image<TPixelA> expected,
            Image<TPixelB> actual)
            where TPixelA : unmanaged, IPixel<TPixelA>
            where TPixelB : unmanaged, IPixel<TPixelB>
        {
            var result = new List<ImageSimilarityReport<TPixelA, TPixelB>>();

            if (expected.Frames.Count != actual.Frames.Count)
            {
                throw new ImageFramesMismatchException(expected.Frames.Count, actual.Frames.Count);
            }

            for (int i = 0; i < expected.Frames.Count; i++)
            {
                ImageSimilarityReport<TPixelA, TPixelB> report = comparer.CompareImagesOrFrames(expected.Frames[i], actual.Frames[i]);
                if (!report.IsEmpty)
                {
                    result.Add(report);
                }
            }

            return result;
        }

        public static void VerifySimilarity<TPixelA, TPixelB>(
            this ImageComparer comparer,
            Image<TPixelA> expected,
            Image<TPixelB> actual)
            where TPixelA : unmanaged, IPixel<TPixelA>
            where TPixelB : unmanaged, IPixel<TPixelB>
        {
            if (expected.Size() != actual.Size())
            {
                throw new ImageDimensionsMismatchException(expected.Size(), actual.Size());
            }

            if (expected.Frames.Count != actual.Frames.Count)
            {
                throw new ImageFramesMismatchException(expected.Frames.Count, actual.Frames.Count);
            }

            IEnumerable<ImageSimilarityReport> reports = comparer.CompareImages(expected, actual);
            if (reports.Any())
            {
                throw new ImageDifferenceIsOverThresholdException(reports);
            }
        }

        public static void VerifySimilarityIgnoreRegion<TPixelA, TPixelB>(
            this ImageComparer comparer,
            Image<TPixelA> expected,
            Image<TPixelB> actual,
            Rectangle ignoredRegion)
            where TPixelA : unmanaged, IPixel<TPixelA>
            where TPixelB : unmanaged, IPixel<TPixelB>
        {
            if (expected.Size() != actual.Size())
            {
                throw new ImageDimensionsMismatchException(expected.Size(), actual.Size());
            }

            if (expected.Frames.Count != actual.Frames.Count)
            {
                throw new ImageFramesMismatchException(expected.Frames.Count, actual.Frames.Count);
            }

            IEnumerable<ImageSimilarityReport<TPixelA, TPixelB>> reports = comparer.CompareImages(expected, actual);
            if (reports.Any())
            {
                var cleanedReports = new List<ImageSimilarityReport<TPixelA, TPixelB>>(reports.Count());
                foreach (ImageSimilarityReport<TPixelA, TPixelB> r in reports)
                {
                    IEnumerable<PixelDifference> outsideChanges = r.Differences.Where(
                        x =>
                        !(ignoredRegion.X <= x.Position.X
                        && x.Position.X < ignoredRegion.Right
                        && ignoredRegion.Y <= x.Position.Y
                        && x.Position.Y < ignoredRegion.Bottom));

                    if (outsideChanges.Any())
                    {
                        cleanedReports.Add(new ImageSimilarityReport<TPixelA, TPixelB>(r.ExpectedImage, r.ActualImage, outsideChanges, null));
                    }
                }

                if (cleanedReports.Count > 0)
                {
                    throw new ImageDifferenceIsOverThresholdException(cleanedReports);
                }
            }
        }
    }
}
