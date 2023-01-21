using System;
using SteamVR_OculusDash_Switcher.Properties.Localization;

namespace SteamVR_OculusDash_Switcher.Logic
{
    public class SteamVRMethod
    {
	    public readonly BreakMethod Value;
	    public override string ToString() =>
		    Value switch
		    {
			    BreakMethod.None => LocalizationStrings.SteamVRDisableMethod_ToString__None,
			    BreakMethod.RenameFolder => LocalizationStrings.SteamVRDisableMethod_ToString__RenameFolder,
			    BreakMethod.RenameExe => LocalizationStrings.SteamVRDisableMethod_ToString__Renameexe,
			    BreakMethod.DummyExe => LocalizationStrings.SteamVRDisableMethod_ToString__DummyExe,
			    _ => throw new ArgumentOutOfRangeException()
		    };

	    public string Description =>
		    Value switch
		    {
			    BreakMethod.None => LocalizationStrings.SteamVRDisableMethod_Discription__None,
			    BreakMethod.RenameFolder => LocalizationStrings.SteamVRDisableMethod_Discription__RenameFolder,
			    BreakMethod.RenameExe => LocalizationStrings.SteamVRDisableMethod_Discription__RenameExe,
			    BreakMethod.DummyExe => LocalizationStrings.SteamVRDisableMethod_Discription__DummyExe,
			    _ => throw new ArgumentOutOfRangeException()
		    };

	    public SteamVRMethod(BreakMethod Value)
	    {
		    this.Value = Value;
	    }
	    public static SteamVRMethod[] GetAll()
	    {
		    var methodEnums = (BreakMethod[])Enum.GetValues(typeof(BreakMethod));
		    var result = new SteamVRMethod[methodEnums.Length];

		    for (int i = 0; i < methodEnums.Length; i++)
		    {
			    result[i] = new SteamVRMethod(methodEnums[i]);
		    }

		    return result;
	    }

	    public override bool Equals(object? obj) =>
		    obj switch
		    {
			    BreakMethod bm => bm == Value,
			    SteamVR svr => svr.Method == Value,
				SteamVRMethod svrm => Equals(svrm),
				DBNull => false,
			    _ => obj != null && (int)obj == (int)Value
		    };

	    protected bool Equals(SteamVRMethod other)
	    {
		    return Value == other.Value;
	    }

	    public override int GetHashCode()
	    {
		    return (int)Value;
	    }

	    public static implicit operator BreakMethod(SteamVRMethod o) => o.Value;
	    public static implicit operator SteamVRMethod(BreakMethod o) => new(o);
    }

    public enum BreakMethod
    {
	    None = -1,
	    RenameFolder,
	    RenameExe,
	    DummyExe
    }
}
