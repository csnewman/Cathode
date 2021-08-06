using System;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Common.Settings
{
    public interface ISettingsDbProvider<T> where T : ICloneable
    {
        DbSet<SettingsEntry<T>> Settings { get; set; }
    }
}