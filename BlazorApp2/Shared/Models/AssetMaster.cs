using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace BlazorApp2.Shared.Models
{
    [Table("TBL_AssetMaster", Schema = "dbo")]
    public class AssetMaster
	{
        [Key]
        public int ID { get; set; }
        public string? AssetCode { get; set; }
        public string? SerialNumber { get; set; }
        public string? ReferenceID { get; set; }
        public string? Agency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd")]
        public DateTime? ExpireDate { get; set; }
        public decimal? Fee { get; set; }

        public List<AssetUsage>? AssetUsages { get; set; }
	}

    public sealed class AssetMasterMap : ClassMap<AssetMaster>
    {
        public AssetMasterMap()
        {
            Map(t => t.ID).Ignore();
            Map(t => t.AssetCode).Name("機体番号").Ignore();
            Map(t => t.SerialNumber).Name("シリアル番号");
            Map(t => t.ReferenceID).Name("レンタル番号");
            Map(t => t.Agency).Name("リース会社");
            Map(t => t.ExpireDate).Name("リース期限");
            Map(t => t.Fee).Name("リース料金");
        }
    }

}