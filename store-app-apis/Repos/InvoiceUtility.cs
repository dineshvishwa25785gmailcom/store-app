
using Humanizer;  // Install the Humanizer NuGet package if not already installed
using System;

namespace store_app_apis.Repos
{
    public static class InvoiceUtility
    {
        /// <summary>
        /// Converts numeric amount to words (e.g., "INR Four Thousand Seventy-Three Only").
        /// </summary>
        public static string ConvertAmountToWords(decimal amount)
        {
            if (amount == 0) return "Zero Rupees";

            int integerPart = (int)Math.Floor(amount);  // ✅ Extracts whole number part
            string words = integerPart.ToWords();  // ✅ Converts integer part to words

            return $"INR {words} Only";  // ✅ Ensures correct formatting
        }

        /// <summary>
        /// Formats a decimal value as a currency string (e.g., "₹ 4,073.04").
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return $"₹ {amount:F2}";  // ✅ Ensures two decimal places with currency symbol
        }
    }







}
