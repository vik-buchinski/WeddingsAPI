using System;

namespace WeddingAPI.Utils
{
    public class Common
    {

        public static String GenerateImageLink(int imageId, String leftUrlPart)
        {
            return leftUrlPart +
                   Constants.IMAGE_DOWNLOAD_URL + imageId;
        }
    }
}