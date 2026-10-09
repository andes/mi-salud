using System.Text;

namespace AndesServices.Helpers
{
    internal static class CdaPdfHelper
    {
        private static readonly byte[] FirmaPdf = "%PDF"u8.ToArray();

        /// <summary>
        /// Devuelve los bytes del PDF tanto si Andes responde el binario como si lo responde en Base64
        /// (texto plano o string JSON). Devuelve null si el contenido no es ninguno de los dos.
        /// </summary>
        public static byte[]? NormalizarPdf(byte[] contenido)
        {
            if (contenido.AsSpan().StartsWith(FirmaPdf))
                return contenido;

            var texto = Encoding.UTF8.GetString(contenido).Trim().Trim('"');
            if (string.IsNullOrEmpty(texto))
                return null;

            var coma = texto.IndexOf(',');
            if (texto.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && coma >= 0)
                texto = texto[(coma + 1)..];

            try
            {
                var decodificado = Convert.FromBase64String(texto);
                return decodificado.AsSpan().StartsWith(FirmaPdf) ? decodificado : null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
