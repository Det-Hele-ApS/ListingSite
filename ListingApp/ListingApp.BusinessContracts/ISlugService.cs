using System.Threading.Tasks;

namespace ListingApp.BusinessContracts
{
	public interface ISlugService
    {
		Task<bool> IsSlugExists(string slug);
    }
}
