using System.ComponentModel;

namespace ValidationClinic.Models
{
    public sealed class ContactErrorModel : ContactEditModel, IDataErrorInfo
    {
        public string Error => "";
        public string this[string columnName] => columnName switch
        {
            nameof(Name) when string.IsNullOrWhiteSpace(Name) => "Name is required.",
            nameof(Email) when string.IsNullOrWhiteSpace(Email) || !Email.Contains('@') => "Enter an email address containing @.",
            nameof(Age) when Age is null or < 0 or > 120 => "Enter an age from 0 to 120.",
            _ => ""
        };
    }
}
