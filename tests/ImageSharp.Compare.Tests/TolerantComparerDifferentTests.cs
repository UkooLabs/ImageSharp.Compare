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
    public class TolerantComparerDifferentTests : IDisposable
    {
        private Image<Rgba32> BaseImage { get; }
        private Image<Rgba32> TwoFramesImage { get; }
        private Image<Rgba32> SmallerImage { get; }
        private Image<Rgba32> ModifiedImage { get; }
        private Image<Rgba32> SmallChangeImage { get; }

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
                    pixel.R = (byte)(255 - pixel.R);
                    pixel.G = (byte)(255 - pixel.G);
                    pixel.B = (byte)(255 - pixel.B);
                    pixel.A = (byte)(255 - pixel.A);
                    ModifiedImage[x, y] = pixel;
                }
            }

            SmallChangeImage = BaseImage.Clone();
            for (var x = 0; x < 100; x++)
            {
                for (var y = 0; y < 100; y++)
                {
                    var pixel = SmallChangeImage[x, y];
                    pixel.R = SlightChange(pixel.R);
                    pixel.G = SlightChange(pixel.G);
                    pixel.B = SlightChange(pixel.B);
                    SmallChangeImage[x, y] = pixel;
                }
            }
        }

        byte SlightChange(byte value)
        {
            return value > 128 ? --value : ++value;
        }

        public void Dispose()
        {
            BaseImage.Dispose();
            SmallerImage.Dispose();
            ModifiedImage.Dispose();
            SmallChangeImage.Dispose();
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
