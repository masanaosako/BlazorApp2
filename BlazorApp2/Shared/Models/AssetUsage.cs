using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace BlazorApp2.Shared.Models
{
	[Table("TBL_AssetUsage", Schema = "dbo")]
    public class AssetUsage : IValidatableObject
	{
		[Key]
		public int ID { get; set; }

        [Required(ErrorMessage = "この項目は入力必須です。")]
        [StringLength(12, ErrorMessage = "12文字以内で入力してください。")]
        public string? AssetCode { get; set; }

		[Required(ErrorMessage = "この項目は入力必須です。")]
        [StringLength(12, ErrorMessage = "12文字以内で入力してください。")]
        public string? State { get; set; }

        public bool? Fault { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "yyyy/MM/dd")]
        public DateTime? UpdateDate { get; set; }

        public AssetMaster? AssetMaster { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var assetUsage = (AssetUsage)validationContext.ObjectInstance;

            if (assetUsage.State == "Active" && assetUsage.Fault == true)
            {
                yield return new ValidationResult(
                    "ステータスがActiveの場合は故障にチェックを入れないでください。",
                    new[] { nameof(assetUsage.Fault) });
            }
            if (assetUsage.AssetCode.StartsWith("8000") && assetUsage.Fault == true)
            {
                yield return new ValidationResult(
                    "機体番号が'8000'で始まる場合は故障にチェックを入れないでください。",
                    new[] { nameof(assetUsage.Fault) });
            }
        }
	}

    public sealed class AssetUsageMap : ClassMap<AssetUsage>
    {
        public AssetUsageMap()
        {
            Map(t => t.ID).Ignore();
            Map(t => t.AssetCode).Name("機体番号").Validate(f =>
            {
                if (string.IsNullOrEmpty(f.Field)) return false;
                if (f.Field.Length > 12) return false;
                return true;
            });
            Map(t => t.State).Name("ステータス").Validate(f =>
            {
                if (string.IsNullOrEmpty(f.Field)) return false;
                if (f.Field.Length > 12) return false;
                return true;
            });
            Map(t => t.Fault).Name("故障");
            Map(t => t.UpdateDate).Name("更新日時");
            References<AssetMasterMap>(t => t.AssetMaster);
        }
    }
}

