using System.ComponentModel.DataAnnotations;
#pragma warning disable 1591//Ignore xml comments

namespace wsIntellinx.ViewModels.Params
{
    public class KbaMemberParam
    {
        [Required]
        public string BarId { get; set; }
        [Required]
        public string KbaUserId { get; set; }
        [Required]
        public string KbaPassword { get; set; }
    }
}
