using Data;
using DataLayer;
namespace Bussiness
{
    public class ServiceBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Service _service;
        public int _serviceID = -1;

        public ServiceBLL()
        {
            _service = new Service();
            _mode = enMode.AddMode;
        }

        public ServiceBLL(Service service)
        {
            _service = service;
            _mode = enMode.UpdateMode;
        }

        public Service CurrentService { get => _service; set => _service = value; }

        public bool Add()
        {
            _serviceID = ServiceDLL.Add(_service);
            return _serviceID > 0;
        }

        public bool Update() => ServiceDLL.Update(_service);

        public bool Delete(int serviceId) => ServiceDLL.Delete(serviceId);

        public static Task<Service?> GetByID(int serviceId) => ServiceDLL.GetByID(serviceId);

        public static Task<List<Service>> GetAll() => ServiceDLL.GetAll();

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
