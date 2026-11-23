using Bussiness.Interfaces;
using Data;
using DataLayer;
using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Bussiness
{
    public class CenterImageBLL : ICenterImageService
    {
        private static readonly int[] VariantWidths = { 300, 800, 1600 };
        private static readonly SemaphoreSlim ProcessingGate = new(1, 1);
        private const int MaxDimension = 12000;
        private const long MaxPixelCount = 40_000_000;
        private const string LargestSuffix = "-1600.webp";

        private readonly CenterImageDLL _centerImageDLL;

        public int _imageID { get; private set; }
        public int LastId => _imageID;

        public CenterImageBLL(CenterImageDLL centerImageDLL)
        {
            _centerImageDLL = centerImageDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(CenterImage image)
        {
            int imageID = await _centerImageDLL.Add(image);

            _imageID = imageID;

            return imageID > 0;
        }

        public async Task<bool> Update(CenterImage image)
            => await _centerImageDLL.Update(image);

        public async Task<bool> Delete(int id)
            => await _centerImageDLL.Delete(id);

        public async Task<List<CenterImage>> GetAll()
            => await _centerImageDLL.GetAll();
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<CenterImage?> GetByID(int id)
            => await _centerImageDLL.GetByID(id);

        // ================ Read By ================


        // ================ Upload ================
        [Obsolete]
        public async Task<ImageUploadResult> Upload(int centerId, Stream input, bool isMain)
        {
            await ProcessingGate.WaitAsync();
            try
            {
                return await UploadCore(centerId, input, isMain);
            }
            finally
            {
                ProcessingGate.Release();
            }
        }

        private async Task<ImageUploadResult> UploadCore(int centerId, Stream input, bool isMain)
        {
            using var codec = SKCodec.Create(input);
            if (codec == null || codec.EncodedFormat is not (SKEncodedImageFormat.Jpeg or SKEncodedImageFormat.Png or SKEncodedImageFormat.Webp))
                return ImageUploadResult.Invalid();

            var sourceInfo = codec.Info;
            if (sourceInfo.Width <= 0 || sourceInfo.Height <= 0
                || sourceInfo.Width > MaxDimension || sourceInfo.Height > MaxDimension
                || (long)sourceInfo.Width * sourceInfo.Height > MaxPixelCount)
                return ImageUploadResult.Invalid();

            using var decoded = SKBitmap.Decode(codec);
            if (decoded == null || decoded.Width <= 0 || decoded.Height <= 0
                || decoded.Width > MaxDimension || decoded.Height > MaxDimension
                || (long)decoded.Width * decoded.Height > MaxPixelCount)
                return ImageUploadResult.Invalid();

            using var oriented = codec.EncodedOrigin == SKEncodedOrigin.Default
                ? null
                : ApplyOrientation(decoded, codec.EncodedOrigin);
            var source = oriented ?? decoded;

            var folder = Path.Combine(GetUploadsRoot(), "centers", centerId.ToString());
            Directory.CreateDirectory(folder);
            var fileId = Guid.NewGuid().ToString("N");
            var variantPaths = new List<string>(VariantWidths.Length);
            var saved = false;

            try
            {
                foreach (var width in VariantWidths)
                {
                    var variantWidth = Math.Min(width, source.Width);
                    var variantHeight = Math.Max(1, (int)Math.Round((double)source.Height * variantWidth / source.Width));

                    var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
                    using var variant = source.Resize(source.Info.WithSize(variantWidth, variantHeight), sampling);
                    if (variant == null) return ImageUploadResult.Failed();

                    using var image = SKImage.FromBitmap(variant);
                    using var webp = image.Encode(SKEncodedImageFormat.Webp, 80);
                    if (webp == null) return ImageUploadResult.Failed();

                    var path = Path.Combine(folder, $"{fileId}-{width}.webp");
                    variantPaths.Add(path);
                    await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write,
                        FileShare.None, 64 * 1024, FileOptions.Asynchronous);
                    webp.SaveTo(output);
                    await output.FlushAsync();
                }

                var img = new CenterImage
                {
                    CenterId = centerId,
                    ImageUrl = $"/uploads/centers/{centerId}/{fileId}{LargestSuffix}",
                    IsMain = isMain,
                    CreatedAt = DateTime.Now
                };

                if (!await Add(img)) return ImageUploadResult.Failed();

                saved = true;
                return ImageUploadResult.Ok(img);
            }
            finally
            {
                if (!saved)
                {
                    foreach (var path in variantPaths)
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        [Obsolete]
        private static SKBitmap ApplyOrientation(SKBitmap decoded, SKEncodedOrigin origin)
        {
            var swapDimensions = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
                or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;

            var orientedWidth = swapDimensions ? decoded.Height : decoded.Width;
            var orientedHeight = swapDimensions ? decoded.Width : decoded.Height;

            var oriented = new SKBitmap(orientedWidth, orientedHeight);
            using var canvas = new SKCanvas(oriented);

            var matrix = SKMatrix.CreateIdentity();
            switch (origin)
            {
                case SKEncodedOrigin.TopRight:
                    matrix.ScaleX = -1;
                    matrix.TransX = decoded.Width;
                    break;
                case SKEncodedOrigin.BottomRight:
                    matrix.ScaleX = -1;
                    matrix.ScaleY = -1;
                    matrix.TransX = decoded.Width;
                    matrix.TransY = decoded.Height;
                    break;
                case SKEncodedOrigin.BottomLeft:
                    matrix.ScaleY = -1;
                    matrix.TransY = decoded.Height;
                    break;
                case SKEncodedOrigin.LeftTop:
                    matrix.ScaleX = 0;
                    matrix.SkewX = 1;
                    matrix.SkewY = 1;
                    matrix.ScaleY = 0;
                    break;
                case SKEncodedOrigin.RightTop:
                    matrix.ScaleX = 0;
                    matrix.SkewX = -1;
                    matrix.TransX = decoded.Height;
                    matrix.SkewY = 1;
                    matrix.ScaleY = 0;
                    break;
                case SKEncodedOrigin.RightBottom:
                    matrix.ScaleX = 0;
                    matrix.SkewX = -1;
                    matrix.TransX = decoded.Height;
                    matrix.SkewY = -1;
                    matrix.ScaleY = 0;
                    matrix.TransY = decoded.Width;
                    break;
                case SKEncodedOrigin.LeftBottom:
                    matrix.ScaleX = 0;
                    matrix.SkewX = 1;
                    matrix.SkewY = -1;
                    matrix.ScaleY = 0;
                    matrix.TransY = decoded.Width;
                    break;
            }

            canvas.SetMatrix(matrix);
            canvas.DrawBitmap(decoded, 0, 0);

            return oriented;
        }
        // ================ Upload ================


        // ================ Delete (DB + files) ================
        public async Task<bool> DeleteWithFiles(CenterImage image)
        {
            if (!await _centerImageDLL.Delete(image.ImageId)) return false;

            // best-effort file cleanup
            try
            {
                var imageUrl = image.ImageUrl.Replace('\\', '/');
                var parts = imageUrl.Split('/');

                if (parts.Length == 5 && parts[0].Length == 0 && parts[1] == "uploads" && parts[2] == "centers"
                    && int.TryParse(parts[3], out _)
                    && parts[4].EndsWith(LargestSuffix, StringComparison.OrdinalIgnoreCase)
                    && Guid.TryParseExact(parts[4][..^LargestSuffix.Length], "N", out _))
                {
                    var fileId = parts[4][..^LargestSuffix.Length];
                    foreach (var width in VariantWidths)
                        DeleteUploadFile($"/uploads/centers/{parts[3]}/{fileId}-{width}.webp");
                }
                else
                {
                    // legacy single-file image
                    DeleteUploadFile(image.ImageUrl);
                }
            }
            catch { }

            return true;
        }

        private static string GetUploadsRoot()
            => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        private static void DeleteUploadFile(string imageUrl)
        {
            var root = Directory.GetCurrentDirectory();
            var uploadsRoot = Path.GetFullPath(GetUploadsRoot());
            var relativePath = imageUrl.TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
            var physicalPath = Path.GetFullPath(Path.Combine(root, "wwwroot", relativePath));
            var pathFromUploads = Path.GetRelativePath(uploadsRoot, physicalPath);

            if (Path.IsPathRooted(pathFromUploads) || pathFromUploads == ".."
                || pathFromUploads.StartsWith(".." + Path.DirectorySeparatorChar))
                return;

            if (System.IO.File.Exists(physicalPath)) System.IO.File.Delete(physicalPath);
        }
        // ================ Delete (DB + files) ================
    }
}