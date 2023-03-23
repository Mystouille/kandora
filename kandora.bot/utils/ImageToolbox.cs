using DSharpPlus;
using kandora.bot.mahjong;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace kandora.bot.utils
{
    class ImageToolbox
    {
        const int tileImgWidth = 80;
        const int tileImgHeight = 129;
        const float shrinkFactor = 3;
        const string inputfileName = "tiles.png";
        const string inputCalledfileName = "tilesCalled.png";
        const string resourcesDirName = "resources";
        const string outputDirName = "generated";

        static readonly char dirChar = Path.DirectorySeparatorChar;
        static readonly string outputDirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));


        static Bitmap GetAllTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{FileUtils.inputResourceLoc}.{inputfileName}");
            var image = new Bitmap(inputFileStream);
            inputFileStream.Close();
            return image;
        }

        static Bitmap GetAllCalledTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{FileUtils.inputResourceLoc}.{inputCalledfileName}");
            var image = new Bitmap(inputFileStream);
            inputFileStream.Close();
            return image;
        }

        public static bool ImageExists(string hand, string melds= "", bool separateLastTile = false)
        {
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, GetFileName(hand, melds, separateLastTile) });
            return File.Exists(outputFilePath);
        }

        public static string GetFileName(string hand, string melds = "", bool lastTileSeparation = false)
        {
            return $"{hand}{melds}{(lastTileSeparation ? "x" : "")}.png";
        }

        public static FileStream GetImageFromTiles(string hand, string melds= "", bool separateLastTile=false)
        {
            List<string> tiles = HandParser.SplitTiles(hand);
            List<string> meldTiles = HandParser.SplitTiles(melds);
            var shouldSeparateLastTile = separateLastTile && tiles.Count == 14;
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, GetFileName(hand, melds, shouldSeparateLastTile) });
            if (!ImageExists(hand, melds, shouldSeparateLastTile))
            {
                if (!Directory.Exists(outputDirPath))
                {
                    Directory.CreateDirectory(outputDirPath);
                }
                CreateSaveImageFromHand(tiles, meldTiles, outputFilePath, shouldSeparateLastTile);
            }
            return new FileStream(outputFilePath, FileMode.Open);
        }

        public static Rectangle getSourceRegion(string tile)
        {
            //Note: Luckily, the aka, noted "0" is also the first of the line in the resource file
            int xPosition = int.Parse(tile[0].ToString()) * tileImgWidth;
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
                    //0z is the back of the tile
                    if (xPosition == 0)
                    {
                        xPosition = 7 * tileImgWidth;
                    }
                    //1z is east
                    else
                    {
                        xPosition -= tileImgWidth;
                    }
                    break;
            }
            yPosition *= tileImgHeight;
            return new Rectangle(xPosition, yPosition, tileImgWidth, tileImgHeight);
        }

        public static Rectangle getSourceRegionForCalledTile(string tile)
        {
            //Note: Luckily, the aka, noted "0" is also the first of the line in the resource file
            int yPosition = (9 - int.Parse(tile[0].ToString())) * tileImgWidth;
            int xPosition = 0;
            char suit = tile[tile.Count() - 1];
            switch (suit)
            {
                case 's':
                    xPosition = 0;
                    break;
                case 'm':
                    xPosition = 1;
                    break;
                case 'p':
                    xPosition = 2;
                    break;
                case 'z':
                    xPosition = 3;
                    //0z is the back of the tile
                    if (yPosition == 10)
                    {
                        yPosition = 3 * tileImgWidth;
                    }
                    //1z is east
                    else
                    {
                        yPosition += tileImgWidth;
                    }
                    break;
            }
            xPosition *= tileImgHeight;
            return new Rectangle(xPosition, yPosition, tileImgHeight, tileImgWidth);
        }

        public static bool isTileCalled(string tile)
        {
            return tile[1] == '\'';
        }

        public static void CreateSaveImageFromHand(List<string> tiles, List<string> meldTiles, string outputFilePath, bool separateLastTile = false)
        {
            var resImage = GetAllTiles();
            var resImageCalled = GetAllCalledTiles();

            //expressed relative to a tile width
            double lastTileSeparation = separateLastTile ? 0.5 : 0;
            double handMeldsSeparation = meldTiles.Count() > 0 ? 1.5 : 0;
            int destImageWidth = (int)(tileImgWidth * (tiles.Count + lastTileSeparation + handMeldsSeparation) / shrinkFactor);
            var nbMeldTilesNotCalled = meldTiles.Where(x => !x.Contains('\'')).Count();
            var nbMeldTilesCalled = meldTiles.Where(x => x.Contains('\'')).Count();
            destImageWidth += (int)((tileImgWidth * nbMeldTilesNotCalled + tileImgHeight * nbMeldTilesCalled) / shrinkFactor);
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
                    var currentDestPosX = 0;
                    for(int idx=0;idx<tiles.Count;idx++)
                    {
                        if (idx != 0)
                        {
                            if (idx == tiles.Count - 1)
                            {
                                currentDestPosX += separateLastTile ? (int)(0.5 * tileImgWidth / shrinkFactor) : 0;
                            }
                            currentDestPosX += (int)(tileImgWidth / shrinkFactor);
                        }
                        var tile = tiles[idx];
                        
                        var srcRegion = getSourceRegion(tile);
                        var destRegion = new Rectangle(
                            currentDestPosX,
                            0,
                            (int)(tileImgWidth / shrinkFactor),
                            (int)(tileImgHeight / shrinkFactor)
                        );
                        graphics.DrawImage(resImage, destRegion, srcRegion, GraphicsUnit.Pixel);
                    }
                    // arbitrary distance between hand and melds
                    currentDestPosX += (int)(1.5 * tileImgWidth / shrinkFactor);
                    for (int idx = 0; idx < meldTiles.Count; idx++)
                    {

                        var tile = meldTiles[idx];
                        var isCalled = isTileCalled(tile);
                        var srcRegion = getSourceRegion(tile);
                        var srcImage = isCalled ? resImageCalled : resImage;
                        
                        if (isCalled)
                        {
                            srcRegion = getSourceRegionForCalledTile(tile);
                        }
                        var destRegion = new Rectangle(
                            currentDestPosX,
                            isCalled ? (int)((tileImgHeight-tileImgWidth)/ shrinkFactor) : 0,
                            isCalled ? (int)(tileImgHeight / shrinkFactor) : (int)(tileImgWidth / shrinkFactor),
                            isCalled ? (int)(tileImgWidth / shrinkFactor) :(int)(tileImgHeight / shrinkFactor)
                        );
                        graphics.DrawImage(srcImage, destRegion, srcRegion, GraphicsUnit.Pixel);
                        currentDestPosX += isCalled ? (int)(tileImgHeight / shrinkFactor) : (int)(tileImgWidth / shrinkFactor);
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
