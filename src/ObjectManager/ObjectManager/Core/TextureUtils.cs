using UnityEngine;

namespace OA.Core
{
    public static class TextureUtils
    {
        //public static void FlipTexture2DVertically(Texture2D texture2D)
        //{
        //    var pixels = texture2D.GetPixels32();
        //    ArrayUtils.Flip2DArrayVertically(pixels, texture2D.height, texture2D.width);
        //    texture2D.SetPixels32(pixels);
        //    texture2D.Apply();
        //}

        //public static void RotateTexture2D(Texture2D texture2D)
        //{
        //    var pixels = texture2D.GetPixels32();
        //    ArrayUtils.Rotate2DArray(pixels, texture2D.width);
        //    texture2D.SetPixels32(pixels);
        //    texture2D.Apply();
        //}

        public static int CalculateMipMapCount(int baseTextureWidth, int baseTextureHeight)
        {
            Debug.Assert(baseTextureWidth > 0 && baseTextureHeight > 0);
            var longerLength = Mathf.Max(baseTextureWidth, baseTextureHeight);
            var mipMapCount = 0;
            var currentLongerLength = longerLength;
            while (currentLongerLength > 0)
            {
                mipMapCount++;
                currentLongerLength /= 2;
            }
            return mipMapCount;
        }

        public static int CalculateMipMappedTextureDataSize(int baseTextureWidth, int baseTextureHeight, int bytesPerPixel)
        {
            Debug.Assert(baseTextureWidth > 0 && baseTextureHeight > 0 && bytesPerPixel > 0);
            var dataSize = 0;
            var currentWidth = baseTextureWidth;
            var currentHeight = baseTextureHeight;
            while (true)
            {
                dataSize += (currentWidth * currentHeight * bytesPerPixel);
                if (currentWidth == 1 && currentHeight == 1)
                    break;
                currentWidth = currentWidth > 1 ? (currentWidth / 2) : currentWidth;
                currentHeight = currentHeight > 1 ? (currentHeight / 2) : currentHeight;
            }
            return dataSize;
        }

        // TODO: Improve algorithm for images with odd dimensions.
        public static void Downscale4Component32BitPixelsX2(byte[] srcBytes, int srcStartIndex, int srcRowCount, int srcColumnCount, byte[] dstBytes, int dstStartIndex)
        {
            var bytesPerPixel = 4;
            var componentCount = 4;
            Debug.Assert(srcStartIndex >= 0 && srcRowCount >= 0 && srcColumnCount >= 0 && (srcStartIndex + (bytesPerPixel * srcRowCount * srcColumnCount)) <= srcBytes.Length);
            var dstRowCount = srcRowCount / 2;
            var dstColumnCount = srcColumnCount / 2;
            Debug.Assert(dstStartIndex >= 0 && (dstStartIndex + (bytesPerPixel * dstRowCount * dstColumnCount)) <= dstBytes.Length);
            for (var dstRowIndex = 0; dstRowIndex < dstRowCount; dstRowIndex++)
                for (var dstColumnIndex = 0; dstColumnIndex < dstColumnCount; dstColumnIndex++)
                {
                    var srcRowIndex0 = 2 * dstRowIndex;
                    var srcColumnIndex0 = 2 * dstColumnIndex;
                    var srcPixel0Index = (srcColumnCount * srcRowIndex0) + srcColumnIndex0;

                    var srcPixelStartIndices = new int[4];
                    srcPixelStartIndices[0] = srcStartIndex + (bytesPerPixel * srcPixel0Index); // top-left
                    srcPixelStartIndices[1] = srcPixelStartIndices[0] + bytesPerPixel; // top-right
                    srcPixelStartIndices[2] = srcPixelStartIndices[0] + (bytesPerPixel * srcColumnCount); // bottom-left
                    srcPixelStartIndices[3] = srcPixelStartIndices[2] + bytesPerPixel; // bottom-right

                    var dstPixelIndex = (dstColumnCount * dstRowIndex) + dstColumnIndex;
                    var dstPixelStartIndex = dstStartIndex + (bytesPerPixel * dstPixelIndex);
                    for (var componentIndex = 0; componentIndex < componentCount; componentIndex++)
                    {
                        float averageComponent = 0;
                        for (var srcPixelIndex = 0; srcPixelIndex < srcPixelStartIndices.Length; srcPixelIndex++)
                            averageComponent += srcBytes[srcPixelStartIndices[srcPixelIndex] + componentIndex];
                        averageComponent /= srcPixelStartIndices.Length;
                        dstBytes[dstPixelStartIndex + componentIndex] = (byte)Mathf.RoundToInt(averageComponent);
                    }
                }
        }
    }
}