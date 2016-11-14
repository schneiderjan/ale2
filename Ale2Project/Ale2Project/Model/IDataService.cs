using System.Threading.Tasks;

namespace Ale2Project.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}