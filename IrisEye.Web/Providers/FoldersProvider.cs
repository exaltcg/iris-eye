using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Web.Data;
using IrisEye.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace IrisEye.Web.Providers
{
    public class FoldersProvider:IFoldersProvider
    {
        private readonly ApplicationDbContext _db;

        public FoldersProvider(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<long> AddNewFolder(Folder folder)
        {
            var folderExists = await _db.Folders.CountAsync(p => p.Name == folder.Name);
            if (folderExists!=0) throw new Exception("Folder with this name was already added.");
            folder.CreatedOn = DateTime.Now;
            if (folder.IsActive)
            {
                var allFolders = await GetAllFolders();
                foreach (var f in allFolders)
                {
                    f.IsActive = false;
                }
            }
           await _db.Folders.AddAsync(folder);
           await _db.SaveChangesAsync();
           return folder.Id;
        }

        public async Task<IList<Folder>> GetAllFolders()
        {
            return await _db.Folders.ToListAsync();
        }

        public async Task<Folder> GetActiveFolder()
        {
            return await _db.Folders
                .Include(p=>p.Runs)
                .ThenInclude(p=>p.Tests)
                .FirstOrDefaultAsync(p => p.IsActive);
        }

        public async Task<Folder> GetFolder(int id)
        {
            return await _db.Folders.Include(p=>p.Runs).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}