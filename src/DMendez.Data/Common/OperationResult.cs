namespace DMendez.Data.Common;

/// <summary>Representa el resultado de una operación que retorna un valor.</summary>
/// <typeparam name="T">Tipo del valor retornado en caso de éxito.</typeparam>
public class OperationResult<T>
{
    /// <summary>Indica si la operación fue exitosa.</summary>
    public bool EsExitoso { get; private set; }

    /// <summary>Valor retornado en caso de éxito.</summary>
    public T? Valor { get; private set; }

    /// <summary>Mensaje de error en caso de fallo.</summary>
    public string? MensajeError { get; private set; }

    /// <summary>Crea un resultado exitoso con el valor especificado.</summary>
    /// <param name="valor">El valor resultante de la operación.</param>
    /// <returns>Una instancia de <see cref="OperationResult{T}"/> con éxito.</returns>
    public static OperationResult<T> Exitoso(T valor) =>
        new() { EsExitoso = true, Valor = valor };

    /// <summary>Crea un resultado fallido con el mensaje de error especificado.</summary>
    /// <param name="mensajeError">El mensaje descriptivo del error ocurrido.</param>
    /// <returns>Una instancia de <see cref="OperationResult{T}"/> con fallo.</returns>
    public static OperationResult<T> Fallido(string mensajeError) =>
        new() { EsExitoso = false, MensajeError = mensajeError };
}

/// <summary>Representa el resultado de una operación sin valor de retorno.</summary>
public class OperationResult
{
    /// <summary>Indica si la operación fue exitosa.</summary>
    public bool EsExitoso { get; private set; }

    /// <summary>Mensaje de error en caso de fallo.</summary>
    public string? MensajeError { get; private set; }

    /// <summary>Crea un resultado exitoso.</summary>
    /// <returns>Una instancia de <see cref="OperationResult"/> con éxito.</returns>
    public static OperationResult Exitoso() => new() { EsExitoso = true };

    /// <summary>Crea un resultado fallido con el mensaje de error especificado.</summary>
    /// <param name="mensajeError">El mensaje descriptivo del error ocurrido.</param>
    /// <returns>Una instancia de <see cref="OperationResult"/> con fallo.</returns>
    public static OperationResult Fallido(string mensajeError) =>
        new() { EsExitoso = false, MensajeError = mensajeError };
}
