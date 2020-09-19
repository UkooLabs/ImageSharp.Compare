using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using UkooLabs.ImageSharp.Compare;
using UkooLabs.ImageSharp.Compare.Exceptions;
using Xunit;

namespace PNI.Templates.Mapping.Tests
{
    public class ExactComparerDifferentTests : IDisposable
    {
        private Image<Rgba32> BaseImage { get; }
        private Image<Rgba32> TwoFramesImage { get; }
        private Image<Rgba32> SmallerImage { get; }
        private Image<Rgba32> ModifiedImage { get; }

        public ExactComparerDifferentTests()
        {
            var testCardPath = Path.Combine(PathHelper.ImagesPath, "TestCard.png");
            BaseImage = Image.Load<Rgba32>(testCardPath);

            var twoFramesImage = BaseImage.Clone();
            _ = twoFramesImage.Frames.CreateFrame();
            TwoFramesImage = twoFramesImage;
                
            var smallerImage = BaseImage.Clone();
            smallerImage.Mutate(m => m.Resize(100, 100));
            SmallerImage = smallerImage;

            ModifiedImage = BaseImage.Clone();
            for (var x = 0; x < 100; x++)
            { 
                for (var y = 0; y < 100; y++)
                {
                    var pixel = ModifiedImage[x, y];
                    pixel.R = (byte)(255 - pixel.R);
                    pixel.G = (byte)(255 - pixel.G);
                    pixel.B = (byte)(255 - pixel.B);
                    pixel.A = (byte)(255 - pixel.A);
                    ModifiedImage[x, y] = pixel;
                }
            }
        }

        public void Dispose()
        {
            BaseImage.Dispose();
            SmallerImage.Dispose();
            ModifiedImage.Dispose();
        }

        [Fact]
        public void ExactComparer_CompareImages_NotSame()
        {
            var reports = ImageComparer.Exact().CompareImages(BaseImage, ModifiedImage).ToArray();
            Assert.Single(reports);
            Assert.Equal(10000, reports[0].Differences.Length);
            Assert.False(reports[0].IsEmpty);
            Assert.Equal("?", reports[0].DifferencePercentageString);
            Assert.Null(reports[0].TotalNormalizedDifference);
            Assert.StartsWith("[Δ", reports[0].ToString());
        }

        [Fact]
        public void ExactComparer_CompareImagesOrFrames_NotSame()
        {
            var report = ImageComparer.Exact().CompareImagesOrFrames(BaseImage, ModifiedImage);
            Assert.Equal(10000, report.Differences.Length);
            Assert.False(report.IsEmpty);
            Assert.Equal("?", report.DifferencePercentageString);
            Assert.Null(report.TotalNormalizedDifference);
            Assert.StartsWith("[Δ", report.ToString());
        }

        [Fact]
        public void ExactComparer_VerifySimilarity_ModifiedImage()
        {
            Assert.Throws<ImageDifferenceIsOverThresholdException>(() =>
            {
                ImageComparer.Exact().VerifySimilarity(BaseImage, ModifiedImage);
            });
        }

        [Fact]
        public void ExactComparer_VerifySimilarityIgnoreRegionNone_ModifiedImage()
        {
            Assert.Throws<ImageDifferenceIsOverThresholdException>(() =>
            {
                ImageComparer.Exact().VerifySimilarityIgnoreRegion(BaseImage, ModifiedImage, new Rectangle(0, 0, 0, 0));
            });
        }

        [Fact]
        public void ExactComparer_VerifySimilarity_SmallerImage()
        {
            Assert.Throws<ImageDimensionsMismatchException>(() =>
            {
                ImageComparer.Exact().VerifySimilarity(BaseImage, SmallerImage);
            });
        }

        [Fact]
        public void ExactComparer_VerifySimilarity_TwoFramesImage()
        {
            Assert.Throws<ImageFramesMismatchException>(() =>
            {
                ImageComparer.Exact().VerifySimilarity(BaseImage, TwoFramesImage);
            });
        }

        [Fact]
        public void ExactComparer_VerifySimilarityIgnoreRegionModified_ModifiedImage()
        {
            ImageComparer.Exact().VerifySimilarityIgnoreRegion(BaseImage, ModifiedImage, new Rectangle(0, 0, 100, 100));
        }
    }
}
