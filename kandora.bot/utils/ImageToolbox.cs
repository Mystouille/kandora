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

        static Bitmap GetAllTiles()
        {
            var inputFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{inputResourceLoc}.{inputfileName}");
            var image = new Bitmap(inputFileStream);
            inputFileStream.Close();
            return image;
        }

        public static bool ImageExists(string hand)
        {
            var dirChar = Path.DirectorySeparatorChar;
            var outputDirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));
            var outputFileName = $"{hand}.png";
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, outputFileName });
            return File.Exists(outputFilePath);
        }

        public static (FileStream,string) getNewProblem(string suit = "s")
        {
            var dirChar = Path.DirectorySeparatorChar;
            var outputDirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));

            var offset = 0;
            if (suit == "s")
            {
                offset = 18;
            }
            else if (suit == "p")
            {
                offset = 9;
            }
            var rd = new Random();
            int[] hand;
            string handStr = "";
            int shanten = 7;
            bool handAlreadyExist = true;
            var nbIter = 0;
            var shantenCalc = new ShantenCalculator();
            var availableTiles = new List<int>();
            for(int i = 0; i <= 8; i++)
            {
                availableTiles.Add(i);
                availableTiles.Add(i);
                availableTiles.Add(i);
                availableTiles.Add(i);
            }
            while (shanten != 0 || handAlreadyExist)
            {
                hand = new int[34];
                for (int i = 1; i <= 13; i++)
                {
                    var roll = rd.Next(0, availableTiles.Count);
                    var value = availableTiles[roll];
                    hand[offset + value]++;
                    availableTiles.RemoveAt(roll);
                }
                shanten = shantenCalc.Calculate_shanten(hand);
                nbIter++;
                var hand136 = TilesConverter.to_136_array(hand.ToList());
                handStr = TilesConverter.to_one_line_string(hand136);

                var outputFileName = $"{hand}.png";
                var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, outputFileName });
                handAlreadyExist = File.Exists(outputFilePath);
            }
            return (GetImageFromTiles(handStr),handStr);
        }

        public static FileStream GetImageFromTiles(string hand)
        {
            List<string> tiles = HandParser.SimpleTiles(hand).Where(x => x.Length == 2).ToList();
            var dirChar = Path.DirectorySeparatorChar;
            var outputDirPath = string.Join(dirChar, Assembly.GetExecutingAssembly().Location.Split(dirChar).SkipLast(1).Append(resourcesDirName).Append(outputDirName));
            var outputFileName = $"{hand}.png";
            var outputFilePath = string.Join(dirChar, new string[] { outputDirPath, outputFileName });
            if (!File.Exists(outputFilePath))
            {
                CreateSaveImageFromHand(tiles, outputFilePath);
            }
            return new FileStream(outputFilePath, FileMode.Open);
        }

        public static void CreateSaveImageFromHand(List<string> tiles, string outputFilePath)
        {
            var resImage = GetAllTiles();

            int destImageWidth = (int)(tileImgWidth * tiles.Count / shrinkFactor);
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
                    int idx = 0;
                    foreach(var tile in tiles)
                    {
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
                                break;
                        }
                        yPosition *= tileImgHeight;
                        var srcRegion = new Rectangle(xPosition, yPosition, tileImgWidth, tileImgHeight);
                        var destRegion = new Rectangle(
                            (int)(idx * tileImgWidth / shrinkFactor),
                            0,
                            (int)(tileImgWidth / shrinkFactor),
                            (int)(tileImgHeight / shrinkFactor)
                        );
                        graphics.DrawImage(resImage, destRegion, srcRegion, GraphicsUnit.Pixel);
                        idx++;
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
