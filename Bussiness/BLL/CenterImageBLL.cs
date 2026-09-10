using Data;
using DataLayer;
namespace Bussiness
{
    public class CenterImageBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private CenterImage _image;
        public int _imageID = -1;

        public CenterImageBLL()
        {
            _image = new CenterImage();
            _mode = enMode.AddMode;
        }

        public CenterImageBLL(CenterImage img)
        {
            _image = img;
            _mode = enMode.UpdateMode;
        }

        public CenterImage CurrentImage { get => _image; set => _image = value; }

        public bool Add()
        {
            _imageID = CenterImageDLL.Add(_image);
            return _imageID > 0;
        }

        public bool Update() => CenterImageDLL.Update(_image);

        public bool Delete(int id) => CenterImageDLL.Delete(id);

        public static Task<CenterImage?> GetByID(int id) => CenterImageDLL.GetByID(id);

        public static Task<List<CenterImage>> GetAll() => CenterImageDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
