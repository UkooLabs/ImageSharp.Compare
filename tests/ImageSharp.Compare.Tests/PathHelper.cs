﻿using System;
using System.IO;

namespace UkooLabs.ImageSharp.Compare.Tests
{
    public static class PathHelper
    {
        private static string GetSolutionPath()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            while (!File.Exists(Path.Combine(path, "ImageSharp.Compare.sln")))
            {
                path = Path.GetFullPath(Path.Combine(path, ".."));
            }
            return path;
        }

        public static string SolutionPath => GetSolutionPath();

        public static string ImagesPath => Path.Combine(SolutionPath, "tests", "Images");
    }
}
