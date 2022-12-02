using System;
using Windows.Devices.Radios;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Linq;

await Radio.RequestAccessAsync().AsTask();
var bluetooth = (await Radio.GetRadiosAsync().AsTask()).Single(r => r.Kind == RadioKind.Bluetooth);
Console.WriteLine(bluetooth.State);

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