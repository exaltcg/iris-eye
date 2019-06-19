using System.Collections.Generic;
using System.Threading.Tasks;
using IrisEye.Core.Entities;

namespace IrisEye.Web.Providers
{
    public interface IFoldersProvider
    {
        Task<long> AddNewFolder(Folder folder);
        Task<IList<Folder>> GetAllFolders();

        Task<Folder> GetActiveFolder();
        Task<Folder> GetFolder(int id);
    }
}