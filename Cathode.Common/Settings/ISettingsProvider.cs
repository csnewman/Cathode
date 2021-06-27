using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Common.Settings
{
    public interface ISettingsProvider<TDatabase, in TSetting> where TDatabase : DbContext, ISettingsDbProvider<TSetting>
    {
        Task<T?> GetAsync<T>(string id) where T : TSetting;

        Task<T> GetOrAddAsync<T>(string id, Func<T> initializer) where T : TSetting;
    }
}