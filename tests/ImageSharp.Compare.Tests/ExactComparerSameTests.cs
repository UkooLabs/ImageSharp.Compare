using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using UkooLabs.ImageSharp.Compare;
using Xunit;

namespace PNI.Templates.Mapping.Tests
{
    public class ExactComparerSameTests : IDisposable
    {
        private Image<Rgba32> BaseImage { get; }

        public ExactComparerSameTests()
        {
            var testCardPath = Path.Combine(PathHelper.ImagesPath, "TestCard.png");
            BaseImage = Image.Load<Rgba32>(testCardPath);
        }

        public void Dispose()
        {
            BaseImage.Dispose();
        }
       

        [Fact]
        public void ExactComparer_CompareImages_SameImage()
        {
            var reports = ImageComparer.Exact().CompareImages(BaseImage, BaseImage).ToArray(); ;
            Assert.Empty(reports);
        }

        [Fact]
        public void ExactComparer_CompareImagesOrFrames_SameImage()
        {
            var report = ImageComparer.Exact().CompareImagesOrFrames(BaseImage, BaseImage);
            Assert.Empty(report.Differences);
            Assert.True(report.IsEmpty);
            Assert.Equal("?", report.DifferencePercentageString);
            Assert.Null(report.TotalNormalizedDifference);
            Assert.Equal("[SimilarImages]", report.ToString());
        }

        [Fact]
        public void ExactComparer_VerifySimilarity_SameImage()
        {
            ImageComparer.Exact().VerifySimilarity(BaseImage, BaseImage);
        }

        [Fact]
        public void ExactComparer_VerifySimilarityIgnoreRegionNone_SameImage()
        {
            ImageComparer.Exact().VerifySimilarityIgnoreRegion(BaseImage, BaseImage, new Rectangle(0, 0, 0, 0));
        }

        [Fact]
        public void ExactComparer_VerifySimilarityIgnoreRegionModified_SameImage()
        {
            ImageComparer.Exact().VerifySimilarityIgnoreRegion(BaseImage, BaseImage, new Rectangle(0, 0, 100, 100));
        }
    }
}
