using System;
using System.Collections.Generic;
using System.Linq;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Admin.Common;

namespace WeddingAPI.Utils
{
    public class Common
    {
        public static String GenerateImageLink(int imageId, String leftUrlPart)
        {
            return leftUrlPart +
                   Constants.IMAGE_DOWNLOAD_URL + imageId;
        }

        public static List<RequestImageModel> GetAlbumImages(Repositories dataRepositories, String leftUrlPart,
            bool isAdmin, String albumTypes)
        {
            var requestImages = new List<RequestImageModel>();
            var images =
                dataRepositories.ImagesModelRepository.Get(
                    f => f.AlbumType.Equals(albumTypes));
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

        public static bool IsAlbumTypeExist(String albumType)
        {
            var array = Enum.GetNames(typeof (Constants.AlbumTypes)).ToList();
            if (array.Contains(albumType.ToUpper()))
            {
                return true;
            }
            return false;
        }
    }
}