using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
namespace store_app_apis.Repos
{
  
    public static class UniqueKeyGenerator
    {
        /// <summary>
        /// Generates a unique key with a prefix and sequential number.
        /// </summary>
        public static async Task<string> GenerateKeyAsync(DbContext context, string prefix)
        {
            var tracker = await context.Set<UniqueKeyTracker>().FirstOrDefaultAsync(t => t.Prefix == prefix);

            if (tracker == null)
            {
                tracker = new UniqueKeyTracker { Prefix = prefix, LastNumber = 1 };
                context.Set<UniqueKeyTracker>().Add(tracker);
            }

            // Format the unique key: PREFIX + 8-digit sequential number
            string uniqueKey = $"{prefix}{tracker.LastNumber:D8}";

            // Increment for next usage
            tracker.LastNumber++;

            await context.SaveChangesAsync();

            return uniqueKey;
        }

        /// <summary>
        /// Resets the unique key counter for a given prefix.
        /// </summary>
        public static async Task ResetKeyCounterAsync(DbContext context, string prefix)
        {
            var tracker = await context.Set<UniqueKeyTracker>().FirstOrDefaultAsync(t => t.Prefix == prefix);
            if (tracker != null)
            {
                tracker.LastNumber = 1;
                await context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Tracking table model to store last unique key value for each prefix.
    /// </summary>
    public class UniqueKeyTracker
    {
        public string Prefix { get; set; } // Example: "CUST", "EMP", "INV", etc.
        public int LastNumber { get; set; } // Sequential number
    }



}
