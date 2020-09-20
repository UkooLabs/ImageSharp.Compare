using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UkooLabs.ImageSharp.Compare.Reports
{
    public class ImageSimilarityReport
    {
        protected ImageSimilarityReport(
            ImageFrame expectedImageFrame,
            ImageFrame actualImageFrame,
            IEnumerable<PixelDifference> differences,
            float? totalNormalizedDifference = null)
        {
            ExpectedImageFrame = expectedImageFrame;
            ActualImageFrame = actualImageFrame;
            TotalNormalizedDifference = totalNormalizedDifference;
            Differences = differences.ToArray();
        }

        public ImageFrame ExpectedImageFrame { get; }

        public ImageFrame ActualImageFrame { get; }

        public Image<Rgba32> CreateDifferenceImage()
        {
            var differenceImage = new Image<Rgba32>(ActualImageFrame.Width, ActualImageFrame.Height, new Rgba32(128, 128, 128, 128));
            foreach (var difference in Differences)
            {
                var delta = (byte)(difference.Delta * 255);
                differenceImage[difference.Position.X, difference.Position.Y] = new Rgba32(delta, delta, delta, delta);
            }
            return differenceImage;
        }

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
}
