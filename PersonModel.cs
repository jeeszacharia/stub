using System.ComponentModel.DataAnnotations;

namespace CIAMstubAPI
{
    public class PersonModel
    {
        [Key]
        public int clientNumber { get; set; }
        public string? contactType { get; set; }
        public int? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? ciamObjectId { get; set; }
        public bool? IsTermsAccepted { get; set; }
        public string? acceptedTermsVersion { get; set; }
        public string? currentTermsVersion { get; set; }
        public DateOnly? termsacceptanceDate { get; set; }
    }
}
