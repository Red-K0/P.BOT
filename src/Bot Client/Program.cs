if (!PHelper.EnableVirtualAndHideCursor()) throw new OperationCanceledException("VT_Sequences (VTs.dll) failed to initlialize the console.");
Console.OutputEncoding = System.Text.Encoding.Unicode;

Client.Start();
Client.StartInteractionHandler();

await Task.Delay(Timeout.Infinite);