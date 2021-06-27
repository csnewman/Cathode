using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Common.Settings
{
    public class SettingsEntry<T>
    {
        public string Id { get; set; }

        [Column(TypeName = "jsonb")]
        public T Value { get; set; }

        public static void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<SettingsEntry<T>>(builder =>
            {
                builder
                    .ToTable("settings")
                    .HasKey(x => x.Id);
            });
        }
    }
}