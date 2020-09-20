using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using UkooLabs.ImageSharp.Compare.Exceptions;
using Xunit;

namespace UkooLabs.ImageSharp.Compare.Tests
{
    public class TolerantComparerDifferentTests : IDisposable
    {
        private Image<Rgba32> BaseImage { get; }
        private Image<Rgba32> TwoFramesImage { get; }
        private Image<Rgba32> SmallerImage { get; }
        private Image<Rgba32> ModifiedImage { get; }
        private Image<Rgba32> SmallChangeImage { get; }
        private Image<Rgba32> CheckerChangeImage { get; }

        public TolerantComparerDifferentTests()
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
                    pixel.R = TestHelper.BigChange(pixel.R);
                    pixel.G = TestHelper.BigChange(pixel.G);
                    pixel.B = TestHelper.BigChange(pixel.B);
                    pixel.A = TestHelper.BigChange(pixel.A);
                    ModifiedImage[x, y] = pixel;
                }
            }

            SmallChangeImage = BaseImage.Clone();
            for (var x = 0; x < 100; x++)
            {
                for (var y = 0; y < 100; y++)
                {
                    var pixel = SmallChangeImage[x, y];
                    pixel.R = TestHelper.SlightChange(pixel.R);
                    pixel.G = TestHelper.SlightChange(pixel.G);
                    pixel.B = TestHelper.SlightChange(pixel.B);
                    SmallChangeImage[x, y] = pixel;
                }
            }

            CheckerChangeImage = BaseImage.Clone();
            for (var x = 0; x < CheckerChangeImage.Width; x++)
            {
                for (var y = 0; y < CheckerChangeImage.Height; y++)
                {
                    var pixel = CheckerChangeImage[x, y];
                    if (x % 200 < 100 ^ y % 200 >= 100)
                    {
                        pixel.R = TestHelper.SlightChange(pixel.R);
                        pixel.G = TestHelper.SlightChange(pixel.G);
                        pixel.B = TestHelper.SlightChange(pixel.B);
                    }
                    else
                    {
                        pixel.R = TestHelper.BigChange(pixel.R);
                        pixel.G = TestHelper.BigChange(pixel.G);
                        pixel.B = TestHelper.BigChange(pixel.B);
                    }
                    CheckerChangeImage[x, y] = pixel;
                }
            }
        }

        public void Dispose()
        {
            BaseImage.Dispose();
            SmallerImage.Dispose();
            ModifiedImage.Dispose();
            SmallChangeImage.Dispose();
            CheckerChangeImage.Dispose();
        }

        [Fact]
        public void TolerantComparer_CompareImages_ModifiedImage()
        {
            var reports = ImageComparer.Tolerant().CompareImages(BaseImage, ModifiedImage).ToArray();
            Assert.Single(reports);
            Assert.Equal(10000, reports[0].Differences.Length);
            Assert.False(reports[0].IsEmpty);
            Assert.Equal("0.1646%", reports[0].DifferencePercentageString);
            Assert.Equal(0.001646257f, reports[0].TotalNormalizedDifference);
            Assert.StartsWith("Total difference:", reports[0].ToString());
        }

        [Fact]
        public void TolerantComparer_CompareImages_CheckerImage()
        {
            var perPixelManhattanThresholdValue = ImageComparer.CalculatePerPixelManhattanThresholdValue(3, ImageComparer.DefaultPerPixelChannelManhattanThreshold);
            var reports = ImageComparer.Tolerant(0, perPixelManhattanThresholdValue).CompareImages(BaseImage, CheckerChangeImage).ToArray();
            Assert.Single(reports);
            using var actualDifference = reports[0].CreateDifferenceImage();
            var ExpectedDifferencePath = Path.Combine(PathHelper.ImagesPath, "ExpectedDifference.png");
            using var expectedDifference = Image.Load<Rgba32>(ExpectedDifferencePath);
            var differenceReports = ImageComparer.Exact().CompareImages(expectedDifference, actualDifference).ToArray();
            Assert.Empty(differenceReports);
        }

        [Fact]
        public void TolerantComparer_CompareImagesOrFrames_ModifiedImage()
        {
            var report = ImageComparer.Tolerant().CompareImagesOrFrames(BaseImage, ModifiedImage);
            Assert.Equal(10000, report.Differences.Length);
            Assert.False(report.IsEmpty);
            Assert.Equal("0.1646%", report.DifferencePercentageString);
            Assert.Equal(0.001646257f, report.TotalNormalizedDifference);
            Assert.StartsWith("Total difference:", report.ToString());
        }

        [Fact]
        public void TolerantComparer_VerifySimilarity_ModifiedImage()
        {
            Assert.Throws<ImageDifferenceIsOverThresholdException>(() =>
            {
                ImageComparer.Tolerant().VerifySimilarity(BaseImage, ModifiedImage);
            });
        }

        [Fact]
        public void TolerantComparer_VerifySimilarityIgnoreRegionNone_ModifiedImage()
        {
            Assert.Throws<ImageDifferenceIsOverThresholdException>(() =>
            {
                ImageComparer.Tolerant().VerifySimilarityIgnoreRegion(BaseImage, ModifiedImage, new Rectangle(0, 0, 0, 0));
            });
        }

        [Fact]
        public void TolerantComparer_VerifySimilarity_SmallChangeImage()
        {
            var perPixelManhattanThresholdValue = ImageComparer.CalculatePerPixelManhattanThresholdValue(3, ImageComparer.DefaultPerPixelChannelManhattanThreshold);
            ImageComparer.Tolerant(0, perPixelManhattanThresholdValue).VerifySimilarity(BaseImage, SmallChangeImage);
            perPixelManhattanThresholdValue = ImageComparer.CalculatePerPixelManhattanThresholdValue(1, ImageComparer.DefaultPerPixelChannelManhattanThreshold);
            Assert.Throws<ImageDifferenceIsOverThresholdException>(() =>
            {
                ImageComparer.Tolerant(0, perPixelManhattanThresholdValue).VerifySimilarity(BaseImage, SmallChangeImage);
            });
        }

        [Fact]
        public void TolerantComparer_VerifySimilarity_SmallerImage()
        {
            Assert.Throws<ImageDimensionsMismatchException>(() =>
            {
                ImageComparer.Tolerant().VerifySimilarity(BaseImage, SmallerImage);
            });
        }

        [Fact]
        public void TolerantComparer_VerifySimilarity_TwoFramesImage()
        {
            Assert.Throws<ImageFramesMismatchException>(() =>
            {
                ImageComparer.Tolerant().VerifySimilarity(BaseImage, TwoFramesImage);
            });
        }

        [Fact]
        public void TolerantComparer_VerifySimilarityIgnoreRegionModified_ModifiedImage()
        {
            ImageComparer.Tolerant().VerifySimilarityIgnoreRegion(BaseImage, ModifiedImage, new Rectangle(0, 0, 100, 100));
        }
    }
}
