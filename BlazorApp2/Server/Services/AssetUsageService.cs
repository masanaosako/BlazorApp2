using System;
using System.Globalization;
using BlazorApp2.Server.Data;
using BlazorApp2.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;
using Microsoft.SqlServer.Server;

namespace BlazorApp2.Server.Services
{
    public interface IAssetUsageService
    {
        Task<List<AssetUsage>> GetLeasedAssets();
        Task<AssetUsage?> GetLeasedAsset(int id);
        Task<List<AssetUsage>> GetAllRecords();
        void PostAssetUsage(AssetUsage assetUsage);
        void PostAssetMaster(AssetMaster assetMaster);
        Task Save();
    }

    public class AssetUsageService : IAssetUsageService
    {

        private readonly AppDbContext _context;

        public AssetUsageService(IDbContextFactory<AppDbContext> context)
        {
            _context = context.CreateDbContext();
        }

        public async Task<List<AssetUsage>> GetLeasedAssets()
        {
            return await _context.AssetUsages
                .Include(au => au.AssetMaster)
                .GroupBy(au => au.AssetCode)
                .Select(g => g.OrderByDescending(au => au.ID).First())
                .ToListAsync();
        }

        public async Task<AssetUsage?> GetLeasedAsset(int id)
        {
            var assetUsage = _context.AssetUsages.Find(id);
            return assetUsage;
        }


        public async Task<List<AssetUsage>> GetAllRecords()
        {
            return await _context.AssetUsages
                .Include(au => au.AssetMaster)
                .ToListAsync();
        }


        public void PostAssetUsage(AssetUsage assetUsage)
        {
            _context.AssetUsages.Add(assetUsage);
        }


        public void PostAssetMaster(AssetMaster assetMaster)
        {
            _context.AssetMasters.Add(assetMaster);
        }


        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

