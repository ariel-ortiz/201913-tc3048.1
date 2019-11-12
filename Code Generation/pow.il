.assembly extern mscorlib
{
  .ver 4:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly 'pow'
{
  .custom instance void class [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::'.ctor'() =  (
		01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
		63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01       ) // ceptionThrows.

  .hash algorithm 0x00008004
  .ver  0:0:0:0
}
.module pow.exe // GUID = {8B1A6366-AE04-4BC6-B9E2-BA5A467C0473}


  .class private auto ansi beforefieldinit Pow
  	extends [mscorlib]System.Object
  {

    // method line 1
    .method public hidebysig specialname rtspecialname 
           instance default void '.ctor' ()  cil managed 
    {
        // Method begins at RVA 0x2050
	// Code size 7 (0x7)
	.maxstack 8
	IL_0000:  ldarg.0 
	IL_0001:  call instance void object::'.ctor'()
	IL_0006:  ret 
    } // end of method Pow::.ctor

    // method line 2
    .method public static hidebysig 
           default void Main ()  cil managed 
    {
        // Method begins at RVA 0x2058
	.entrypoint
	// Code size 22 (0x16)
	.maxstack 2
	.locals init (
		int32	V_0,
		int32	V_1,
		int32	V_2)
	IL_0000:  ldc.i4.2 
	IL_0001:  stloc.0 
	IL_0002:  ldc.i4.3 
	IL_0003:  stloc.1 
	IL_0004:  ldloc.0 
	IL_0005:  conv.r8 
	IL_0006:  ldloc.1 
	IL_0007:  conv.r8 
	IL_0008:  call float64 class [mscorlib]System.Math::Pow(float64, float64)
	IL_000d:  conv.i4 
	IL_000e:  stloc.2 
	IL_000f:  ldloc.2 
	IL_0010:  call void class [mscorlib]System.Console::WriteLine(int32)
	IL_0015:  ret 
    } // end of method Pow::Main

  } // end of class Pow

