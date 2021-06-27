using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Common.Settings
{
    public class SettingsProvider<TDatabase, TSetting> : ISettingsProvider<TDatabase, TSetting>
        where TDatabase : DbContext, ISettingsDbProvider<TSetting>
    {
        private readonly TDatabase _db;

        public SettingsProvider(TDatabase db)
        {
            _db = db;
        }

        public async Task<T?> GetAsync<T>(string id) where T : TSetting
        {
            var result = await _db.Settings.SingleOrDefaultAsync(
                x => x.Id == id
            );

            if (result == null)
            {
                return default;
            }

            if (result.Value is not T)
            {
                throw new Exception(
                    $"Database contained incompatible type {result.Value?.GetType()} when expecting {typeof(T)}"
                );
            }

            return (T)result.Value!;
        }

        public async Task<T> GetOrAddAsync<T>(string id, Func<T> initializer) where T : TSetting
        {
            var result = await GetAsync<T>(id);
            if (result != null)
            {
                return result;
            }

            var entry = await _db.Settings.AddAsync(new SettingsEntry<TSetting>
            {
                Id = id,
                Value = initializer(),
            });
            await _db.SaveChangesAsync();
            return (T)entry.Entity.Value!;
        }
    }
}