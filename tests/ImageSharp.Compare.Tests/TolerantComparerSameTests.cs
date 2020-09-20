using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace UkooLabs.ImageSharp.Compare.Tests
{
    public class TolerantComparerSameTests : IDisposable
    {
        private Image<Rgba32> BaseImage { get; }

        public TolerantComparerSameTests()
        {
            var testCardPath = Path.Combine(PathHelper.ImagesPath, "TestCard.png");
            BaseImage = Image.Load<Rgba32>(testCardPath);
        }

        public void Dispose()
        {
            BaseImage.Dispose();
        }


        [Fact]
        public void TolerantComparer_CompareImages_SameImage()
        {
            var reports = ImageComparer.TolerantExact().CompareImages(BaseImage, BaseImage).ToArray(); ;
            Assert.Empty(reports);
        }

        [Fact]
        public void TolerantComparer_CompareImagesOrFrames_SameImage()
        {
            var report = ImageComparer.TolerantExact().CompareImagesOrFrames(BaseImage, BaseImage);
            Assert.Empty(report.Differences);
            Assert.True(report.IsEmpty);
            Assert.Equal("0%", report.DifferencePercentageString);
            Assert.Equal(0, report.TotalNormalizedDifference);
            Assert.Equal("[SimilarImages]", report.ToString());
        }

        [Fact]
        public void TolerantComparer_VerifySimilarity_SameImage()
        {
            ImageComparer.TolerantExact().VerifySimilarity(BaseImage, BaseImage);
        }

        [Fact]
        public void TolerantComparer_VerifySimilarityIgnoreRegionNone_SameImage()
        {
            ImageComparer.TolerantExact().VerifySimilarityIgnoreRegion(BaseImage, BaseImage, new Rectangle(0, 0, 0, 0));
        }

        [Fact]
        public void TolerantComparer_VerifySimilarityIgnoreRegionModified_SameImage()
        {
            ImageComparer.TolerantExact().VerifySimilarityIgnoreRegion(BaseImage, BaseImage, new Rectangle(0, 0, 100, 100));
        }
    }
}
