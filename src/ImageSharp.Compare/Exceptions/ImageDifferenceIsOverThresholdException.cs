using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UkooLabs.ImageSharp.Compare.Exceptions
{
    public class ImageDifferenceIsOverThresholdException : ImagesSimilarityException
    {
        public ImageSimilarityReport[] Reports { get; }

        public ImageDifferenceIsOverThresholdException(IEnumerable<ImageSimilarityReport> reports)
            : base("Image difference is over threshold!" + StringifyReports(reports))
        {
            Reports = reports.ToArray();
        }

        private static string StringifyReports(IEnumerable<ImageSimilarityReport> reports)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var report in reports)
            {
                sb.AppendLine($"Report ImageFrame {i}: {report}");
                i++;
            }
            return sb.ToString();
        }
    }
}
