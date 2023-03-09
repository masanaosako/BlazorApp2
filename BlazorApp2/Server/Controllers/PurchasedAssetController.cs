using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorApp2.Server.Data;
using BlazorApp2.Server.Services;
using BlazorApp2.Shared.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.TypeConversion;
using NuGet.ContentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BlazorApp2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedAssetController : ControllerBase
    {
        //private readonly AppDbContext _context;
        private readonly IPurchasedAssetService _service;

        public PurchasedAssetController(IPurchasedAssetService service)
        {
            _service = service;
        }

        // GET: api/asset/usage
        [HttpGet]
        public async Task<ActionResult<List<AssetUsage>>> GetPurchasedAssets()
        {
            var purchasedAssets = await _service.GetPurchasedAssets();
            if (purchasedAssets == null) return NotFound();
            return purchasedAssets;
        }


        // GET: api/asset/usage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetUsage>> GetPurchasedAsset(int id)
        {
            var purchasedAsset = await _service.GetPurchasedAsset(id);
            if (purchasedAsset == null) return NotFound();
            return purchasedAsset;
        }


        // POST: api/asset/usage
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssetUsage>> PostPurchasedAssetUsage(AssetUsage assetUsage)
        {
            _service.PostPurchasedAssetUsage(assetUsage);
            await _service.Save();
            return CreatedAtAction(nameof(GetPurchasedAsset), new { id = assetUsage.ID }, assetUsage);
        }


        // POST: api/asset/import/add
        [HttpPost("import/add")]
        public async Task<IActionResult> PostPurchasedAsset(IFormFile file)
        {
            if (file == null)
            {
                return Problem("File is unable to be uploaded.");
            }

            var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var recordMaster = new AssetMaster();
                    var recordUsage = new AssetUsage();

                    recordMaster.AssetCode = csv.GetField("機体番号");
                    recordMaster.SerialNumber = csv.GetField("シリアル番号");

                    recordUsage.AssetCode = csv.GetField("機体番号");
                    recordUsage.State = csv.GetField("ステータス");
                    recordUsage.Fault = csv.GetField<bool?>("故障");
                    recordUsage.UpdateDate = DateTime.Now;

                    _service.PostPurchasedAssetMaster(recordMaster);
                    _service.PostPurchasedAssetUsage(recordUsage);
                }
                await _service.Save();
                return NoContent();
            }
        }

        // POST: api/asset/import/update
        [HttpPost("import/update")]
        public async Task<IActionResult> PostAssetUsages(IFormFile file)
        {
            if (file == null)
            {
                return Problem("File is unable to be uploaded.");
            }

            var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            using (var reader = new StreamReader(stream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var recordUsage = new AssetUsage();

                        recordUsage.AssetCode = csv.GetField("機体番号");
                        recordUsage.State = csv.GetField("ステータス");
                        recordUsage.Fault = csv.GetField<bool?>("故障");
                        recordUsage.UpdateDate = DateTime.Now;

                        _service.PostPurchasedAssetUsage(recordUsage);
                    }
                }
            }
            await _service.Save();
            return NoContent();
        }
    }
}
