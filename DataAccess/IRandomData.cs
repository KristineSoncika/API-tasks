using System.Threading.Tasks;
using API_01.Models;
using Refit;

namespace API_01.DataAccess;

public interface IRandomData
{
    [Get("/random?auth=null")]
    Task<ApiResponse<PublicApiResponseModel>> GetRandomData();
}