using System.ComponentModel.DataAnnotations;

namespace Product
{
    public enum SwitchMethod
    {
        [Display(Name = "Messaging")]
        Message = 0,

        [Display(Name = "Input Simulation")]
        InputSimulation = 1
    }
}