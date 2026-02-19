using System.Reflection;

namespace SmartPanelGuiasApi.Helpers
{
    public static class AuditoriaHelper
    {
        public static List<string> ObtenerCambios<T>(T original, T modificado)
        {
            var cambios = new List<string>();

            var propiedades = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in propiedades)
            {
                if (!prop.CanRead) continue;

                // Excluir propiedades que no queremos auditar
                if (prop.Name == "NroInt") continue;


                var valorOriginal = prop.GetValue(original);
                var valorNuevo = prop.GetValue(modificado);

                if (!Equals(valorOriginal, valorNuevo))
                {
                    // Caso especial: Descripción (texto largo)
                    if (prop.PropertyType == typeof(string) && prop.Name == "Descripcion")
                    {
                        cambios.Add("Descripción modificada");
                    }
                    else
                    {
                        cambios.Add($"{prop.Name}: {valorOriginal} → {valorNuevo}");
                    }
                }
            }

            return cambios;
        }
    }
}
