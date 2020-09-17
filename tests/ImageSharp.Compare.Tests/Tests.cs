using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Linq;
using UkooLabs.ImageSharp.Compare;
using Xunit;

namespace PNI.Templates.Mapping.Tests
{
    public class Tests
    {
        [Fact]
        public void ExactImageComparerCompareSame()
        {
            var testCardPath = Path.Combine(PathHelper.ImagesPath, "TestCard.png");
            using var expectedImage = Image.Load<Rgba32>(testCardPath);
            using var actualImage = Image.Load<Rgba32>(testCardPath);
            var reports = ExactImageComparer.Instance.CompareImages(expectedImage, actualImage);
            Assert.Empty(reports);
        }

        [Fact]
        public void ExactImageComparerCompareDifferent()
        {
            var testCardPath = Path.Combine(PathHelper.ImagesPath, "TestCard.png");
            using var expectedImage = Image.Load<Rgba32>(testCardPath);
            using var actualImage = Image.Load<Rgba32>(testCardPath);
            var pixel = actualImage[0, 0];
            pixel.A = (byte)(255 - pixel.A);
            actualImage[0, 0] = pixel;
            var reports = ExactImageComparer.Instance.CompareImages(expectedImage, actualImage);
            Assert.Single(reports);
        }
    }
}
