using kandora.bot.mahjong;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;


namespace kandora.bot.utils
{
    class ImageToolbox
    {
        const int tileImgWidth = 80;
        const int tileImgHeight = 129;
        const float shrinkFactor = 3;
        const string inputResourceLoc = "kandora.bot.resources";
        const string inputfileName = "tiles.png";
        const string resourcesDirName = "resources";
        const string outputDirName = "generated";

        static readonly char dirChar = Path.DirectorySeparatorChar;
        static readonly string outputDirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));


        static Bitmap GetAllTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{inputResourceLoc}.{inputfileName}");
            var image = new Bitmap(inputFileStream);
            inputFileStream.Close();
            return image;
        }

        public static bool ImageExists(string hand, bool separateLastTile = false)
        {
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, GetFileName(hand, separateLastTile) });
            return File.Exists(outputFilePath);
        }

        public static string GetFileName(string hand, bool lastTileSeparation = false)
        {
            return $"{hand}{(lastTileSeparation ? "x" : "")}.png";
        }

        public static FileStream GetImageFromTiles(string hand, bool separateLastTile=false)
        {
            List<string> tiles = HandParser.SimpleTiles(hand).Where(x => x.Length == 2).ToList();
            var shouldSeparateLastTile = separateLastTile && tiles.Count == 14;
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, GetFileName(hand, shouldSeparateLastTile) });
            if (!ImageExists(hand, shouldSeparateLastTile))
            {
                if (!Directory.Exists(outputDirPath))
                {
                    Directory.CreateDirectory(outputDirPath);
                }
                CreateSaveImageFromHand(tiles, outputFilePath, shouldSeparateLastTile);
            }
            return new FileStream(outputFilePath, FileMode.Open);
        }

        public static void CreateSaveImageFromHand(List<string> tiles, string outputFilePath, bool separateLastTile = false)
        {
            var resImage = GetAllTiles();

            //expressed relative to a tile width
            double lastTileSeparation = separateLastTile ? 0.5 : 0;
            int destImageWidth = (int)(tileImgWidth * (tiles.Count + lastTileSeparation) / shrinkFactor);
            int destImageHeight = (int)(tileImgHeight / shrinkFactor);
            var destImage = new Bitmap(destImageWidth, destImageHeight);
            destImage.SetResolution(resImage.HorizontalResolution, resImage.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    for(int idx=0;idx<tiles.Count;idx++)
                    {
                        var tile = tiles[idx];
                        //Note: Luckily, the aka, noted "0" is also the first of the line in the resource file
                        int xPosition = int.Parse(tile[0].ToString())*tileImgWidth;
                        int yPosition = 0;
                        char suit = tile[1];
                        switch (suit)
                        {
                            case 's':
                                yPosition = 0;
                                break;
                            case 'm':
                                yPosition = 1;
                                break;
                            case 'p':
                                yPosition = 2;
                                break;
                            case 'z':
                                yPosition = 3;
                                xPosition = xPosition - tileImgWidth;
                                break;
                        }
                        yPosition *= tileImgHeight;
                        var srcRegion = new Rectangle(xPosition, yPosition, tileImgWidth, tileImgHeight);
                        var destRegion = new Rectangle(
                            (int)((idx + (idx==tiles.Count-1?lastTileSeparation:0)) * tileImgWidth / shrinkFactor),
                            0,
                            (int)(tileImgWidth / shrinkFactor),
                            (int)(tileImgHeight / shrinkFactor)
                        );
                        graphics.DrawImage(resImage, destRegion, srcRegion, GraphicsUnit.Pixel);
                    }
                    
                }
            }
            destImage.Save(outputFilePath, ImageFormat.Png);
        }


        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
