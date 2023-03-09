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
//using NuGet.ContentModel;
//using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BlazorApp2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : Controller
    {
        //private readonly AppDbContext _context;
        private readonly IAssetUsageService _service;

        public AssetController(IAssetUsageService service)
        {
            _service = service;
        }


        // GET: api/asset/usage
        [HttpGet("usage")]
        public async Task<ActionResult<List<AssetUsage>>> GetLeasedAssets()
        {
            var assets = await _service.GetLeasedAssets();
            if (assets == null) return NotFound();
            return assets;
        }


        // GET: api/asset/usage/5
        [HttpGet("usage/{id}")]
        public async Task<ActionResult<AssetUsage>> GetLeasedAsset(int id)
        {
            var assetUsage = await _service.GetLeasedAsset(id);
            if (assetUsage == null) return NotFound();
            return assetUsage;
        }


        // GET: api/asset/history
        [HttpGet("history")]
        public async Task<ActionResult<List<AssetUsage>>> GetUpdateHistory()
        {
            var history = await _service.GetAllRecords();
            if (history == null) return NotFound();
            return history;
        }

        // POST: api/asset/usage
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("usage")]
        public async Task<ActionResult<AssetUsage>> PostAssetUsage(AssetUsage assetUsage)
        {
            _service.PostAssetUsage(assetUsage);
            await _service.Save();
            return CreatedAtAction(nameof(GetLeasedAsset), new { id = assetUsage.ID }, assetUsage);
        }


        // POST: api/asset/import/leases
        [HttpPost("import/leases")]
        public async Task<IActionResult> PostAssetLeases(IFormFile file)
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
                    recordMaster.ReferenceID = csv.GetField("レンタル番号");
                    recordMaster.Agency = csv.GetField("リース会社");
                    recordMaster.ExpireDate = csv.GetField<DateTime?>("リース期限");
                    recordMaster.Fee = csv.GetField<Decimal?>("リース料金");

                    recordUsage.AssetCode = csv.GetField("機体番号");
                    recordUsage.State = csv.GetField("ステータス");
                    recordUsage.Fault = csv.GetField<bool?>("故障");
                    recordUsage.UpdateDate = DateTime.Now;

                    _service.PostAssetMaster(recordMaster);
                    _service.PostAssetUsage(recordUsage);
                }
                await _service.Save();
                return NoContent();
            }
        }

        // POST: api/asset/import/usages
        [HttpPost("import/usages")]
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

                        _service.PostAssetUsage(recordUsage);
                    }
                }
            }
            await _service.Save();
            return NoContent();
        }
    }
}
