
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using File = System.IO.File;

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


        static Image GetAllTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{FileUtils.inputResourceLoc}.{inputfileName}");
            var image = Image.Load(inputFileStream);
            inputFileStream.Close();
            return image;
        }

        static Image GetAllCalledTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{FileUtils.inputResourceLoc}.{inputCalledfileName}");
            var image = Image.Load(inputFileStream);
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
            var shouldSeparateLastTile = separateLastTile && tiles.Count + meldTiles.Count == 14;
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
            var imageOffset = (int)(tileImgWidth / (2 * shrinkFactor));
            destImageWidth += (int)((tileImgWidth * nbMeldTilesNotCalled + tileImgHeight * nbMeldTilesCalled) / shrinkFactor) + imageOffset;
            int destImageHeight = (int)(tileImgHeight / shrinkFactor);
            var destImage = new Image<Rgba32>(destImageWidth, destImageHeight);


            var currentDestPosX = imageOffset;
            for (int idx=0;idx<tiles.Count;idx++)
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
                var tileWidth = (int)(tileImgWidth / shrinkFactor);
                var tileHeight = (int)(tileImgHeight / shrinkFactor);
                var destRegion = new Rectangle(
                    currentDestPosX,
                    0,
                    tileWidth,
                    tileHeight
                );
                var tileImage = resImage.Clone(x => x.Crop(srcRegion).Resize(tileWidth, tileHeight));
                destImage.Mutate(x=>x.DrawImage(tileImage, new Point(currentDestPosX,0),1));
            }
            // arbitrary distance between hand and melds
            currentDestPosX += (int)(1.5 * tileImgWidth / shrinkFactor);
            for (int idx = 0; idx < meldTiles.Count; idx++)
            {

                var tile = meldTiles[idx];
                var isCalled = isTileCalled(tile);
                var srcRegion = isCalled ? getSourceRegionForCalledTile(tile) : getSourceRegion(tile);
                var srcImage = isCalled ? resImageCalled : resImage;

                var tileWidth = isCalled ? (int)(tileImgHeight / shrinkFactor) : (int)(tileImgWidth / shrinkFactor);
                var tileHeight = isCalled ? (int)(tileImgWidth / shrinkFactor) : (int)(tileImgHeight / shrinkFactor);
                var currentDestPosY = isCalled ? tileWidth-tileHeight : 0;
                var destRegion = new Rectangle(
                    currentDestPosX,
                    isCalled ? (int)((tileImgHeight-tileImgWidth)/ shrinkFactor) : 0,
                    tileWidth,
                    tileHeight
                );

                var tileImage = srcImage.Clone(x => x.Crop(srcRegion).Resize(tileWidth, tileHeight));
                destImage.Mutate(x => x.DrawImage(tileImage, new Point(currentDestPosX, currentDestPosY), 1));
                currentDestPosX += isCalled ? (int)(tileImgHeight / shrinkFactor) : (int)(tileImgWidth / shrinkFactor);

            }

            var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
            destImage.Save(outputStream, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
            outputStream.Close();
        }
    }
}
