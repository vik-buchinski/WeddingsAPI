namespace WeddingAPI.Utils
{
    public class Constants
    {
        public const string CLIENT_URL = "http://designforlife.pl";
        //public const string CLIENT_URL = "http://localhost:9000";
        public const string IMG_UPLOADS_PATH = "/App_Data/uploads/images";

        public const string IMAGE_DOWNLOAD_URL = "/api/images/";

        public const string SESSION_TOKEN_HEADER_KEY = "Session-Token";

        public enum AlbumTypes
        {
            BOUQUETS,
            DECORATIONS,
            INVITATIONS,
            GRAPHIC
        };

        public enum TitleImagesTypes
        {
            ABOUT
        };
    }
}