using System;

[Serializable]
[Flags]
public enum TargetStore
{
	iOSAppStore = 1,
	GooglePlay = 2,
	Amazon = 4,
	Nook = 8,
	Editor = 0x10
}
