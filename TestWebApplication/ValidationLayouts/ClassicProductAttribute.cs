using System.ComponentModel.DataAnnotations;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.ValidationLayouts
{
    public class ClassicProductAttribute : ValidationAttribute
    {
        public ClassicProductAttribute(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int? Id { get; set; }
        public string Name { get; set; }

        public string GetErrorMessage() => "Error";


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var obj = (ProductVM)validationContext.ObjectInstance;

            Id = obj.Product.Id;
            Name = obj.Product.Name;

            if (Id != null && Name != null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(GetErrorMessage());
        }

    }
}
