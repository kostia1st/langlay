using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Product.Common
{
    public enum SwitchMethod
    {
        [Display(Name = "Messaging")]
        Message = 0,
        [Display(Name = "Input Simulation")]
        InputSimulation = 1
    }
}
