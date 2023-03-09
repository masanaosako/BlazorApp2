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
    public interface IPurchasedAssetService
    {
        Task<List<AssetUsage>> GetPurchasedAssets();
        Task<AssetUsage?> GetPurchasedAsset(int id);
        void PostPurchasedAssetUsage(AssetUsage assetUsage);
        void PostPurchasedAssetMaster(AssetMaster assetMaster);
        Task Save();
    }

    public class PurchasedAssetService : IPurchasedAssetService
    {

        private readonly AppDbContext _context;

        public PurchasedAssetService(IDbContextFactory<AppDbContext> context)
        {
            _context = context.CreateDbContext();
        }

        public async Task<List<AssetUsage>> GetPurchasedAssets()
        {
            return await _context.AssetUsages
                .Include(au => au.AssetMaster)
                .Select(au => new AssetUsage
                {
                    ID = au.ID,
                    AssetCode = au.AssetCode,
                    State = au.State,
                    UpdateDate = au.UpdateDate,
                    AssetMaster = new AssetMaster
                    {
                        ID = au.AssetMaster.ID,
                        AssetCode = au.AssetMaster.AssetCode,
                        SerialNumber = au.AssetMaster.SerialNumber,
                    }
                })
                .GroupBy(au => au.AssetCode)
                .Select(g => g.OrderByDescending(au => au.ID).First())
                .ToListAsync();

        }

        public async Task<AssetUsage?> GetPurchasedAsset(int id)
        {
            var assetUsage = await _context.AssetUsages
                    .Include(au => au.AssetMaster)
                    .Select(au => new AssetUsage
                    {
                        ID = au.ID,
                        AssetCode = au.AssetCode,
                        State = au.State,
                        UpdateDate = au.UpdateDate,
                        AssetMaster = new AssetMaster
                        {
                            ID = au.AssetMaster.ID,
                            AssetCode = au.AssetMaster.AssetCode,
                            SerialNumber = au.AssetMaster.SerialNumber,
                        }
                    })
                    .Where(au => au.ID == id)
                    .FirstOrDefaultAsync();

            return assetUsage;
        }


        public void PostPurchasedAssetUsage(AssetUsage assetUsage)
        {
            _context.AssetUsages.Add(assetUsage);
        }


        public void PostPurchasedAssetMaster(AssetMaster assetMaster)
        {
            _context.AssetMasters.Add(assetMaster);
        }


        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

