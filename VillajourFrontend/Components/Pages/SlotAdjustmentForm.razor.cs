using Microsoft.AspNetCore.Components;
using Radzen;
using System;

namespace VillajourFrontend.Components
{
    public class SlotAdjustmentModel
    {
        public TimeOnly MorningStart { get; set; }
        public TimeOnly MorningEnd { get; set; }
        public TimeOnly AfternoonStart { get; set; }
        public TimeOnly AfternoonEnd { get; set; }
    }

    public partial class SlotAdjustmentFormBase : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Parameter]
        public TimeOnly MorningStart { get; set; }

        [Parameter]
        public TimeOnly MorningEnd { get; set; }

        [Parameter]
        public TimeOnly AfternoonStart { get; set; }

        [Parameter]
        public TimeOnly AfternoonEnd { get; set; }

        protected SlotAdjustmentModel model = new SlotAdjustmentModel();

        protected override void OnParametersSet()
        {
            model.MorningStart = MorningStart;
            model.MorningEnd = MorningEnd;
            model.AfternoonStart = AfternoonStart;
            model.AfternoonEnd = AfternoonEnd;
        }

        protected void OnSubmit()
        {
            DialogService.Close(model);
        }
    }
}
