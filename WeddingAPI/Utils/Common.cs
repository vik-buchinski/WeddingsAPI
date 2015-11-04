using System;
using System.Collections.Generic;
using System.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Models.Database.Common;
using WeddingAPI.Models.Requests.Common;

namespace WeddingAPI.Utils
{
    public class Common
    {
        public static String GenerateImageLink(int? imageId, String leftUrlPart)
        {
            if (null != imageId && !String.IsNullOrEmpty(leftUrlPart))
            {
                return leftUrlPart +
                       Constants.IMAGE_DOWNLOAD_URL + imageId;
            }
            return null;
        }

        public static List<RequestImageModel> GetAlbumImages(Repositories dataRepositories, String leftUrlPart,
            bool isAdmin, int albumId)
        {
            var requestImages = new List<RequestImageModel>();
            var images =
                dataRepositories.ImagesModelRepository.Get(
                    f => f.AlbumId == albumId);
            if (images == null)
            {
                return null;
            }
            foreach (var image in images)
            {
                if (isAdmin)
                {
                    requestImages.Add(new RequestImageModel
                                      {
                                          Id = image.Id,
                                          Description = image.Description,
                                          ImageUrl = GenerateImageLink(image.Id,
                                              leftUrlPart)
                                      });
                }
                else
                {
                    requestImages.Add(new RequestImageModel
                                      {
                                          Id = image.Id,
                                          Description = image.Description,
                                          ImageUrl = GenerateImageLink(image.Id,
                                              leftUrlPart),
                                          Width = image.Width,
                                          Height = image.Height
                                      });
                }
            }
            return requestImages;
        }

        public static RequestAlbumModel BuildRequestAlbumModel(Repositories dataRepositories, String leftUrlPart,
            AlbumModel albumModel, Boolean isAdmin)
        {

            return new RequestAlbumModel
                   {
                       AlbumDescription = albumModel.AlbumDescription,
                       AlbumName = albumModel.AlbumName,
                       Id = albumModel.Id,
                       Images = GetAlbumImages(
                           dataRepositories,
                           leftUrlPart,
                           isAdmin,
                           albumModel.Id),
                       IsExpanded = albumModel.IsExpanded,
                       MainImage = GenerateImageLink(albumModel.ImageId,
                           leftUrlPart),
                       AlbumType = albumModel.AlbumType.ToLower(),
                       IsVisible = albumModel.IsVisible
                   };
        }

        public static IList<RequestAlbumModel> BuildRequestAlbumsList(Repositories dataRepositories, String leftUrlPart,
            IEnumerable<AlbumModel> albumsList)
        {
            return albumsList.Select(albumModel => new RequestAlbumModel
                                                   {
                                                       AlbumDescription = albumModel.AlbumDescription,
                                                       AlbumName = albumModel.AlbumName,
                                                       Id = albumModel.Id,
                                                       MainImage = GenerateImageLink(albumModel.ImageId, leftUrlPart),
                                                       IsVisible = albumModel.IsVisible
                                                   }).ToList();
        }

        public static bool IsAlbumTypeExist(String albumType)
        {
            if (!String.IsNullOrEmpty(albumType))
            {
                var array = Enum.GetNames(typeof (Constants.AlbumTypes)).ToList();
                if (array.Contains(albumType.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}