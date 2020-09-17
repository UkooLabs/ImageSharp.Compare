using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UkooLabs.ImageSharp.Compare
{
    public class ImageSimilarityReport
    {
        protected ImageSimilarityReport(
            object expectedImage,
            object actualImage,
            IEnumerable<PixelDifference> differences,
            float? totalNormalizedDifference = null)
        {
            ExpectedImage = expectedImage;
            ActualImage = actualImage;
            TotalNormalizedDifference = totalNormalizedDifference;
            Differences = differences.ToArray();
        }

        public object ExpectedImage { get; }

        public object ActualImage { get; }

        // TODO: This should not be a nullable value!
        public float? TotalNormalizedDifference { get; }

        public string DifferencePercentageString
        {
            get
            {
                if (!TotalNormalizedDifference.HasValue)
                {
                    return "?";
                }
                else if (TotalNormalizedDifference == 0)
                {
                    return "0%";
                }
                else
                {
                    return $"{TotalNormalizedDifference.Value * 100:0.0000}%";
                }
            }
        }

        public PixelDifference[] Differences { get; }

        public bool IsEmpty => Differences.Length == 0;

        public override string ToString()
        {
            return IsEmpty ? "[SimilarImages]" : PrintDifference();
        }

        private string PrintDifference()
        {
            var sb = new StringBuilder();
            if (TotalNormalizedDifference.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine($"Total difference: {DifferencePercentageString}");
            }

            int max = Math.Min(5, Differences.Length);

            for (int i = 0; i < max; i++)
            {
                sb.Append(Differences[i]);
                if (i < max - 1)
                {
                    sb.AppendFormat(";{0}", Environment.NewLine);
                }
            }

            if (Differences.Length >= 5)
            {
                sb.Append("...");
            }

            return sb.ToString();
        }
    }

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