using Microsoft.EntityFrameworkCore;

namespace Cathode.Common.Settings
{
    public interface ISettingsDbProvider<T>
    {
        DbSet<SettingsEntry<T>> Settings { get; set; }
    }
}