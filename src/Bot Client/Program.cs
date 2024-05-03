using PBot.Commands.Helpers;

if (!PHelper.EnableVirtualAndHideCursor()) throw new OperationCanceledException("VT_Sequences (VTs.dll) failed to initlialize the console.");
Console.OutputEncoding = System.Text.Encoding.Unicode;

Start();
StartInteractionHandler();

ProbabilityStateMachine.InitXShift128();

await Task.Delay(Timeout.Infinite);