using System;
using System.Drawing;
using System.Windows.Forms;
using BS.Output.VSO.Models;
using BS.Output.VSO.Properties;
using BS.Output.VSO.Services;

namespace BS.Output.VSO
{
    public class OutputAddIn : V2.OutputAddIn<VSOOutput, BugDetails>
    {
        protected override Guid ID => new Guid("3185C77F-1583-4A46-A8B7-7D0F69B7F2D8");

        protected override string Name => "Visual Studio Online";

        protected override string Description => "Visual Studio Online";

        protected override string GroupName => Name;

        protected override Image Image64x32 => new Bitmap(Resources.vso_64_32);

        protected override Image Image16x16 => new Bitmap(Resources.vso_16_16);

        protected override OutputCategory Category => OutputCategory.BugTracker;

        protected override bool Editable => true;

        protected override bool MultiInstancePossible => true;

        protected override VSOOutput CreateOutput(IWin32Window owner)
        {
            VSOOutput objVsoOutput = new VSOOutput(Name);

            return EditOutput(owner, objVsoOutput);
        }

        protected override VSOOutput EditOutput(IWin32Window owner, VSOOutput output)
        {
            using (EditOutputSettingsForm settingsForm = new EditOutputSettingsForm(new VSOOutput(output)))
            {
                if (settingsForm.ShowDialog(owner) == DialogResult.OK)
                {
                    return settingsForm.Output;
                }

                return null;
            }
        }

        protected override OutputValueCollection SerializeOutput(VSOOutput objVsoOutput)
        {
            return objVsoOutput.Serialize();
        }

        protected override VSOOutput DeserializeOutput(OutputValueCollection objOutputValues)
        {
            return VSOOutput.Deserialize(objOutputValues, Name);
        }

        protected override BugDetails GetSendOptions(IWin32Window owner, VSOOutput vsoOutput, ImageData imageData,
            ref bool cancel)
        {
            using (EditBugDetailsForm bugDetailsForms = new EditBugDetailsForm(vsoOutput))
            {
                if (bugDetailsForms.ShowDialog(owner) == DialogResult.OK)
                    return bugDetailsForms.Options;
                cancel = true;
                return null;
            }
        }

        protected override async void SendAsync(VSOOutput vsoOutput, ImageData imageData, BugDetails sendOptions, SendResult sendResult)
        {
            VSOClient client = new VSOClient(vsoOutput);
            await client.Connect();

            await client.CreateBug(sendOptions, imageData);

            sendResult.Result = enumSendResult.Success;
        }
    }
}