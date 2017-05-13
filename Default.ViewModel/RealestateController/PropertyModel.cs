using System.ComponentModel.DataAnnotations;

namespace Default.ViewModel.RealestateController
{
    public class PropertyModel
    {
        public string Id { get; set; }

        public uint? ZillowId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Value { get; set; }

        public bool IsIncludedInWorth { get; set; }
    }
}