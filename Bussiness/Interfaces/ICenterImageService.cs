using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface ICenterImageService
    {
        Task<bool> Add(CenterImage image);
        Task<bool> Update(CenterImage image);
        Task<bool> Delete(int id);
        Task<List<CenterImage>> GetAll();
        Task<CenterImage?> GetByID(int id);
        int LastId { get; }

        Task<ImageUploadResult> Upload(int centerId, Stream input, bool isMain);
        Task<bool> DeleteWithFiles(CenterImage image);
    }
    public enum ImageUploadStatus { Success, InvalidImage, Failed }

    public class ImageUploadResult
    {
        public ImageUploadStatus Status { get; private init; }
        public CenterImage? Image { get; private init; }

        public static ImageUploadResult Ok(CenterImage image)
            => new() { Status = ImageUploadStatus.Success, Image = image };

        public static ImageUploadResult Invalid()
            => new() { Status = ImageUploadStatus.InvalidImage };

        public static ImageUploadResult Failed()
            => new() { Status = ImageUploadStatus.Failed };
    }
}
