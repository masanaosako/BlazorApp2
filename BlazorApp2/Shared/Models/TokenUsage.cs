using System;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace BlazorApp2.Shared.Models
{
    [Table("TBL_TokenUsage", Schema = "dbo")]
    public class TokenUsage
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "この項目は入力必須です。")]
        public string AssetCode { get; set; }
        [Required(ErrorMessage = "この項目は入力必須です。")]
        public string Profile { get; set; }

        public AssetMaster? AssetMaster { get; set; }
    }

    public sealed class TokenUsageMap: ClassMap<TokenUsage>
    {
        public TokenUsageMap()
        {
            Map(t => t.ID).Ignore();
            Map(t => t.AssetCode).Name("機体番号").Ignore();
            Map(t => t.Profile).Name("プロファイル");
        }
    }
}
