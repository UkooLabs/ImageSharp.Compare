using System;

namespace UkooLabs.ImageSharp.Compare.Exceptions
{
    public abstract class ImagesSimilarityException : Exception
    {
        public ImagesSimilarityException(string message) : base(message)
        {
        }
    }
}
