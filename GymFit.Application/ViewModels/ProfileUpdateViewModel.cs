using System.ComponentModel.DataAnnotations;

namespace GymFit.Application.ViewModels;

public class ProfileUpdateViewModel
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string EmergencyContact { get; set; } = string.Empty;
}
