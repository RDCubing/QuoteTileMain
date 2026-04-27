using System;
using QuoteTile.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace QuoteTile.Services
{
    public sealed class FavoriteService
    {
        private static FavoriteService _instance;
        public static FavoriteService Instance => _instance ?? (_instance = new FavoriteService());

        private const string FILE_NAME = "favorites.json";

        private FavoriteService() { }

        public async Task<List<QuoteModel>> GetFavoritesAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FILE_NAME);
                string json = await FileIO.ReadTextAsync(file);
                var list = JsonConvert.DeserializeObject<List<QuoteModel>>(json);
                return list ?? new List<QuoteModel>();
            }
            catch (Exception ex)
            {
                // If file is missing or corrupted, log error and return empty list
                System.Diagnostics.Debug.WriteLine($"GetFavoritesAsync failed: {ex}");
                return new List<QuoteModel>();
            }
        }

        public async Task AddFavoriteAsync(QuoteModel quote)
        {
            try
            {
                var list = await GetFavoritesAsync();

                // Avoid duplicates
                if (!list.Any(q => q.Content == quote.Content && q.Author == quote.Author))
                {
                    list.Add(quote);
                    await SaveAsync(list);
                }
            }
            catch (Exception ex)
            {
                // Log error so app won't crash
                System.Diagnostics.Debug.WriteLine($"AddFavoriteAsync failed: {ex}");
                throw; // optional: rethrow so the UI can show error dialog
            }
        }

        public async Task RemoveFavoriteAsync(QuoteModel quote)
        {
            try
            {
                var list = await GetFavoritesAsync();
                list.RemoveAll(q => q.Content == quote.Content && q.Author == quote.Author);
                await SaveAsync(list);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveFavoriteAsync failed: {ex}");
                throw;
            }
        }

        private async Task SaveAsync(List<QuoteModel> list)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
                string json = JsonConvert.SerializeObject(list);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }
    }
}