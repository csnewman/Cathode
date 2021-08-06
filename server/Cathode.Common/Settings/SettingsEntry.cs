using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cathode.Common.Settings
{
    public class SettingsEntry<T> where T : ICloneable
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

                builder.Property(x => x.Value)
                    .Metadata
                    .SetValueComparer(new ValueComparer<T>(
                        (a, b) => a.Equals(b),
                        a => a.GetHashCode(),
                        a => (T)a.Clone()
                    ));
            });
        }
    }
}