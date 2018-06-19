using System.Threading.Tasks;

namespace ListingApp.BusinessContracts.Services
{
	public interface IAzureUploadService
    {
		Task<string> Upload(string connectionString, string name, byte[] data);
    }
}
