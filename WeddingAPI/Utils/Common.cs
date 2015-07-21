using System;
using System.Collections.Generic;
using WeddingAPI.DAL;
using WeddingAPI.Models.Requests.Admin.Bouquets;

namespace WeddingAPI.Utils
{
    public class Common
    {
        public static String GenerateImageLink(int imageId, String leftUrlPart)
        {
            return leftUrlPart +
                   Constants.IMAGE_DOWNLOAD_URL + imageId;
        }

        public static List<BouquetImageModel> GetBouquetImages(Repositories dataRepositories, String leftUrlPart, bool isAdmin)
        {
            var bouquets = new List<BouquetImageModel>();
            var images =
                dataRepositories.ImagesModelRepository.Get(
                    f => f.AlbumType.Equals(Constants.AlbumTypes.BOUQUETS.ToString()));
            if (images == null)
            {
                return null;
            }
            foreach (var image in images)
            {
                if (isAdmin)
                {
                    bouquets.Add(new BouquetImageModel
                    {
                        Id = image.Id,
                        Description = image.Description,
                        ImageUrl = GenerateImageLink(image.Id,
                            leftUrlPart)
                    });
                }
                else
                {
                    bouquets.Add(new BouquetImageModel
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
            return bouquets;
        }
    }
}