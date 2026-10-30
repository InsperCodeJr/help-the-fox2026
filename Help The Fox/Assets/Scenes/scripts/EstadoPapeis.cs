using System.Collections.Generic;

public static class EstadoPapeis
{
    private static readonly List<PapelInventario> papeis = new List<PapelInventario>();

    public static bool Possui(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        return papeis.Exists(papel => papel.Id == id);
    }

    public static bool Guardar(PapelInventario papel)
    {
        if (papel == null || string.IsNullOrWhiteSpace(papel.Id) || Possui(papel.Id))
        {
            return false;
        }

        // Guardamos somente os dados e os sprites, sem depender do objeto da cena.
        papeis.Add(Copiar(papel));
        return true;
    }

    public static List<PapelInventario> PegarPapeisDaMochila()
    {
        List<PapelInventario> copia = new List<PapelInventario>(papeis.Count);
        foreach (PapelInventario papel in papeis)
        {
            copia.Add(Copiar(papel));
        }

        return copia;
    }

    public static int QuantidadePapeis()
    {
        return papeis.Count;
    }

    private static PapelInventario Copiar(PapelInventario papel)
    {
        return new PapelInventario
        {
            Id = papel.Id,
            Nome = papel.Nome,
            SpriteMochila = papel.SpriteMochila,
            SpriteLeitura = papel.SpriteLeitura,
            Texto = papel.Texto
        };
    }
}
