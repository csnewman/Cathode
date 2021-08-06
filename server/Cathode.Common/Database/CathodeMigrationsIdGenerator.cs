using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cathode.Common.Database
{
    public class CathodeMigrationsIdGenerator : IMigrationsIdGenerator
    {
        private const string Prefix = "C1-";

        public string GenerateId(string name)
        {
            return $"{Prefix}{name}";
        }

        public string GetName(string id)
        {
            if (!IsValidId(id))
            {
                throw new ArgumentException("Invalid id", nameof(id));
            }

            return id[(Prefix.Length + 1)..];
        }

        public bool IsValidId(string value)
        {
            return value.StartsWith(Prefix);
        }
    }
}