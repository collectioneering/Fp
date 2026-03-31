namespace Fp.Helpers;

/// <summary>
/// Base unmanaged integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedIntegerArrayHelper<T> : BaseUnmanagedArrayHelper<T> where T : unmanaged, System.Numerics.INumber<T>
;
