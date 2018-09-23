using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BS.Output.VSO.Models;
using BS.Output.VSO.Properties;
using BS.Output.VSO.Services;
using BS.Plugin.V3.Common;
using BS.Plugin.V3.Output;

namespace BS.Output.VSO
{
    public class OutputAddIn :  OutputPlugin<VSOOutput>
    {
        protected override string Name => "Visual Studio Online";

        protected override string Description => "Visual Studio Online";

        protected override Image Image64 => new Bitmap(Resources.vso_64_32);

        protected override Image Image16 => new Bitmap(Resources.vso_16_16);

        protected override bool Editable => true;

        protected override VSOOutput CreateOutput(IWin32Window owner)
        {
            var objVsoOutput = new VSOOutput(Name);

            return EditOutput(owner, objVsoOutput);
        }

        protected override VSOOutput EditOutput(IWin32Window owner, VSOOutput vsoOutput)
        {
            using (var settingsForm = new EditOutputSettingsForm(new VSOOutput(vsoOutput)))
            {
                if (settingsForm.ShowDialog(owner) == DialogResult.OK)
                {
                    return settingsForm.Output;
                }

                return null;
            }
        }

        protected override OutputValues SerializeOutput(VSOOutput vsoOutput)
        {
            return vsoOutput.Serialize();
        }

        protected override VSOOutput DeserializeOutput(OutputValues outputValues)
        {
            return VSOOutput.Deserialize(outputValues, Name);
        }

        protected override async Task<SendResult> Send(IWin32Window owner, VSOOutput vsoOutput, ImageData imageData)
        {
            var client = new VSOClient(vsoOutput);
            await client.Connect();

            bool cancelled = false;
            var options = GetSendOptions(owner, vsoOutput, ref cancelled);
            if (!cancelled)
            {
                await client.CreateBug(options, imageData);
            }

            return new SendResult(cancelled ? Result.Canceled : Result.Success);
        }

        private static BugDetails GetSendOptions(IWin32Window owner, VSOOutput vsoOutput, ref bool cancel)
        {
            using (var bugDetailsForms = new EditBugDetailsForm(vsoOutput))
            {
                if (bugDetailsForms.ShowDialog(owner) == DialogResult.OK)
                    return bugDetailsForms.Options;
                cancel = true;
                return null;
            }
        }
    }
}