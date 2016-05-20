using System.ComponentModel.DataAnnotations;

namespace Product
{
    public enum OverlayLocation
    {
        None,

        [Display(Name = "Top Left")]
        TopLeft,

        [Display(Name = "Top Center")]
        TopCenter,

        [Display(Name = "Top Right")]
        TopRight,

        [Display(Name = "Middle Left")]
        MiddleLeft,

        [Display(Name = "Middle Center")]
        MiddleCenter,

        [Display(Name = "Middle Right")]
        MiddleRight,

        [Display(Name = "Bottom Left")]
        BottomLeft,

        [Display(Name = "Bottom Center")]
        BottomCenter,

        [Display(Name = "Bottom Right")]
        BottomRight
    }
}