using System;
using Windows.Devices.Radios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Linq;
using Microsoft.Toolkit.Uwp.Notifications;

await Radio.RequestAccessAsync().AsTask();
var bluetooth = (await Radio.GetRadiosAsync().AsTask()).Single(r => r.Kind == RadioKind.Bluetooth);
var prevState = bluetooth.State;

var stateStr = args.Length < 1 ? "toggle" : args[0];


RadioState next;
if (stateStr.ToLower() == "toggle")
{
    if (prevState == RadioState.Off)
        next = RadioState.On;
    else
        next = RadioState.Off;
}
else
    next = (RadioState)Enum.Parse(typeof(RadioState), stateStr, true);

await bluetooth.SetStateAsync(next).AsTask();

new ToastContentBuilder()
    .AddText($"Bluetooth state {bluetooth.State}")
    .AddText(prevState == bluetooth.State ? "Not updated." : $"{prevState} → {bluetooth.State}")
    .Show();

static class Extension
{
    public static Task<T> AsTask<T>(this IAsyncOperation<T> operation)
    {
        var tcs = new TaskCompletionSource<T>();
        operation.Completed = delegate
        {
            switch (operation.Status)   //--- 状態に合わせて完了通知
            {
                case AsyncStatus.Completed: tcs.SetResult(operation.GetResults()); break;
                case AsyncStatus.Error: tcs.SetException(operation.ErrorCode); break;
                case AsyncStatus.Canceled: tcs.SetCanceled(); break;
            }
        };
        return tcs.Task;  //--- 完了が通知されるTaskを返す
    }

}