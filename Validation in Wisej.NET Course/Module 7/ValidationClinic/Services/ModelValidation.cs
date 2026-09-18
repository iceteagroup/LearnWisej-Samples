using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationClinic.Models;

namespace ValidationClinic.Services
{
    public static class ModelValidation
    {
        public static List<ValidationResult> ValidateModel(ContactEditModel model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
            return results;
        }
    }
}
